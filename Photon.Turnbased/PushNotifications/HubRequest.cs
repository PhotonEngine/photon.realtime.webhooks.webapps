using System.Collections.Generic;

namespace Photon.Webhooks.Turnbased.PushNotifications
{

    public class HubRequest
    {
        public List<HubNotification> Notifications { get; set; }
        public List<IList<string>> Conditions { get; set; }
    }
}
