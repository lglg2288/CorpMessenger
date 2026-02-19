using MessengerAvalonia.Shared.FriendsGrpc;
using System.Collections.Concurrent;

namespace Server.Services
{
    public class FriendService : FriendsService.FriendsServiceBase
    {
        private readonly ILogger<FriendService> _logger;

        public FriendService(ILogger<FriendService> logger)
        {
            _logger = logger;
        }
        public override Task<AddFriendResponse> AddFriend(AddFriendRequest request, Grpc.Core.ServerCallContext context)
        {
            _logger.LogInformation(
                "Received AddFriend request from {Username} to add {FriendLogin}",
                request.UserLogin,
                request.FriendLogin);

            var db = new DatabaseHelper("Database/DataBase.db");
            var dbResponse = db.AddFriend(request.UserLogin, request.FriendLogin);
            var response = new AddFriendResponse
            {
                Success = dbResponse.Success,
                Message = dbResponse.Message
            };
            return Task.FromResult(response);
        }

        public override Task<FriendsResponse> GetFriends(FriendsRequest request, Grpc.Core.ServerCallContext context)
        {
            _logger.LogInformation("Received GetFriends request for username: {Username}", request.Login);
            var db = new DatabaseHelper("Database/DataBase.db");
            var friendsList = db.GetRooms(request.Login);
            var response = new FriendsResponse();
            response.Friends.AddRange(friendsList);
            Console.WriteLine($"Friends list count: {friendsList.Count}");
            return Task.FromResult(response);
        }
    }
}
