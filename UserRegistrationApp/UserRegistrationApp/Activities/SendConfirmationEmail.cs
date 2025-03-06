using CoreWf;
using Microsoft.Extensions.Logging;
using System;

namespace UserRegistrationApp.Activities
{
    public class SendConfirmationEmail : CodeActivity
    {
        public InArgument<string> Email { get; set; }
        private readonly ILogger<SendConfirmationEmail> _logger;

        public SendConfirmationEmail(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendConfirmationEmail>();
        }

        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.AddArgument(new RuntimeArgument("Email", typeof(string), ArgumentDirection.In, true));
        }

        protected override void Execute(CodeActivityContext context)
        {
            string email = context.GetValue(Email);

            _logger.LogInformation("Sending confirmation email to: {Email}", email);
            Console.WriteLine($"Confirmation email sent to: {email}");
        }
    }
}
