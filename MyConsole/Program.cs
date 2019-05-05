using System;
using Autofac;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Decorators;
using DependencyInjectionWorkshop.Models;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace MyConsole
{
    internal class Program
    {
        private static IContainer _container;

        private static void Main(string[] args)
        {
            RegisterContain();
            IAuthenticationService authentication = _container.Resolve<IAuthenticationService>();
            var isValid = authentication.Valid("ray", "pw", "123456");
            Console.WriteLine($"Result: {isValid}");
        }

        private static void RegisterContain()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<FakeProfile>().As<IProfile>();
            containerBuilder.RegisterType<FakeHash>().As<IHash>();
            containerBuilder.RegisterType<FakeOtp>().As<IOtp>();
            containerBuilder.RegisterType<FakeSlack>().As<INotification>();
            containerBuilder.RegisterType<FakeFailedCounter>().As<IFailedCounter>();
            containerBuilder.RegisterType<FakeLogger>().As<ILogger>();

            containerBuilder.RegisterType<NotificationDecorator>();
            containerBuilder.RegisterType<FailedCounterDecorator>();
            containerBuilder.RegisterType<LogDecorator>();

            containerBuilder.RegisterType<AuthenticationService>().As<IAuthenticationService>();
            containerBuilder.RegisterDecorator<NotificationDecorator, IAuthenticationService>();
            containerBuilder.RegisterDecorator<FailedCounterDecorator, IAuthenticationService>();
            containerBuilder.RegisterDecorator<LogDecorator, IAuthenticationService>();

            _container = containerBuilder.Build();
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