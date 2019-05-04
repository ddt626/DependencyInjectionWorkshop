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