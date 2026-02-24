using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using MessengerAvalonia.Shared.FriendsGrpc;

namespace Client.Interprice
{
    public class GrpcFriendService : Client.Services.IFriendService
    {
        private readonly FriendsService.FriendsServiceClient _client;

        public GrpcFriendService(GrpcChannel channel)
        {
            _client = new FriendsService.FriendsServiceClient(channel);
        }

        public async Task<FriendsResponse> GetFriendsAsync(string login, string password)
        {
            var request = new FriendsRequest
            {
                Login = login,
            };
            try
            {
                return await _client.GetFriendsAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during GetFriendsAsync: " + ex.Message, ex);
            }
        }
        public async Task<AddFriendResponse> AddFriendAsync(string login, string password, string friendslogin)
        {
            var request = new AddFriendRequest
            {
                UserLogin = login,
                FriendLogin = friendslogin
            };
            try
            {
                return await _client.AddFriendAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during AddFriendAsync: " + ex.Message, ex);
            }
        }
    }
}
