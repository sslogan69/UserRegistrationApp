using CoreWf;
using Microsoft.Extensions.Logging;

namespace UserRegistrationApp.Activities
{
    public class ValidateUserInput : CodeActivity
    {
        public InArgument<string> UserName { get; set; }
        public InArgument<string> Email { get; set; }
        private readonly ILogger<ValidateUserInput> _logger;

        public ValidateUserInput(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ValidateUserInput>();
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.AddArgument(new RuntimeArgument("UserName", typeof(string), ArgumentDirection.In, true));
            metadata.AddArgument(new RuntimeArgument("Email", typeof(string), ArgumentDirection.In, true));
        }

        protected override void Execute(CodeActivityContext context)
        {
            string userName = context.GetValue(UserName);
            string email = context.GetValue(Email);

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Validation failed: UserName or Email is empty.");
                throw new ValidationException("UserName and Email are required.");
            }

            _logger.LogInformation("Validation succeeded for UserName: {UserName}, Email: {Email}", userName, email);
        }
    }
}
