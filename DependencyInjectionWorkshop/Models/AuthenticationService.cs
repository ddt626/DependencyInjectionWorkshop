using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        private IFailedCounter _failedCounter;
        private IProfile _profileRepo;
        private IHash _sha256Adapter;
        private IOtp _otpService;
        private INotification _slackAdapter;
        private ILogger _nLogAdapter;

        public AuthenticationService(
            IFailedCounter failedCounter,
            IProfile profileRepo,
            IHash sha256Adapter,
            IOtp otpService,
            INotification slackAdapter,
            ILogger nLogAdapter)
        {
            _failedCounter = failedCounter;
            _profileRepo = profileRepo;
            _sha256Adapter = sha256Adapter;
            _otpService = otpService;
            _slackAdapter = slackAdapter;
            _nLogAdapter = nLogAdapter;
        }

        public AuthenticationService()
        {
            _failedCounter = new FailedCounter();
            _profileRepo = new ProfileRepo();
            _sha256Adapter = new Sha256Adapter();
            _otpService = new OtpService();
            _slackAdapter = new SlackAdapter();
            _nLogAdapter = new NLogAdapter();
        }

        public bool Valid(string accountId, string password, string otp)
        {
            _failedCounter.CheckAccountIsLock(accountId);

            var dbPassword = _profileRepo.GetPassword(accountId);

            var hashedPassword = _sha256Adapter.GetPassword(password);

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

                _slackAdapter.PushMessage($"account:{accountId} verify failed");

                _nLogAdapter.Info($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}