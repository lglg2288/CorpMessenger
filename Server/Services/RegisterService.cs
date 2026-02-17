using Grpc.Core;
using MessengerAvalonia.Shared;

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

            var response = new RegisterResponse
            {
                Success = true,
                Message = "Registration successful!"
            };

            return Task.FromResult(response);
        }
    }
}
