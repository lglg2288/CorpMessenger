using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Data.Sqlite;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Server
{
    public class DatabaseHelper
    {
        private readonly string _dbConnectionString;
        private SqliteConnection _dbConnection;
        public DatabaseHelper(string DatabaseConnectionString)
        {
            _dbConnectionString = DatabaseConnectionString;
            _dbConnection = new SqliteConnection($"Data Source ={_dbConnectionString}");
            _dbConnection.Open();
        }
        public RegistrationStatus Registration(string login, string password)
        {
            string query = "SELECT login FROM Users WHERE login = @Login;";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return new RegistrationStatus { Success = false, Message = "Login already exists." };
                }
            }

            query = "INSERT INTO Users (login, password) VALUES (@Login, @Password);";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.ExecuteNonQuery();
                return new RegistrationStatus { Success = true, Message = "Registration successful." };
            }
            return null;
        }
        public bool Login(string login, string password)
        {
            string query = "SELECT login, password FROM Users WHERE login = @Login AND password = @Password;";
            using (SqliteCommand cmd = new SqliteCommand(query, _dbConnection))
            {
                cmd.Parameters.AddWithValue("@Login", login);
                cmd.Parameters.AddWithValue("@Password", password);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return true;
                    else
                        return false;
                }
            }
        }
        public AddFriendStatus AddFriend(string userLogin, string friendLogin)
        {
            if (userLogin == friendLogin)
                return new AddFriendStatus { Success = false, Message = "You cannot add yourself." };

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                // 1. Получаем ID пользователей
                long? userId = GetUserId(userLogin, transaction);
                long? friendId = GetUserId(friendLogin, transaction);

                if (userId == null || friendId == null)
                    return new AddFriendStatus { Success = false, Message = "User not found." };

                // 2. Проверяем, существует ли уже общая комната
                string checkQuery = @"
                    SELECT ur1.room_id
                    FROM UsersRoom ur1
                    JOIN UsersRoom ur2 ON ur1.room_id = ur2.room_id
                    WHERE ur1.user_id = @UserId AND ur2.user_id = @FriendId;
                ";

                using (var checkCmd = new SqliteCommand(checkQuery, _dbConnection, transaction))
                {
                    checkCmd.Parameters.AddWithValue("@UserId", userId);
                    checkCmd.Parameters.AddWithValue("@FriendId", friendId);

                    var existingRoom = checkCmd.ExecuteScalar();
                    if (existingRoom != null)
                        return new AddFriendStatus { Success = false, Message = "Chat already exists." };
                }

                // 3. Создаём комнату
                string insertRoom = "INSERT INTO ChatRoom (name) VALUES (NULL); SELECT last_insert_rowid();";
                long roomId;

                using (var roomCmd = new SqliteCommand(insertRoom, _dbConnection, transaction))
                {
                    roomId = Convert.ToInt64(roomCmd.ExecuteScalar());

                }

                // 4. Добавляем связи
                string insertUsersRoom = @"
                    INSERT INTO UsersRoom (name, user_id, room_id)
                    VALUES (@Name, @UserId, @RoomId);
                ";

                using (var cmd1 = new SqliteCommand(insertUsersRoom, _dbConnection, transaction))
                {
                    cmd1.Parameters.AddWithValue("@Name", friendLogin);
                    cmd1.Parameters.AddWithValue("@UserId", userId);
                    cmd1.Parameters.AddWithValue("@RoomId", roomId);
                    cmd1.ExecuteNonQuery();
                }

                using (var cmd2 = new SqliteCommand(insertUsersRoom, _dbConnection, transaction))
                {
                    cmd2.Parameters.AddWithValue("@Name", userLogin);
                    cmd2.Parameters.AddWithValue("@UserId", friendId);
                    cmd2.Parameters.AddWithValue("@RoomId", roomId);
                    cmd2.ExecuteNonQuery();
                }

                transaction.Commit();

                return new AddFriendStatus { Success = true, Message = "Chat created successfully." };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new AddFriendStatus { Success = false, Message = ex.Message };
            }
        }

        public List<string> GetRooms(string login)
        {
            var friends = new List<string>();
            string query = @"
                SELECT name FROM UsersRoom
                JOIN Users ON UsersRoom.user_id = Users.ID
                WHERE Users.login = @Login;
            ";

            using var cmd = new SqliteCommand(query, _dbConnection);
            cmd.Parameters.AddWithValue("@Login", login);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                friends.Add(reader.GetString(0));
            }

            return friends;
        }

        public bool SendMessage(string userLogin, string friendLogin, string text)
        {
            var roomId = GetRoomId(userLogin, friendLogin);
            var userId = GetUserId(userLogin);

            if (roomId == null || userId == null)
                return false;

            string query = @"
                INSERT INTO Messages (Context, user_id, room_id)
                VALUES (@Text, @UserId, @RoomId);
            ";

            using var cmd = new SqliteCommand(query, _dbConnection);
            cmd.Parameters.AddWithValue("@Text", text);
            cmd.Parameters.AddWithValue("@UserId", userId);
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            cmd.ExecuteNonQuery();
            return true;
        }
        public List<(string Writer, string Message)> GetMessages(string userLogin, string friendLogin)
        {
            var result = new List<(string, string)>();

            var roomId = GetRoomId(userLogin, friendLogin);
            if (roomId == null)
                return result;

            string query = @"
                SELECT u.login, m.Context
                FROM Messages m
                JOIN Users u ON m.user_id = u.ID
                WHERE m.room_id = @RoomId
                ORDER BY m.ID ASC;
            ";

            using var cmd = new SqliteCommand(query, _dbConnection);
            cmd.Parameters.AddWithValue("@RoomId", roomId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string writer = reader.GetString(0);
                string message = reader.GetString(1);

                result.Add((writer, message));
            }

            return result;
        }


        private long? GetUserId(string login)
        {
            string query = "SELECT ID FROM Users WHERE login = @Login;";
            using var cmd = new SqliteCommand(query, _dbConnection);
            cmd.Parameters.AddWithValue("@Login", login);

            var result = cmd.ExecuteScalar();
            return result == null ? null : Convert.ToInt64(result);
        }

        private int? GetUserId(string login, SqliteTransaction transaction)
        {
            string query = "SELECT ID FROM Users WHERE login = @Login;";
            using var cmd = new SqliteCommand(query, _dbConnection, transaction);
            cmd.Parameters.AddWithValue("@Login", login);

            var result = cmd.ExecuteScalar();
            return result == null ? null : Convert.ToInt32(result);
        }
        private long? GetRoomId(string userLogin, string friendLogin)
        {
            string query = @"
                SELECT ur1.room_id
                FROM UsersRoom ur1
                JOIN Users u1 ON ur1.user_id = u1.ID
                JOIN UsersRoom ur2 ON ur1.room_id = ur2.room_id
                JOIN Users u2 ON ur2.user_id = u2.ID
                WHERE u1.login = @UserLogin
                AND u2.login = @FriendLogin;
            ";

            using var cmd = new SqliteCommand(query, _dbConnection);
            cmd.Parameters.AddWithValue("@UserLogin", userLogin);
            cmd.Parameters.AddWithValue("@FriendLogin", friendLogin);

            var result = cmd.ExecuteScalar();
            return result == null ? null : Convert.ToInt64(result);
        }
    }
    public class RegistrationStatus
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class AddFriendStatus
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}