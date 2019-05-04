namespace DependencyInjectionWorkshop.Models
{
    public interface IAuthenticationService
    {
        bool Valid(string accountId, string password, string otp);
    }
}