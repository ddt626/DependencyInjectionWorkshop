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

            var dbPassword = string.Empty;
            using (var connection = new SqlConnection("my connection string"))
            {
                var password1 = connection.Query<string>("spGetUserPassword", new { Id = accountId },
                    commandType: CommandType.StoredProcedure).SingleOrDefault();

                dbPassword = password1;
            }

            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            foreach (var theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }

            var hashedPassword = hash.ToString();

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

            if (dbPassword == hashedPassword && otp == currentOtp)
            {
                var resetResult = httpClient.PostAsJsonAsync("api/failedCounter/Reset", accountId).Result;
                resetResult.EnsureSuccessStatusCode();

                return true;
            }
            else
            {
                var addFailedCountResponse = httpClient.PostAsJsonAsync("api/failedCounter/Add", accountId).Result;
                addFailedCountResponse.EnsureSuccessStatusCode();

                var slackClient = new SlackClient("my api token");
                var message = $"account:{accountId} verify failed";
                slackClient.PostMessage(resp => { }, "my channel", message, "my bot name");

                return false;
            }
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}