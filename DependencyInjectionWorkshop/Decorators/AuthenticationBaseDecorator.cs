using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop.Decorators
{
    public class AuthenticationBaseDecorator : IAuthenticationService
    {
        private readonly IAuthenticationService _authentication;

        public AuthenticationBaseDecorator(IAuthenticationService authentication)
        {
            _authentication = authentication;
        }

        public virtual bool Valid(string accountId, string password, string otp)
        {
            return _authentication.Valid(accountId, password, otp);
        }
    }
}