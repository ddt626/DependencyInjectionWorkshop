using DependencyInjectionWorkshop.Adapter;
using DependencyInjectionWorkshop.Models;

namespace DependencyInjectionWorkshop.Decorators
{
    public class NotificationDecorator : AuthenticationBaseDecorator, IAuthenticationService
    {
        private readonly INotification _notification;

        public NotificationDecorator(IAuthenticationService authentication, INotification notification) : base(
            authentication)
        {
            _notification = notification;
        }

        private void PushMessage(string accountId)
        {
            _notification.PushMessage($"account:{accountId} verify failed");
        }

        public override bool Valid(string accountId, string password, string otp)
        {
            var isValid = base.Valid(accountId, password, otp);
            if (!isValid)
            {
                PushMessage(accountId);
            }

            return isValid;
        }
    }
}