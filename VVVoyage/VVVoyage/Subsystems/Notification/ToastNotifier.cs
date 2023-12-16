using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    internal class ToastNotifier : INotifier
    {
        public Task ShowNotificationAsync(string message, string title, string okText)
        {
            // TODO implement this using either the CommunityToolkit package or the MAUI essentials package.
            // Since we need CommunityToolkit anyway, this is probably the best option.
            throw new NotImplementedException();
        }
    }
}
