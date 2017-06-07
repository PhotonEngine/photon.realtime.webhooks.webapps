// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameLeaveController.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photon.Webhooks.Turnbased.DataAccess;

namespace Photon.Webhooks.Turnbased.Controllers
{
    using Models;
    using Newtonsoft.Json;

    public class GameLeaveController : Controller
    {
        private readonly ILogger<GameLeaveController> _logger;
        private readonly IDataAccess _dataAccess;

        #region Public Methods and Operators
        public GameLeaveController(ILogger<GameLeaveController> logger, DataSources dataSources)
        {
            _logger = logger;
            _dataAccess = dataSources.DataAccess;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GameLeaveRequest request, string appId)
        {
            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return Ok(errorResponse);
            }

            appId = appId.ToLowerInvariant();

            // TODO: update this check and maybe move game insertion to GameJoin controller
            if (request.IsInactive)
            {
                _dataAccess.GameInsert(appId, request.UserId, request.GameId, request.ActorNr);
            }
            else
            {
                _dataAccess.GameDelete(appId, request.UserId, request.GameId);
            }

            var okResponse = new OkResponse();
            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(okResponse)}");
            return Ok(okResponse);
        }

        private static bool IsValid(GameLeaveRequest request, out string message)
        {
            if (request == null)
            {
                message = "Received request does not contain expected JSON data.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.GameId))
            {
                message = "Missing \"GameId\" parameter.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.UserId))
            {
                message = "Missing \"UserId\" parameter.";
                return false;
            }

            if (request.ActorNr <= 0)
            {
                message = $"Unexpected \"ActorNr\" value: {request.ActorNr}.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        #endregion
    }
}