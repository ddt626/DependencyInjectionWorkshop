namespace DependencyInjectionWorkshop.Models
{
    public interface IUseTimeQuota
    {
        void Add(string accountId, IAuthenticationService authenticationService);
    }
}