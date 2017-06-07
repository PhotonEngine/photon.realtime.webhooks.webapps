// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace Photon.Webhooks.Turnbased.Models
{
    using System.Collections.Generic;
    using System.ComponentModel;

    #region Requests

    public class BaseRequest
    {
        #region Public Properties

        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string Region { get; set; }

        #endregion
    }

    public abstract class BaseWebhooksRequest : BaseRequest
    {
        #region Public Properties

        public string Cloud { get; set; }

        public string GameId { get; set; }

        public string Type { get; set; }

        #endregion
    }

    public abstract class BaseWebhooksUserRequest : BaseWebhooksRequest
    {
        #region Public Properties

        [DefaultValue(-1)]
        public int ActorNr { get; set; }

        public string UserId { get; set; }

        public string Nickname { get; set; }

        #endregion
    }

    public abstract class BaseWebRpcRequest : BaseRequest
    {
        #region Public Properties
        
        public string UserId { get; set; }

        public dynamic AuthCookie { get; set; }

        public dynamic RpcParams { get; set; }

        #endregion
    }

    public class GameCloseRequest : BaseWebhooksRequest
    {
        #region Public Properties

        [DefaultValue(-1)]
        public int ActorCount { get; set; }

        /// <summary>
        /// the current game state, returned again with load game
        /// </summary>
        public dynamic State { get; set; }

        #endregion
    }

    public class GameCreateRequest : BaseWebhooksUserRequest
    {
        #region Public Properties

        public dynamic CreateOptions { get; set; }

        public bool CreateIfNotExists { get; set; }

        #endregion
    }

    public class GameJoinRequest : BaseWebhooksUserRequest
    {
        #region Public Properties

        #endregion
    }

    public class GameLeaveRequest : BaseWebhooksUserRequest
    {
        #region Public Properties

        /// <summary> Refers to the state of the actor before leaving the Room. If set to true then the actor can rejoin the game. 
        /// If set to false, then the actor left for good and is removed from ActorList and can't rejoin the game. </summary>
        public bool IsInactive { get; set; }

        public LeaveReason Reason { get; set; }

        public dynamic AuthCookie { get; set; }

        #endregion

        public enum LeaveReason
        {
            /// <summary> Indicates that the client called Disconnect() </summary>
            ClientDisconnect = 0,
            /// <summary> Indicates that client has timed-out server. This is valid only when using UDP/ENET. </summary>
            ClientTimeoutDisconnect = 1,
            /// <summary> Indicates client is too slow to handle data sent. </summary>
            ManagedDisconnect = 2,
            /// <summary> Indicates low level protocol error which can be caused by data corruption. </summary>
            ServerDisconnect = 3,
            /// <summary> Indicates that the server has timed-out client. </summary>
            TimeoutDisconnect = 4,
            /// <summary> Indicates that the client called OpLeave(). </summary>
            LeaveRequest = 101,
            /// <summary> Indicates that the inactive actor timed-out, meaning the PlayerTtL of the room expired for that actor. </summary>
            PlayerTtlTimedOut = 102,
            /// <summary> Indicates a very unusual scenario where the actor did not send anything to Photon Servers for 5 minutes. 
            /// Normally peers timeout long before that but Photon does a check for every connected peer's timestamp of 
            /// the last exchange with the servers (called LastTouch) every 5 minutes. </summary>
            PeerLastTouchTimedout = 103,
            /// <summary> Indicates that the actor was removed from ActorList by a plugin. </summary>
            PluginRequest = 104,
            /// <summary> Indicates an internal error in a plugin implementation. </summary>
            PluginFailedJoin = 105
        }

    }

    [Obsolete("This is no longer used.")]
    public class GameLoadRequest
    {
        #region Public Properties

        [DefaultValue(-1)]
        public int ActorNr { get; set; }

        public string GameId { get; set; }

        public string UserId { get; set; }

        #endregion
    }

    public class GamePropertiesRequest : BaseWebhooksUserRequest
    {
        #region Public Properties

        public dynamic State { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public dynamic AuthCookie { get; set; }

        #endregion
    }

    public class GameEventRequest : BaseWebhooksUserRequest
    {
        #region Public Properties

        public byte EvCode { get; set; }

        public dynamic Data { get; set; }

        public dynamic State { get; set; }
        
        public dynamic AuthCookie { get; set; }

        #endregion
    }

    public class GetGameListRequest : BaseWebRpcRequest
    {
        #region Public Properties

        #endregion
    }

    #endregion

    #region Responses

    public class ErrorResponse
    {
        #region Public Properties

        public string Message { get; set; }

        public int ResultCode => (int)Models.ResultCode.Failed;

        #endregion
    }

    public class GameLoadResponse
    {
        #region Public Properties

        public int ResultCode => (int)Models.ResultCode.Ok;

        /// <summary>
        /// the game state as saved at game close 
        /// </summary>
        public dynamic State { get; set; }

        #endregion
    }

    public class GetGameListResponse
    {
        #region Public Properties

        /// <summary>
        /// the list of open games for this user, containing key/value pairs with game name/actor number
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        public int ResultCode => (int)Models.ResultCode.Ok;

        #endregion
    }

    public class OkResponse
    {
        #region Public Properties

        public int ResultCode => (int)Models.ResultCode.Ok;

        #endregion
    }

    #endregion

    public enum ResultCode
    {
        Ok = 0,
        Failed = 1,
    }
}