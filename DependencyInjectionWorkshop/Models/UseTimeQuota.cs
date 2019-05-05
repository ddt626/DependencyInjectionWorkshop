using System;

namespace DependencyInjectionWorkshop.Models
{
    public class UseTimeQuota : IUseTimeQuota
    {
        public void Add(string accountId, IAuthenticationService authenticationService)
        {
            Console.WriteLine($"use time add: {accountId}");
        }
    }
}