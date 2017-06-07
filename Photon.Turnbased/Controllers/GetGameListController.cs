// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetGameListController.cs" company="Exit Games GmbH">
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

    public class GetGameListController : Controller
    {
        private readonly ILogger<GetGameListController> _logger;
        private readonly IDataAccess _dataAccess;

        #region Public Methods and Operators

        public GetGameListController(ILogger<GetGameListController> logger, DataSources dataSources)
        {
            _logger = logger;
            _dataAccess = dataSources.DataAccess;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GetGameListRequest request, string appId)
        {
            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return Ok(errorResponse);
            }

            appId = appId.ToLowerInvariant();

            var list = new Dictionary<string, object>();

            foreach (var pair in _dataAccess.GameGetAll(appId, request.UserId))
            {
                // exists - save result in list
                //if (DataSources.DataAccess.StateExists(appId, pair.Key))
                var stateJson = _dataAccess.StateGet(appId, pair.Key);
                if (stateJson != null)
                {
                    dynamic customProperties = null;
                    if (stateJson != string.Empty)
                    {
                        var state = JsonConvert.DeserializeObject<dynamic>(stateJson);
                        customProperties = state.CustomProperties;
                    }

                    var gameListItem = new GameListItem { ActorNr = int.Parse(pair.Value), Properties = customProperties };

                    list.Add(pair.Key, gameListItem);
                }
                // not exists - delete
                else
                {
                    _dataAccess.GameDelete(appId, request.UserId, pair.Key);
                }
            }

            var getGameListResponse = new GetGameListResponse { Data = list };
            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(getGameListResponse)}");
            return Ok(getGameListResponse);
        }

        private static bool IsValid(GetGameListRequest request, out string message)
        {
            if (request == null)
            {
                message = "Received request does not contain expected JSON data.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                message = "Missing \"UserId\" parameter.";
                return false;
            }

            message = string.Empty;
            return true;
        }
        #endregion
    }
}