using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    public class PopupNotifier : INotifier
    {
        public async Task ShowNotificationAsync(string message, string title, string okText)
        {
            await Shell.Current.DisplayAlert(title, message, okText);
        }
    }
}
