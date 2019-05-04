using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Decorators
{
    public class LogDecorator : IAuthenticationService
    {
        private readonly IAuthenticationService _authentication;
        private readonly IFailedCounter _failedCounter;
        private readonly ILogger _logger;

        public LogDecorator(IAuthenticationService authentication, IFailedCounter failedCounter, ILogger logger)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
            _logger = logger;
        }

        private void LogVerify(string accountId)
        {
            var failedCount = _failedCounter.Get(accountId);

            _logger.Info($"Account: {accountId}, valid Failed {failedCount} times.");
        }

        public bool Valid(string accountId, string password, string otp)
        {
            var isValid = _authentication.Valid(accountId, password, otp);
            if (isValid == false)
            {
                LogVerify(accountId);
            }

            return isValid;
        }
    }
}