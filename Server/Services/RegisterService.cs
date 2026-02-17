using Grpc.Core;
using MessengerAvalonia.Shared.RegisterGrpc;

namespace Server.Services
{
    public class RegisterService : Register.RegisterBase
    {
        private readonly ILogger<RegisterService> _logger;

        public RegisterService(ILogger<RegisterService> logger)
        {
            _logger = logger;
        }

        public override Task<RegisterResponse> SignUp(RegisterRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received registration request for username: {Username}", request.Login);

            var db = new DatabaseHelper("Database/DataBase.db");

            RegistrationStatus dbResponse = db.Registration(request.Login, request.Password);

            var response = new RegisterResponse
            {
                Success = dbResponse.Success,
                Message = dbResponse.Message
            };

            return Task.FromResult(response);
        }
    }
}