namespace ARVI.SDK
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using UnityEngine;

    using Debug = UnityEngine.Debug;

    public enum AudioChatChannel : int
    {
        Public = 0,
        Channel1 = 1,
        Channel2 = 2,
        Channel3 = 3,
        Channel4 = 4,
        Channel5 = 5,
        Channel6 = 6,
        Channel7 = 7,
        Channel8 = 8,
        Channel9 = 9,
        Channel10 = 10
    }

    public enum ErrorCode : int
    {
        Unknown = -1,
        BadResponseID = -2,
        None = 0,
        ApiKeyError = 1,
        AuthDataError = 2,
        AuthenticationError = 3,
        RequestError = 4
    }

    internal static class Errors
    {
        internal const string EMPTY_APPKEY = "Application Key is empty. You can configure it in ARVI->Integration->Settings menu.";
        internal const string INTEGRATION_NOT_INITIALIZED = "Integration API not initialized. Was Integration.Initialize() called?";
        internal const string PLUGIN_NOT_FOUND = "Plugin not found.";
        internal const string PLATFORM_MESSAGES_NOT_INITIALIZED = "Platform Message module not initialized.";
    }

    public static class Integration
    {
        private const int SDK_INTEGRATION_VERSION = 1;

        public delegate void PlatformMessageReceivedHandler(PlatformMessage message);
        public static event PlatformMessageReceivedHandler PlatformMessageReceived;

        public delegate void PlatformTimeLeftRequestHandler(out int seconds);
        public static event PlatformTimeLeftRequestHandler TimeLeftRequest;

        public delegate void PlatformPlayerPositionRequestHandler(out Vector3 playerPosition, out Vector3 playerForward, out Vector3 playerUp);
        public static event PlatformPlayerPositionRequestHandler PlayerPositionRequest;

        /// <value>Gets if the integration API initialized and ready to use</value>
        public static bool Initialized { get; private set; }

        /// <value>Application Key, provided with API and used for entitlement checks</value>
        public static string AppKey { get; private set; }

        /// <value>Gets if the application is running in trial mode</value>
        public static bool IsApplicationInTrialMode { get; private set; }

        /// <value>Gets if the game should track cord twisting</value>
        public static bool ShouldApplicationTrackCordTwisting { get; private set; }

        /// <value>SDK version</value>
        public static string Version {
            get
            {
                return string.Format("{0}.{1}", Marshal.PtrToStringUni(API.GetSDKVersion()), SDK_INTEGRATION_VERSION);
            }
        }

        /// <summary>
        /// Initializes integration API
        /// </summary>
        /// <returns>Initialized status</returns>
        public static bool Initialize()
        {
            if (!Initialized)
            {
                try
                {
                    AppKey = IntegrationSettings.AppKey;
                    if (!string.IsNullOrEmpty(AppKey))
                    {
                        if (PlatformMessages.InitializeMessages())
                        {
                            PlatformMessages.OnMessageReceived(HandlePlatformMessage);

                            new GameObject("ARVI.Integration.ResponseHandler", typeof(ResponseHandler));
                            new GameObject("ARVI.Integration.PlatformMessageHandler", typeof(PlatformMessageHandler));

                            if (!Application.runInBackground)
                                Debug.LogWarning("<b><color=red>Warning!</color></b> For ARVI SDK to work correctly, you must to set the option <b>\"Run In Background\"</b> in the project settings (Edit -> Project Settings -> Player -> Resolution and Presentation).");

                            Initialized = true;
                        }
                        else
                            throw new Exception(string.Format("{0} Error: {1}", Errors.PLATFORM_MESSAGES_NOT_INITIALIZED, PlatformMessages.GetErrorMessage()));
                    }
                    else
                        throw new Exception(Errors.EMPTY_APPKEY);
                }
                catch (DllNotFoundException)
                {
                    Debug.LogError(string.Format("{0} File {1}.dll not found in \"Plugins\" directory.", Errors.PLUGIN_NOT_FOUND, API.LIB_ARVI_DLL_NAME));
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
            return Initialized;
        }

        /// <summary>
        /// Checks if application is entitled to run on this computer
        /// </summary>
        /// <returns>Queued request</returns>
        public static Request<Response> IsApplicationEntitled()
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.IsApplicationEntitled(AppKey));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends notification about started game server
        /// </summary>
        /// <returns>Queued request</returns>
        public static Request<Response> ServerStarted()
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.ServerStarted());
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends notification about game completion
        /// </summary>
        /// <returns>Queued request</returns>
        public static Request<Response> GameCompleted()
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.GameCompleted());
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends request when operator needed
        /// </summary>
        /// <returns>Queued request</returns>
        public static Request<Response> CallOperator()
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.CallOperator());
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
}

        /// <summary>
        /// Sets an audio chat channel
        /// </summary>
        /// <param name="channel">Channel number</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetAudioChatChannel(AudioChatChannel channel)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.SetAudioChatChannel(channel));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Activates in-game command. If several commands have the same activation message, then they will all be activated
        /// </summary>
        /// <param name="activationMessage">Activation message</param>
        /// <returns>Queued request</returns>
        public static Request<Response> ActivateInGameCommand(string activationMessage)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.ActivateInGameCommand(activationMessage));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Deactivates in-game command. If several commands have the same deactivation message, then they will all be deactivated
        /// </summary>
        /// <param name="deactivationMessage">Activation message</param>
        /// <returns>Queued request</returns>
        public static Request<Response> DeactivateInGameCommand(string deactivationMessage)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.DeactivateInGameCommand(deactivationMessage));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends text message to platform
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="messageGroup">Optional parameter. The name of the group by which the messages will be grouped</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SendGameMessage(string message, string messageGroup = "")
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.SendGameMessage(message, messageGroup));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends text message to platform log
        /// </summary>
        /// <param name="message">Message text</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SendLogMessage(string message)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.SendLogMessage(message));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sends tracking message. It will be used for your events visualization
        /// </summary>
        /// <param name="message">Tracking message text</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SendTrackingMessage(string message)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.SendTrackingMessage(message));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        static Integration()
        {
            Initialized = false;
            IsApplicationInTrialMode = false;
            ShouldApplicationTrackCordTwisting = true;

            ReadCommandLineArgs();
        }

        private static void ReadCommandLineArgs()
        {
            foreach (var commandLineArg in Environment.GetCommandLineArgs())
                switch (commandLineArg.ToLower())
                {
                    case "-trial":
                        IsApplicationInTrialMode = true;
                        break;
                    case "-disablecordtwisttracking":
                        ShouldApplicationTrackCordTwisting = false;
                        break;
                }
        }

        private static void HandlePlatformMessage(PlatformMessage message)
        {
            if (message == null)
                return;
            // Check for known messages
            string messageName = message.Name.ToUpperInvariant();
            switch(messageName)
            {
                case "TIMELEFT":
                    message.SetResponse(HandleTimeLeftMessage());
                    break;
                case "POSITION":
                    message.SetResponse(HandlePlayerPositionMessage(), "application/json");
                    break;
                default:
                    if (PlatformMessageReceived != null)
                        PlatformMessageReceived.Invoke(message);
                    break;

            }
        }

        private static string HandleTimeLeftMessage()
        {
            string response = "";
            if (TimeLeftRequest != null)
            {
                int seconds;
                TimeLeftRequest.Invoke(out seconds);
                response = seconds.ToString(CultureInfo.InvariantCulture);
            }
            return response;
        }

        private static string HandlePlayerPositionMessage()
        {
            string response = "";
            if (PlayerPositionRequest != null)
            {
                Vector3 playerPosition;
                Vector3 playerForward;
                Vector3 playerUp;
                PlayerPositionRequest.Invoke(out playerPosition, out playerForward, out playerUp);
                response = string.Format(
                    "{{" +
                        "\"player\":{{" +
                            "\"position\":{{\"x\":{0:0.###},\"y\":{1:0.###},\"z\":{2:0.###}}}," +
                            "\"forward\":{{\"x\":{3:0.###},\"y\":{4:0.###},\"z\":{5:0.###}}}," +
                            "\"up\":{{\"x\":{6:0.###},\"y\":{7:0.###},\"z\":{8:0.###}}}" +
                        "}}," +
                        "\"camera\":{{" +
                            "\"position\":{{\"x\":{0:0.###},\"y\":{1:0.###},\"z\":{2:0.###}}}," +
                            "\"forward\":{{\"x\":{3:0.###},\"y\":{4:0.###},\"z\":{5:0.###}}}," +
                            "\"up\":{{\"x\":{6:0.###},\"y\":{7:0.###},\"z\":{8:0.###}}}" +
                        "}}" +
                    "}}",
                    playerPosition.x, playerPosition.y, playerPosition.z,
                    playerForward.x, playerForward.y, playerForward.z,
                    playerUp.x, playerUp.y, playerUp.z
                );
            }
            return response;
        }
    }
}