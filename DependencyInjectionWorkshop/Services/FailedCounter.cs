using System;
using System.Net.Http;
using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop.Services
{
    public class FailedCounter
    {
        public void ResetFailedCounter(string accountId)
        {
            var httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.com/")};
            var resetResult = httpClient
                .PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResult.EnsureSuccessStatusCode();
        }

        public void AddFailedCount(string accountId)
        {
            var httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.com/")};
            var addFailedCountResponse = httpClient
                .PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
        }

        public int GetFailedCount(string accountId)
        {
            var httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.com/")};
            var getFailedCountResponse = httpClient
                .PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            getFailedCountResponse.EnsureSuccessStatusCode();
            var failedCount = getFailedCountResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        public void CheckAccountIsLock(string accountId)
        {
            var httpClient = new HttpClient() {BaseAddress = new Uri("http://joey.com/")};
            var isLockResponse = httpClient
                .PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
            if (isLockResponse.IsSuccessStatusCode)
            {
                var isLock = isLockResponse.Content.ReadAsAsync<bool>().Result;
                if (isLock)
                {
                    throw new ValidFailedManyTimeException();
                }
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }
        }
    }
}