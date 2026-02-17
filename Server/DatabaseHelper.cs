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
    }
    public class RegistrationStatus
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
