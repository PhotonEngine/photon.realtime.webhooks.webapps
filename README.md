# Photon Realtime Webhooks Sample

## Summary

This is the **Photon Realtime Webhooks** sample using [Azure Websites](https://docs.microsoft.com/en-us/azure/app-service/app-service-how-works-readme?toc=%2fazure%2fapp-service-api%2ftoc.json) and [Blob Storage](http://www.windowsazure.com/en-us/services/storage/) and [Table Storage](http://www.windowsazure.com/en-us/services/storage/) for persitence.


## Requirements

- [Photon Account for Realtime](https://www.photonengine.com/en-US/Realtime)
- Windows with IIS (Internet Information Service) [feature enabled](https://msdn.microsoft.com/en-GB/library/ms181052(v=vs.80).aspx)
- [Visual Studio 2017](https://www.visualstudio.com/downloads/)
- [PostMan](https://www.getpostman.com/)


## Set this project up for free now! Check out all the possibilities of Microsoft Azure!

Sign up for your free trial month of Microsoft Azure now and get USD 200 / EURO 150 to spend on all(!) Azure services you like to try out – without any further obligation!  
You'll get the full power from the Cloud and you can choose yourself, how to spend your balance!
 
For authentication purposes you'll need a credit card to sign up.
Without a credit card request a free Microsoft Azure Pass simply by sending a short note to [azurenow@microsoft.com](azurenow@microsoft.com).
Use the Microsoft Azure Pass to sign up at [www.windowsazurepass.com](www.windowsazurepass.com) and you'll also discover the cloud power of Microsoft Azure for free!

## Deploy to Azure
Hit this link to create all infrastructure required to run locally and in Azure.  
 
[![Deploy to Azure](http://azuredeploy.net/deploybutton.png)](https://azuredeploy.net/)

Note - this will host the WebAPI in Azure as well as the infrastructure.

Created Infrastructure
*  [Storage Account](https://docs.microsoft.com/en-us/azure/storage/storage-introduction) - 

    Storage Accounts contain different ways to persist data in tables, queues or binary large objects (Blob). This project uses blob and table storage to store game event data 

*  [Web API App](https://docs.microsoft.com/en-us/azure/app-service/app-service-how-works-readme?toc=%2fazure%2fapp-service-api%2ftoc.json) -

    Azure is able to host websites and web api's in the cloud. This code will be automatically deployed to a web api app read for a Http request to be sent

*  [Notification Hub Namespace and Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/) -

    Notification Hub in Azure is like a queue, we can send a message and our client will consume that message, and respond accordingly

*  [Application Insights](https://azure.microsoft.com/en-gb/services/application-insights/) -
   
    Application Insights allows us to track numerous aspects of out project, such as execution time, errors, calls and many more, this can then be linked
    with BI services to get deep insight into performance and such from our code

## Run & develop it locally

- Open the sample running Visual Studio as administrator and build the project (admin privileges are required because a virtual directory is used).
- Login to [Portal](http://portal.azure.com) and go to your newly created resource group and the storage account within it
- Select the Azure Storage > Access Keys (copy paste into the config) & Azure Notification Namespace > Notification Hub > Access Keys
- set `"AzureBlobConnectionString": "{your connection string here}"`
- set `"NotificationHubConnectionString": "{your connection string here}"`

Deployment of the Azure Infrastructure will also update connection strings of created resources in the source code so no changes are required to get the sample running in azure; 
however connection strings will be required for Storage Account & Notification Hub, these are updated in `appsettings.json`

## Notification Hub
Azure Notification Hub requires clients to be registered with the [back-end service](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-ios-aspnet-register-user-from-backend-to-push-notification),
as well as notifications for [specific client OS](https://docs.microsoft.com/en-us/azure/notification-hubs/notification-hubs-aspnet-cross-platform-notification).
A template has been created for messages in `PushNotifications/HubMessage.cs` & a template for sending a message in `PushNotification/AzureHubNotification.csx`, implement your own logic here for sending push notifications to specific clients.

## Link with Photon Realtime
Create your Photon Realtime account login and create a new app. Use the Url from your hosted Azure Web Api e.g. `https://photoncloud.azurewebsites.net`.
Hit Manage -> Create Web Hook, set your Url and Paths for the controllers and hit apply.
Your Photon Cloud Realtime account is now pointing to your Azure Web Api