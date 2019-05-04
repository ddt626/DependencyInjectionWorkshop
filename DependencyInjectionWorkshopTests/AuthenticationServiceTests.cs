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
        [Test]
        public void is_valid()
        {
            var profile = Substitute.For<IProfile>();
            var hash = Substitute.For<IHash>();
            var otp = Substitute.For<IOtp>();
            var notification = Substitute.For<INotification>();
            var failedCounter = Substitute.For<IFailedCounter>();
            var logger = Substitute.For<ILogger>();

            var authenticationService =
                new AuthenticationService(failedCounter, profile, hash, otp, notification, logger);

            otp.GetCurrent("Ray").Returns("123456");
            profile.GetPassword("Ray").Returns("Hashed Password");
            hash.GetPassword("pw").Returns("Hashed Password");

            var isValid = authenticationService.Valid("Ray", "pw", "123456");
            Assert.IsTrue(isValid);
        }
    }
}