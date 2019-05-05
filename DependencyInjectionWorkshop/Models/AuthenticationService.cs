using System;
using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Repository;
using DependencyInjectionWorkshop.Services;

namespace DependencyInjectionWorkshop.Models
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IProfile _profile;
        private readonly IHash _hash;
        private readonly IOtp _otpService;
        //private readonly UseTimeQuotaDecorator _useTimeQuotaDecorator;

        public AuthenticationService(IProfile profile,
            IHash hash,
            IOtp otpService)
        {
            _profile = profile;
            _hash = hash;
            _otpService = otpService;
        }

        public bool Valid(string accountId, string password, string otp)
        {
            var dbPassword = _profile.GetPassword(accountId);

            var hashedPassword = _hash.GetPassword(password);

            var currentOtp = _otpService.GetCurrent(accountId);

            var isValid = dbPassword == hashedPassword && otp == currentOtp;

            return isValid;
        }
    }

    public class ValidFailedManyTimeException : Exception
    {
    }
}