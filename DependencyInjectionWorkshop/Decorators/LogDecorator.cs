using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Decorators
{
    public class LogDecorator : AuthenticationBaseDecorator, IAuthenticationService
    {
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorator(IAuthenticationService authentication, IFailedCounter failedCounter, ILogger logger) : base(
            authentication)
        {
            _failedCounter = failedCounter;
            _logger = logger;
        }

        private void LogVerify(string accountId)
        {
            var failedCount = _failedCounter.Get(accountId);

            _logger.Info($"Account: {accountId}, valid Failed {failedCount} times.");
        }

        public override bool Valid(string accountId, string password, string otp)
        {
            var isValid = base.Valid(accountId, password, otp);
            if (isValid == false)
            {
                LogVerify(accountId);
            }

            return isValid;
        }
    }
}