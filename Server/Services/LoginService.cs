using Grpc.Core;
using MessengerAvalonia.Shared.LoginGrpc;
using MessengerAvalonia.Shared.RegisterGrpc;

namespace Server.Services
{
    public class LoginService : Loginer.LoginerBase
    {
        private readonly ILogger<LoginService> _logger;

        public LoginService(ILogger<LoginService> logger)
        {
            _logger = logger;
        }

        public override Task<LoginResponse> SignIn(LoginRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received login request for username: {Username}", request.Login);

            var db = new DatabaseHelper("Database/DataBase.db");

            bool successLogin = db.Login(request.Login, request.Password);

            var response = new LoginResponse
            {
                Success = successLogin,
                Message = successLogin ? "Login successful." : "Invalid username or password."
            };

            return Task.FromResult(response);
        }
    }
}