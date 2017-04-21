// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GamePropertiesController.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photon.Webhooks.Turnbased.DataAccess;

namespace Photon.Webhooks.Turnbased.Controllers
{
    using System.Collections.Generic;
    using Models;
    using Newtonsoft.Json;
    using PushNotifications;

    public class GamePropertiesController : Controller
    {
        private readonly ILogger<GamePropertiesController> _logger;
        private readonly IDataAccess _dataAccess;
        private readonly INotification _notification;

        #region Public Methods and Operators

        public GamePropertiesController(ILogger<GamePropertiesController> logger, DataSources dataSources, INotification notification)
        {
            _logger = logger;
            _dataAccess = dataSources.DataAccess;
            _notification = notification;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GamePropertiesRequest request, string appId)
        {
            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return BadRequest(errorResponse);
            }

            if (request.State != null)
            {
                var state = (string)JsonConvert.SerializeObject(request.State);
                _dataAccess.StateSet(appId, request.GameId, state);

                var properties = request.Properties;
                object actorNrNext = null;
                properties?.TryGetValue("turn", out actorNrNext);
                var userNextInTurn = string.Empty;
                foreach (var actor in request.State.ActorList)
                {
                    if (actorNrNext != null)
                    {
                        if (actor.ActorNr == actorNrNext)
                        {
                            userNextInTurn = (string)actor.UserId;
                        }
                    }
                    _dataAccess.GameInsert(appId, (string)actor.UserId, request.GameId, (int)actor.ActorNr);
                }

                if (!string.IsNullOrEmpty(userNextInTurn))
                {
                    var notificationContent = new Dictionary<string, string>
                                                  {
                                                      { "en", "{USERNAME} finished. It's your turn." },
                                                      { "de", "{USERNAME} hat seinen Zug gemacht. Du bist dran." },
                                                  };
                    _notification.SendMessage(notificationContent, request.Username, "UID2", userNextInTurn, appId);
                }
            }

            var response = new OkResponse();
            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }


        private static bool IsValid(GamePropertiesRequest request, out string message)
        {
            if (string.IsNullOrEmpty(request.GameId))
            {
                message = "Missing GameId.";
                return false;
            }

            message = "";
            return true;
        }

        #endregion
    }
}