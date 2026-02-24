using Grpc.Net.Client;
using MessengerAvalonia.Shared.LoginGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interprice
{
    public class GrpcLoginService : Client.Services.ILoginService
    {
        private readonly Loginer.LoginerClient _client;

        public GrpcLoginService(GrpcChannel channel)
        {
            _client = new Loginer.LoginerClient(channel);
        }

        public async Task<LoginResponse> SignInAsync(string login, string password)
        {
            var request = new LoginRequest
            {
                Login = login,
                Password = password
            };
            try
            {
                return await _client.SignInAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during SignInAsync: " + ex.Message, ex);
            }
        }
    }
}
