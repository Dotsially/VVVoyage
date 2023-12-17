using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    internal interface INotifier
    {
        Task ShowNotificationAsync(string message, string title, string okText);
    }
}
