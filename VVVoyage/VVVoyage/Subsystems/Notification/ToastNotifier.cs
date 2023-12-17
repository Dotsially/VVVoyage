using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    internal class ToastNotifier : INotifier
    {
        public async Task ShowNotificationAsync(string message, string title, string okText)
        {
            var toast = Toast.Make(message, ToastDuration.Short);
            await toast.Show();
        }
    }
}
