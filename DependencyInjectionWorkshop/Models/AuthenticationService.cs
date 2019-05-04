using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private readonly ProfileRepo _profileRepo = new ProfileRepo();
        private readonly Sha256Adapter _sha256Adapter = new Sha256Adapter();
        private readonly OtpService _otpService = new OtpService();
        private readonly FailedCounter _failedCounter = new FailedCounter();
        private readonly SlackAdapter _slackAdapter = new SlackAdapter();
        private readonly NLogAdapter _nLogAdapter = new NLogAdapter();

        public bool Valid(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profileRepo.GetDbPassword(accountId);

            var hashedPassword = _sha256Adapter.GetHashedPassword(password);

            var currentOtp = _otpService.GetCurrentOtp(accountId);

            if (dbPassword == hashedPassword && otp == currentOtp)
            {
                _failedCounter.ResetFailedCounter(accountId);

                return true;
            }
            else
            {
                _failedCounter.AddFailedCount(accountId);

                var failedCount = _failedCounter.GetFailedCount(accountId);

                _slackAdapter.Notify($"account:{accountId} verify failed");

                _nLogAdapter.Log($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}