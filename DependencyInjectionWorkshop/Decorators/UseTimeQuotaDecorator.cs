using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop.Decorators
{
    public class UseTimeQuotaDecorator : AuthenticationBaseDecorator
    {
        private readonly IAuthenticationService _authentication;
        private readonly IUseTimeQuota _useTimeQuota;

        public UseTimeQuotaDecorator(IAuthenticationService authentication, IUseTimeQuota useTimeQuota) : base(authentication)
        {
            _authentication = authentication;
            _useTimeQuota = useTimeQuota;
        }

        private void UseApiTimes(string accountId)
        {
            _useTimeQuota.Add(accountId, _authentication);
        }

        public override bool Valid(string accountId, string password, string otp)
        {
            var isValid = base.Valid(accountId, password, otp);
            UseApiTimes(accountId);
            return isValid;
        }
    }
}