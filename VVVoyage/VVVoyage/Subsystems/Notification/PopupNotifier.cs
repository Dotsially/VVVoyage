using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    internal class PopupNotifier : INotifier
    {
        public async Task ShowNotificationAsync(string message, string title, string okText)
        {
            await Shell.Current.DisplayAlert(message, title, okText);
        }
    }
}
