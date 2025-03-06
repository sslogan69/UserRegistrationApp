using CoreWf;
using CoreWf.Statements;
using Microsoft.Extensions.Logging;
using UserRegistrationApp.Activities;

namespace UserRegistrationApp.Workflows
{
    public class UserRegistrationWorkflow : NativeActivity
    {
        public InArgument<string> UserName { get; set; }
        public InArgument<string> Email { get; set; }
        private readonly ILoggerFactory _loggerFactory;

        public UserRegistrationWorkflow(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            var sequence = new Sequence
            {
                Activities =
                {
                    new ValidateUserInput(_loggerFactory)
                    {
                        UserName = new InArgument<string>(ctx => UserName.Get(ctx)),
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    },
                    new CreateUser(_loggerFactory)
                    {
                        UserName = new InArgument<string>(ctx => UserName.Get(ctx)),
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    },
                    new SendConfirmationEmail(_loggerFactory)
                    {
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    }
                }
            };

            metadata.AddChild(sequence);
        }

        protected override void Execute(NativeActivityContext context)
        {
            var sequence = new Sequence
            {
                Activities =
                {
                    new ValidateUserInput(_loggerFactory)
                    {
                        UserName = new InArgument<string>(ctx => UserName.Get(ctx)),
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    },
                    new CreateUser(_loggerFactory)
                    {
                        UserName = new InArgument<string>(ctx => UserName.Get(ctx)),
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    },
                    new SendConfirmationEmail(_loggerFactory)
                    {
                        Email = new InArgument<string>(ctx => Email.Get(ctx))
                    }
                }
            };

            WorkflowInvoker.Invoke(sequence, new Dictionary<string, object>
            {
                { "UserName", context.GetValue(UserName) },
                { "Email", context.GetValue(Email) }
            });
        }
    }
}
