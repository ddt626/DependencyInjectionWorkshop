using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Decorators
{
    public class FailedCounterDecorator : AuthenticationBaseDecorator, IAuthenticationService
    {
        private readonly IFailedCounter _failedCounter;

        public FailedCounterDecorator(IAuthenticationService authentication, IFailedCounter failedCounter) : base(
            authentication)
        {
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

        public override bool Valid(string accountId, string password, string otp)
        {
            AccountIsLock(accountId);
            var isValid = base.Valid(accountId, password, otp);
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