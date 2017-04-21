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
        //TODO: Class contains static singleton methods, means that logger can't be used in methods. 
        //TODO: Maybe just return the message and log or change to no static? Will need to change caller as well

        private readonly ILogger<GameCreateController> _logger;
        //TODO: cleaner way of using IDataAccess
        //Don't really like this, but it allows a static variable which this class wants for the static methods
        internal static  IDataAccess DataAccess { get; private set; }

        #region Public Methods and Operators

        public GameCreateController(ILogger<GameCreateController> logger, DataSources dataSources)
        {
            _logger = logger;
            DataAccess = dataSources.DataAccess;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GameCreateRequest request, string appId)
        {

            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return BadRequest(errorResponse);
            }

            dynamic response;
            if (!string.IsNullOrEmpty(request.Type) && request.Type == "Load")
            {
                response = GameLoad(request, appId);
            }
            else
            {
                response = GameCreate(request, appId);
            }

            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        private dynamic GameCreate(GameCreateRequest request, string appId)
        {
            dynamic response;
            if (DataAccess.StateExists(appId, request.GameId))
            {
                response = new ErrorResponse { Message = "Game already exists." };
                return response;
            }

            if (request.CreateOptions == null)
            {
                DataAccess.StateSet(appId, request.GameId, string.Empty);
            }
            else
            {
                DataAccess.StateSet(appId, request.GameId, (string)JsonConvert.SerializeObject(request.CreateOptions));
            }

            response = new OkResponse();
            //_logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public static dynamic GameLoad(GameCreateRequest request, string appId)
        {
            dynamic response;
            var stateJson = DataAccess.StateGet(appId, request.GameId);

            if (!string.IsNullOrEmpty(stateJson))
            {
                response = new GameLoadResponse { State = JsonConvert.DeserializeObject(stateJson) };
                return response;
            }
            //TBD - check how deleteIfEmpty works with createifnot exists
            if (stateJson == string.Empty)
            {
                DataAccess.StateDelete(appId, request.GameId);
                
                //_logger.LogInformation($"Deleted empty state, app id {appId}, gameId {request.GameId}");
        
            }

            if (request.CreateIfNotExists)
            {
                response = new OkResponse();
                //_logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
                return response;
            }

            response = new ErrorResponse { Message = "GameId not Found." };
            //_logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return response;
        }
        private static bool IsValid(GameCreateRequest request, out string message)
        {
            if (string.IsNullOrEmpty(request.GameId))
            {
                message = "Missing GameId.";
                return false;
            }

            if (string.IsNullOrEmpty(request.UserId))
            {
                message = "Missing UserId.";
                return false;
            }

            message = "";
            return true;
        }

        #endregion
    }
}