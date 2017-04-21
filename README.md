# Photon Turnbased Webhooks Sample

## Summary

This is the **Photon Turnbased Webhooks** sample using [Azure Websites](http://www.windowsazure.com/en-us/services/storage/) and [Blob Storage]() and [Table Storage](http://www.windowsazure.com/en-us/services/storage/) for persitence.


## Requirements

- [Photon Account for Turnbased](https://www.exitgames.com/en/Turnbased)
- Windows with IIS (Internet Information Service) [feature enabled](https://msdn.microsoft.com/en-GB/library/ms181052(v=vs.80).aspx)
- [Visual Studio 2017](https://www.visualstudio.com/downloads/)
- [ngrok](https://ngrok.com/) to forward requests to your PC


## Set this project up for free now! Check out all the possibilities of Microsoft Azure!

Sign up for your free trial month of Microsoft Azure now and get USD 200 / EURO 150 to spend on all(!) Azure services you like to try out – without any further obligation!  
You’ll get the full power from the Cloud and you can choose yourself, how to spend your balance!
 
For authentification purposes you’ll need a credit card to sign up.
Without a credit card request a free Microsoft Azure Pass simply by sending a short note to [azurenow@microsoft.com](azurenow@microsoft.com).
Use the Microsoft Azure Pass to sign up at [www.windowsazurepass.com](www.windowsazurepass.com) and you’ll also discover the cloud power of Microsoft Azure for free!


## Run it locally

- Open the sample running Visual Studio as administrator and build the project (admin privileges are required because a virtual directory is used).
- Azure Storage*, appsettings.json `"DataSource": "Azure"`
  - Select the Azure Storage > Access Keys (copy paste into the config) & Azure Notification Namespace > Notification Hub > Access Keys
  - set `"AzureBlobConnectionString": "{your connection string here}"`
  - set `"NotificationHubConnectionString": "{your connection string here}"`
- Start ngrok in a command shell: `ngrok http 80` and copy the url which forwards to 127.0.0.1:80.
- go to the [Photon Dashboard](https://www.exitgames.com/en/Turnbased/Dashboard), create an application and set in the Webhooks tab the BaseUrl value: `[url from ngrok]/turnbased/[your app id]/`.
- run the client demo
- check the requests and responses in your browser at [127.0.0.1:4040](http://127.0.0.1:4040)


## Next Steps ##

## Deploy to Azure
Hit this link to create all infrastructure required to run locally and in Azure.  
 
[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

Note - this will host the WebAPI in Azure as well as the infrastructure.

Created Infrastructure
*  Storage Account
*  App Service Plan
*  Web API
*  Notification Hub Namespace and Notification Hub
*  Application Insights

Deployment of the Azure Infrastructure will also update connection strings of created resources in the source code so no changes are required to get the sample running in azure; 
however connection strings will be required for Storage Account & Notification Hub, these are updated in `appsettings.json`

## Notification Hub

Azure Notification Hub requires clients to be registered with the [back-end service](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-aspnet-register-user-from-backend-to-push-notification),
as well as notifications for [specific client OS](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-aspnet-cross-platform-notification).
A template has been created for messages in `PushNotifications/HubMessage.cs` & a template for sending a message in `PushNotification/AzureHubNotification.csx`, implement your own logic here for sending push notifications to specific clients.
