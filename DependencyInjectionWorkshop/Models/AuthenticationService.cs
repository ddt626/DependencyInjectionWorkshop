using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public class NotificationDecorator : IAuthenticationService
    {
        private IAuthenticationService _authentication;
        private readonly INotification _notification;

        public NotificationDecorator(IAuthenticationService authentication, INotification notification)
        {
            _authentication = authentication;
            _notification = notification;
        }

        private void PushMessage(string accountId)
        {
            _notification.PushMessage($"account:{accountId} verify failed");
        }

        public bool Valid(string accountId, string password, string otp)
        {
            var isValid = _authentication.Valid(accountId, password, otp);
            if (!isValid)
            {
                PushMessage(accountId);
            }

            return isValid;
        }
    }

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

        public bool Valid(string accountId, string password, string otp)
        {
            var isLock = _failedCounter.CheckAccountIsLock(accountId);
            if (isLock)
            {
                throw new ValidFailedManyTimeException();
            }

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

                //_notificationDecorator.PushMessage(accountId);

                _logger.Info($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}