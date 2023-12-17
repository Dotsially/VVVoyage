using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    internal class PushNotifier : INotifier
    {
        public async Task ShowNotificationAsync(string message, string title, string okText)
        {
            NotificationRequest notification = new()
            {
                // Apparently this package requires the dev to insert a notification ID, so try a random number.
                NotificationId = new Random().Next(700, 9000),
                Title = title,
                Description = message
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
    }
}
