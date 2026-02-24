using Grpc.Net.Client;
using MessengerAvalonia.Shared.ChatsGrpc;
using MessengerAvalonia.Shared.LoginGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interprice
{
    public class GrpcChatsService : Client.Services.IChatsService
    {
        private readonly ChatsService.ChatsServiceClient _client;
        public GrpcChatsService(GrpcChannel channel)
        {
            _client = new ChatsService.ChatsServiceClient(channel);
        }

        public async Task<GetChatsResponse> GetMessages(string login, string password, string friendsLogin)
        {
            var request = new GetChatsRequest
            {
                UserLogin = login,
                FriendLogin = friendsLogin
            };
            return await _client.GetMessagesAsync(request);
            try
            {
                return await _client.GetMessagesAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during GetMessages: " + ex.Message, ex);
            }
        }
        public async Task<AddMessageResponse> SendMessage(string login, string password, string friendsLogin, string text)
        {
            var request = new AddMessageRequest
            {
                UserLogin = login,
                FriendLogin = friendsLogin,
                Text = text
            };
            try
            {
                return await _client.SendMessageAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during SendMessage: " + ex.Message, ex);
            }
        }
    }
}