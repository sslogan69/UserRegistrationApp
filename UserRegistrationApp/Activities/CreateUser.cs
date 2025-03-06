using CoreWf;
using Microsoft.Extensions.Logging;
using System;

namespace UserRegistrationApp.Activities
{
    public class CreateUser : CodeActivity
    {
        public InArgument<string> UserName { get; set; }
        public InArgument<string> Email { get; set; }
        private readonly ILogger<CreateUser> _logger;

        public CreateUser(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateUser>();
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
            _logger.LogDebug("Starting user creation for {UserName}, {Email}", userName, email);
            _logger.LogInformation("Creating user: {UserName}, {Email}", userName, email);
            try
            {
                Console.WriteLine($"User created: {userName}, {email}");
                _logger.LogInformation("User created successfully: {UserName}, {Email}", userName, email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user: {UserName}, {Email}", userName, email);
                throw;
            }
        }
    }
}
