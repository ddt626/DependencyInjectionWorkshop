using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop.Decorators
{
    public class NotificationDecorator : IAuthenticationService
    {
        private IAuthenticationService _authentication;
        private readonly INotification _notification;

        public NotificationDecorator(IAuthenticationService authentication, INotification notification)
        {
            _authentication = authentication;
            _notification = notification;
        }

        private void PushMessage(string accountId)
        {
            _notification.PushMessage($"account:{accountId} verify failed");
        }

        public bool Valid(string accountId, string password, string otp)
        {
            var isValid = _authentication.Valid(accountId, password, otp);
            if (!isValid)
            {
                PushMessage(accountId);
            }

            return isValid;
        }
    }
}