using System.Collections.Generic;
using System.Threading.Tasks;

namespace Photon.Webhooks.Turnbased.PushNotifications
{
    public interface INotification
    {
        Task SendMessage(Dictionary<string, string> notificationContent, string username, string usertag, string target,
            string appid);
    }
}