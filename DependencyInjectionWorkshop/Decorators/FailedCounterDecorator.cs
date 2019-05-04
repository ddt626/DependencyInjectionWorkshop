using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Decorators
{
    public class FailedCounterDecorator : IAuthenticationService
    {
        private readonly IAuthenticationService _authentication;
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthenticationService authentication, IFailedCounter failedCounter)
        {
            _authentication = authentication;
            _failedCounter = failedCounter;
        }

        private void AccountIsLock(string accountId)
        {
            var isLock = _failedCounter.CheckAccountIsLock(accountId);
            if (isLock)
            {
                throw new ValidFailedManyTimeException();
            }
        }

        public bool Valid(string accountId, string password, string otp)
        {
            AccountIsLock(accountId);
            var isValid = _authentication.Valid(accountId, password, otp);
            if (isValid)
            {
                _failedCounter.Reset(accountId);
            }
            else
            {
                _failedCounter.Add(accountId);
            }

            return isValid;
        }
    }
}