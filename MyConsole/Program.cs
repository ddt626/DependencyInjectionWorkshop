using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Decorators;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace MyConsole
{
    internal class AuthenticationFactory
    {
        static AuthenticationFactory()
        {
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            IProfile profile = new FakeProfile();
            IHash hash = new FakeHash();
            IOtp otp = new FakeOtp();
            INotification notification = new FakeSlack();
            IFailedCounter failedCounter = new FakeFailedCounter();
            ILogger logger = new FakeLogger();

            var authenticationService = new AuthenticationService(profile, hash, otp);
            var notificationDecorator = new NotificationDecorator(authenticationService, notification);

            var failedCounterDecorator = new FailedCounterDecorator(notificationDecorator, failedCounter);
            var logDecorator = new LogDecorator(failedCounterDecorator, failedCounter, logger);
            IAuthenticationService authentication = logDecorator;
            var isValid = authentication.Valid("ray", "pw", "123456");
            Console.WriteLine($"Result: {isValid}");
        }
    }

    internal class FakeLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine($"Fake log: {message}");
        }
    }

    internal class FakeSlack : INotification
    {
        public void PushMessage(string message)
        {
            Console.WriteLine($"{nameof(FakeSlack)}.{nameof(PushMessage)}({message})");
        }
    }

    internal class FakeFailedCounter : IFailedCounter
    {
        public void Reset(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Reset)}({accountId})");
        }

        public void Add(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Add)}({accountId})");
        }

        public int Get(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(Get)}({accountId})");
            return 91;
        }

        public bool CheckAccountIsLock(string accountId)
        {
            Console.WriteLine($"{nameof(FakeFailedCounter)}.{nameof(CheckAccountIsLock)}({accountId})");
            return false;
        }
    }

    internal class FakeOtp : IOtp
    {
        public string GetCurrent(string accountId)
        {
            Console.WriteLine($"{nameof(FakeOtp)}.{nameof(GetCurrent)}({accountId})");
            return "123456";
        }
    }

    internal class FakeHash : IHash
    {
        public string GetPassword(string plainText)
        {
            Console.WriteLine($"{nameof(FakeHash)}.{nameof(GetPassword)}({plainText})");
            return "my hashed password wrong";
        }
    }

    internal class FakeProfile : IProfile
    {
        public string GetPassword(string accountId)
        {
            Console.WriteLine($"{nameof(FakeProfile)}.{nameof(GetPassword)}({accountId})");
            return "my hashed password";
        }
    }
}