using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using Dapper;
using SlackAPI;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService
    {
        public bool Valid(string accountId, string password, string otp)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri("http://joey.com/") };
            CheckAcocuntIsLock(accountId, httpClient);

            var dbPassword = GetDbPassword(accountId);

            var hashedPassword = GetHashedPassword(password);

            var currentOtp = GetCurrentOtp(accountId, httpClient);

            if (dbPassword == hashedPassword && otp == currentOtp)
            {
                ResetFailedCounter(accountId, httpClient);

                return true;
            }
            else
            {
                AddFailedCount(accountId, httpClient);


                var failedCount = GetFailedCount(accountId, httpClient);

                Notify($"account:{accountId} verify failed");

                Log($"Account: {accountId}, valid Failed {failedCount} times.");

                return false;
            }
        }

        private static void Log(string message)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(message);
        }

        private static void Notify(string message)
        {
            var slackClient = new SlackClient("my api token");
            slackClient.PostMessage(resp => { }, "my channel", message, "my bot name");
        }

        private static int GetFailedCount(string accountId, HttpClient httpClient)
        {
            var getFailedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/GetFailedCount", accountId).Result;
            getFailedCountResponse.EnsureSuccessStatusCode();
            var failedCount = getFailedCountResponse.Content.ReadAsAsync<int>().Result;
            return failedCount;
        }

        private static void AddFailedCount(string accountId, HttpClient httpClient)
        {
            var addFailedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
            addFailedCountResponse.EnsureSuccessStatusCode();
        }

        private static void ResetFailedCounter(string accountId, HttpClient httpClient)
        {
            var resetResult = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
            resetResult.EnsureSuccessStatusCode();
        }

        private static string GetCurrentOtp(string accountId, HttpClient httpClient)
        {
            var currentOtp = string.Empty;
            var response = httpClient.PostAsJsonAsync("api/otps", accountId).Result;
            if (response.IsSuccessStatusCode)
            {
                currentOtp = response.Content.ReadAsAsync<string>().Result;
            }
            else
            {
                throw new Exception($"web api error, accountId:{accountId}");
            }

            return currentOtp;
        }

        private static string GetHashedPassword(string password)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            var hashedPassword = hash.ToString();
            return hashedPassword;
        }

        private static string GetDbPassword(string accountId)
        {
            var dbPassword = string.Empty;
            using (var connection = new SqlConnection("my connection string"))
            {
                var password1 = connection.Query<string>("spGetUserPassword", new {Id = accountId},
                    commandType: CommandType.StoredProcedure).SingleOrDefault();

                dbPassword = password1;
            }

            return dbPassword;
        }

        private static void CheckAcocuntIsLock(string accountId, HttpClient httpClient)
        {
            var isLockResponse = httpClient.PostAsJsonAsync("api/failedCounter/IsLock", accountId).Result;
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

    public class ValidFailedManyTimeException : Exception
    {
    }
}