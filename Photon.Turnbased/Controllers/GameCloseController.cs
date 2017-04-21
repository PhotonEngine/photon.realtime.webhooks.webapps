// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameCloseController.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photon.Webhooks.Turnbased.DataAccess;

namespace Photon.Webhooks.Turnbased.Controllers
{
    using Newtonsoft.Json;
    using Models;


    public class GameListItem
    {
        public int ActorNr;
        public dynamic Properties;
    }

    public class GameCloseController : Controller
    {
        private readonly ILogger<GameCloseController> _logger;
        private readonly IDataAccess _dataAccess;

        #region Public Methods and Operators

        public GameCloseController(ILogger<GameCloseController> logger, DataSources dataSources)
        {
            _logger = logger;
            _dataAccess = dataSources.DataAccess;
        }

        [HttpPost]
        public IActionResult Index([FromBody] GameCloseRequest request, string appId)
        {

            if (!IsValid(request, out string message))
            {
                var errorResponse = new ErrorResponse { Message = message };
                _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                return BadRequest(errorResponse);
            }

            if (request.State == null)
            {
                if (request.ActorCount > 0)
                {
                    var errorResponse = new ErrorResponse { Message = "Missing State." };
                    _logger.LogError($"{Request.GetUri()} - {JsonConvert.SerializeObject(errorResponse)}");
                    return BadRequest(errorResponse);
                }

                _dataAccess.StateDelete(appId, request.GameId);

                var okResponse = new OkResponse();
                _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(okResponse)}");
                return Ok(okResponse);
            }

            foreach (var actor in request.State.ActorList)
            {
                //var listProperties = new ListProperties() { ActorNr = (int)actor.ActorNr, Properties = request.State.CustomProperties };
                //DataSources.DataAccess.GameInsert(appId, (string)actor.UserId, request.GameId, (string)JsonConvert.SerializeObject(listProperties));
                _dataAccess.GameInsert(appId, (string)actor.UserId, request.GameId, (int)actor.ActorNr);
            }                

            //deprecated
            if (request.State2 != null)
            {
                foreach (var actor in request.State2.ActorList)
                {
                    _dataAccess.GameInsert(appId, (string)actor.UserId, request.GameId, (int)actor.ActorNr);
                }
            }

            var state = (string)JsonConvert.SerializeObject(request.State);
            _dataAccess.StateSet(appId, request.GameId, state);

            var response = new OkResponse();
            _logger.LogInformation($"{Request.GetUri()} - {JsonConvert.SerializeObject(response)}");
            return Ok(response);
        }

        private static bool IsValid(GameCloseRequest request, out string message)
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