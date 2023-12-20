using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VVVoyage.Subsystems.Notification
{
    public interface INotifier
    {
        Task<bool> ShowNotificationAsync(string message, string title, string okText, string cancelText = "");
    }
}
