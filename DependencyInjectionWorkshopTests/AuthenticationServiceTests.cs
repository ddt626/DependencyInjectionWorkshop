using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace DependencyInjectionWorkshopTests
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private const string defaultAccountId = "Ray";
        private const string defaultOtp = "123456";
        private const string defaultHashedPassword = "Hashed Password";
        private const string defaultPassword = "pw";
        private const int defaultFailedCount = 91;
        private IProfile _profile;
        private IHash _hash;
        private IOtp _otpService;
        private INotification _notification;
        private IFailedCounter _failedCounter;
        private ILogger _logger;
        private AuthenticationService _authenticationService;

        [SetUp]
        public void SetUp()
        {
            _profile = Substitute.For<IProfile>();
            _hash = Substitute.For<IHash>();
            _otpService = Substitute.For<IOtp>();
            _notification = Substitute.For<INotification>();
            _failedCounter = Substitute.For<IFailedCounter>();
            _logger = Substitute.For<ILogger>();

            _authenticationService =
                new AuthenticationService(_failedCounter, _profile, _hash, _otpService, _notification, _logger);
        }

        [Test]
        public void is_valid()
        {
            GivenOtp(defaultAccountId, defaultOtp);
            GivenPassword(defaultAccountId, defaultHashedPassword);
            GivenHash(defaultPassword, defaultHashedPassword);

            var isValid = WhenVerify();
            ShouldBeValid(isValid);
        }

        [Test]
        public void is_invalid_wrong_otp()
        {
            GivenOtp(defaultAccountId, "wrong_otp");
            GivenPassword(defaultAccountId, defaultHashedPassword);
            GivenHash(defaultPassword, defaultHashedPassword);

            var isValid = WhenVerify();
            WhenIsInValid(isValid);
        }

        [Test]
        public void is_invalid_notify_user()
        {
            WhenInValid();
            ShouldNotifyUser();
        }


        [Test]
        public void is_invalid_log_failed_count()
        {
            GivenFailedCount(defaultFailedCount);
            WhenInValid();
            ShouldLogContain(defaultAccountId, defaultFailedCount);
        }

        [Test]
        public void add_failedCount_when_invalid()
        {
            WhenInValid();
            ShouldAddFailedCount(defaultAccountId);
        }


        [Test]
        public void reset_failed_counter_when_valid()
        {
            WhenValid();

            _failedCounter.Received(1).Reset(defaultAccountId);
        }

        private void WhenValid()
        {
            GivenOtp(defaultAccountId, defaultOtp);
            GivenPassword(defaultAccountId, defaultHashedPassword);
            GivenHash(defaultPassword, defaultHashedPassword);

            WhenVerify();
        }

        private void ShouldAddFailedCount(string accountId)
        {
            _failedCounter.Received(1).Add(Arg.Is<string>(m => m == accountId));
        }

        private void ShouldLogContain(string accountId, int failedCount)
        {
            _logger.Received(1).Info(Arg.Is<string>(m => m.Contains(accountId)
                                                         && m.Contains(failedCount.ToString())));
        }

        private void GivenFailedCount(int failedCount)
        {
            _failedCounter.Get(defaultAccountId).Returns(failedCount);
        }


        private void WhenInValid()
        {
            GivenOtp(defaultAccountId, "wrong_otp");
            GivenPassword(defaultAccountId, defaultHashedPassword);
            GivenHash(defaultPassword, defaultHashedPassword);

            WhenVerify();
        }

        private void ShouldNotifyUser()
        {
            _notification.Received(1).PushMessage(Arg.Any<string>());
        }

        private static void WhenIsInValid(bool isValid)
        {
            Assert.IsFalse(isValid);
        }

        private bool WhenVerify()
        {
            var isValid = _authenticationService.Valid(defaultAccountId, defaultPassword, defaultOtp);
            return isValid;
        }

        private static void ShouldBeValid(bool isValid)
        {
            Assert.IsTrue(isValid);
        }

        private void GivenHash(string password, string hashedPassword)
        {
            _hash.GetPassword(password).ReturnsForAnyArgs(hashedPassword);
        }

        private void GivenPassword(string accountId, string password)
        {
            _profile.GetPassword(accountId).ReturnsForAnyArgs(password);
        }

        private void GivenOtp(string accountId, string otp)
        {
            _otpService.GetCurrent(accountId).ReturnsForAnyArgs(otp);
        }
    }
}