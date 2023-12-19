using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    public class PopupNotifier : INotifier
    {
        public async Task<bool> ShowNotificationAsync(string message, string title, string okText, string cancelText = "")
        {
            bool accepted = true;

            if (string.IsNullOrEmpty(cancelText))
            {
                await Shell.Current.DisplayAlert(title, message, okText);
            }
            else
            {
                accepted = await Shell.Current.DisplayAlert(title, message, okText, cancelText);
            }
            

            return accepted;
        }
    }
}
