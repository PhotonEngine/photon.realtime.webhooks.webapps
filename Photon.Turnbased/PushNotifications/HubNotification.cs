using System.Collections.Generic;

namespace Photon.Webhooks.Turnbased.PushNotifications
{

    public class HubNotification
    {
        public string SendDate { get; set; }
        public bool IgnoreUserTimezone { get; set; }
        public Dictionary<string, string> Content { get; set; }
    }
}
