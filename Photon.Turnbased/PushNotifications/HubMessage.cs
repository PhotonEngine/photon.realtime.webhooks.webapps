using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Photon.Webhooks.Turnbased.PushNotifications
{
    public static class HubMessage
    {
        public static string WrapMessage(Dictionary<string, string> notificationContent, string username, string usertag,
            string target, string appid)
        {
            var conditions = new List<IList<string>>
            {
                new[] {usertag, "EQ", target},
                new[] {"PhotonAppId", "EQ", appid}
            };

            var content = new Dictionary<string, string>();
            foreach (var item in notificationContent)
            {
                content[item.Key] = item.Value.Replace("{USERNAME}", username);
            }

            var notifications = new List<HubNotification>
            {
                new HubNotification
                {
                    SendDate = DateTime.UtcNow.ToShortTimeString(),
                    IgnoreUserTimezone = true,
                    Content = content,
                }
            };
            var request = new Dictionary<string, HubRequest>
            {
                {
                    "request",
                    new HubRequest
                    {
                        Notifications = notifications,
                        Conditions = conditions,
                    }
                }
            };
            return JsonConvert.SerializeObject(request);
        }
    }
}
