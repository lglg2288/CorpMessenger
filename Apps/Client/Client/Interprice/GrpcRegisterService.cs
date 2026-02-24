using Grpc.Net.Client;
using MessengerAvalonia.Shared.LoginGrpc;
using MessengerAvalonia.Shared.RegisterGrpc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Interprice
{
    public class GrpcRegisterService : Services.IRegisterService
    {
        private readonly Register.RegisterClient _client;
        public GrpcRegisterService(GrpcChannel channel)
        {
            _client = new Register.RegisterClient(channel);
        }

        public async Task<RegisterResponse> RegisterAsync(string login, string password)
        {
            var request = new RegisterRequest
            {
                Login = login,
                Password = password
            };
            try
            {
                return await _client.SignUpAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception("Error during RegisterAsync: " + ex.Message, ex);
            }
        }
    }
}
