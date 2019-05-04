using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private IFailedCounter _failedCounter;
        private IProfile _profile;
        private IHash _hash;
        private IOtp _otpService;
        private INotification _notification;
        private ILogger _logger;

        public AuthenticationService(
            IFailedCounter failedCounter,
            IProfile profile,
            IHash hash,
            IOtp otpService,
            INotification notification,
            ILogger logger)
        {
            _failedCounter = failedCounter;
            _profile = profile;
            _hash = hash;
            _otpService = otpService;
            _notification = notification;
            _logger = logger;
        }

        public AuthenticationService()
        {
            _failedCounter = new FailedCounter();
            _profile = new ProfileRepo();
            _hash = new Sha256Adapter();
            _otpService = new OtpService();
            _notification = new SlackAdapter();
            _logger = new NLogAdapter();
        }

        public bool Valid(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profile.GetPassword(accountId);

            var hashedPassword = _hash.GetPassword(password);

            var currentOtp = _otpService.GetCurrent(accountId);

            if (dbPassword == hashedPassword && otp == currentOtp)
            {
                _failedCounter.Reset(accountId);

                return true;
            }
            else
            {
                _failedCounter.Add(accountId);

                var failedCount = _failedCounter.Get(accountId);

                _notification.PushMessage($"account:{accountId} verify failed");

                _logger.Info($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}