using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Photon.Webhooks.Turnbased.Config;
using static Photon.Webhooks.Turnbased.PushNotifications.HubMessage;
using Microsoft.Azure.NotificationHubs;

namespace Photon.Webhooks.Turnbased.PushNotifications
{
    public class AzureHubNotification : INotification
    {
        private readonly ILogger<AzureHubNotification> _logger;
        private readonly ConnectionStrings _connectionStrings;
        private NotificationHubClient hub = null;
        //private readonly TopicDescription td = new TopicDescription("turnbasednotify");

        public AzureHubNotification(ILogger<AzureHubNotification> logger, IOptions<ConnectionStrings> connectionStrings)
        {
            _logger = logger;
            _connectionStrings = connectionStrings.Value;
            hub = NotificationHubClient.CreateClientFromConnectionString(connectionStrings.Value.NotificationHubConnectionString, "notifyhub"); //todo: move to config
        }

        public async Task SendMessage(Dictionary<string, string> notificationContent, string username, string usertag, string target, string appid)
        {
            var template = WrapMessage(notificationContent, username, usertag, target, appid);
            //implement logic to send notification here
        }
    }
}
