using MessengerAvalonia.Shared.ChatsGrpc;
using MessengerAvalonia.Shared.FriendsGrpc;
using System.Collections.Concurrent;

namespace Server.Services
{
    public class ChatService : ChatsService.ChatsServiceBase
    {
        private readonly ILogger<ChatService> _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }
        public override Task<GetChatsResponse> GetMessages(GetChatsRequest request, Grpc.Core.ServerCallContext context)
        {
            _logger.LogInformation(
                "Received GetMessages request from {Username} to add {FriendLogin}",
                request.UserLogin,
                request.FriendLogin);

            var db = new DatabaseHelper("Database/DataBase.db");
            var list = db.GetMessages(request.UserLogin, request.FriendLogin);
            Console.WriteLine($"Chat list count: {list.Count}");

            var response = new GetChatsResponse();

            foreach (var item in list)
            {
                response.Writer.Add(item.Writer);
                response.Message.Add(item.Message);
            }

            return Task.FromResult(response);
        }
        

        public override Task<AddMessageResponse> SendMessage(AddMessageRequest request, Grpc.Core.ServerCallContext context)
        {
            _logger.LogInformation(
                "Received AddFriend request from {Username} to add {FriendLogin}",
                request.UserLogin,
                request.FriendLogin);

            var db = new DatabaseHelper("Database/DataBase.db");
            bool success = db.SendMessage(request.UserLogin, request.FriendLogin, request.Text);

            return Task.FromResult(new AddMessageResponse
            {
                Success = success,
                Message = success ? "Sent" : "Error"
            });
        }
    }
}
