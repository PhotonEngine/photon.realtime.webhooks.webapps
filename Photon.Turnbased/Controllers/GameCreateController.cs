// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameCreateController.cs" company="Exit Games GmbH">
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

    public class GameCreateController : Controller
    {
        private readonly ILogger<GameCreateController> _logger;
        private readonly IDataAccess _dataAccess;

        #region Public Methods and Operators

        public GameCreateController(ILogger<GameCreateController> logger, DataSources dataSources)
        {
            _logger = logger;
            _dataAccess = dataSources.DataAccess;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GameCreateRequest request, string appId)
        {

            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return Ok(errorResponse);
            }

            appId = appId.ToLowerInvariant();

            dynamic response;
            if ("Load".Equals(request.Type))
            {
                response = GameLoad(request, appId);
            }
            else if ("Create".Equals(request.Type))
            {
                response = GameCreate(request, appId);
            }
            else
            {
                var errorResponse = new ErrorResponse { Message = $"Unexpected \"Type\" parameter value: \"{request.Type}\"." };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return Ok(errorResponse);
            }

            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        private dynamic GameCreate(GameCreateRequest request, string appId)
        {
            dynamic response;
            if (_dataAccess.StateExists(appId, request.GameId))
            {
                response = new ErrorResponse { Message = $"Game with ID=\"{request.GameId}\" already exist." };
                return response;
            }

            if (request.CreateOptions == null)
            {
                _dataAccess.StateSet(appId, request.GameId, string.Empty);
            }
            else
            {
                _dataAccess.StateSet(appId, request.GameId, JsonConvert.SerializeObject(request.CreateOptions));
            }

            response = new OkResponse();
            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public dynamic GameLoad(GameCreateRequest request, string appId)
        {
            dynamic response;
            var stateJson = _dataAccess.StateGet(appId, request.GameId);

            if (!string.IsNullOrEmpty(stateJson))
            {
                response = new GameLoadResponse { State = JsonConvert.DeserializeObject(stateJson) };
                return response;
            }
            //TBD - check how deleteIfEmpty works with createifnot exists
            if (stateJson == string.Empty)
            {
                _dataAccess.StateDelete(appId, request.GameId);

                _logger.LogInformation($"Deleted empty state, app id {appId}, gameId {request.GameId}");

            }

            if (request.CreateIfNotExists)
            {
                response = new OkResponse();
                _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
                return response;
            }

            response = new ErrorResponse { Message = $"Game with ID=\"{request.GameId}\" not found." };
            _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return response;
        }

        private static bool IsValid(GameCreateRequest request, out string message)
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

            if (string.IsNullOrWhiteSpace(request.Type))
            {
                message = "Missing \"Type\" parameter.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        #endregion
    }
}