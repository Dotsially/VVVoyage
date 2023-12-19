using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    public class PushNotifier : INotifier
    {
        public async Task<bool> ShowNotificationAsync(string message, string title, string okText = "", string cancelText = "")
        {
            NotificationRequest notification = new()
            {
                // Apparently this package requires the dev to insert a notification ID, so try a random number.
                NotificationId = new Random().Next(700, 9000),
                Title = title,
                CategoryType = NotificationCategoryType.Status,
                Description = message
            };
            await LocalNotificationCenter.Current.Show(notification);

            return true;
        }
    }
}
