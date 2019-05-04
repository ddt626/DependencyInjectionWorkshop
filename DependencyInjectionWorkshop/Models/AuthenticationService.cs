using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public interface IAuthenticationService
    {
        bool Valid(string accountId, string password, string otp);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IFailedCounter _failedCounter;
        private IProfile _profile;
        private IHash _hash;
        private IOtp _otpService;
        private ILogger _logger;

        public AuthenticationService(
            IFailedCounter failedCounter,
            IProfile profile,
            IHash hash,
            IOtp otpService,
            ILogger logger)
        {
            _failedCounter = failedCounter;
            _profile = profile;
            _hash = hash;
            _otpService = otpService;
            _logger = logger;
        }

        public bool Valid(string accountId, string password, string otp)
        {
            var dbPassword = _profile.GetPassword(accountId);

            var hashedPassword = _hash.GetPassword(password);

            var currentOtp = _otpService.GetCurrent(accountId);

            if (dbPassword == hashedPassword && otp == currentOtp)
            {
                return true;
            }
            else
            {
                var failedCount = _failedCounter.Get(accountId);

                _logger.Info($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}