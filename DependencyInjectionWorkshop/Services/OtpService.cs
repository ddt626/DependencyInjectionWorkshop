using System;
using System.Net.Http;

namespace DependencyInjectionWorkshop.Services
{
    public interface IOtp
    {
        string GetCurrent(string accountId);
    }

    public class OtpService : IOtp
    {
        public string GetCurrent(string accountId)
        {
            var currentOtp = string.Empty;
            var response = new HttpClient() {BaseAddress = new Uri("http://joey.com/")}
                .PostAsJsonAsync("api/otps", accountId).Result;
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
    }
}