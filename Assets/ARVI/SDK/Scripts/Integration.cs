namespace ARVI.SDK
{
    using System;
    using System.Text;
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

    public enum DominantHand : int
    {
        NotSet = 0,
        Left = 1,
        Right = 2,
        Any = 3
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
        internal const string UNKNOWN_ERROR = "Unknown error.";
        internal const string EMPTY_APPKEY = "Application Key is empty. You can configure it in ARVI->Integration->Settings menu.";
        internal const string INTEGRATION_NOT_INITIALIZED = "Integration API not initialized. Was Integration.Initialize() called?";
        internal const string PLUGIN_NOT_FOUND = "Plugin not found.";
        internal const string PLATFORM_MESSAGES_NOT_INITIALIZED = "Platform Message module not initialized.";
    }

    public static class Integration
    {
        private const int SDK_INTEGRATION_VERSION = 4;

        #region Events
        public delegate void PlatformMessageReceivedHandler(PlatformMessage message);
        public static event PlatformMessageReceivedHandler PlatformMessageReceived;

        public delegate void PlatformEventReceivedHandler(PlatformEvent @event);

        public delegate void PlatformTimeLeftRequestHandler(out int seconds);
        public static event PlatformTimeLeftRequestHandler TimeLeftRequest;

        public delegate void PlatformPlayerPositionRequestHandler(out Vector3 playerPosition, out Vector3 playerForward, out Vector3 playerUp);
        public static event PlatformPlayerPositionRequestHandler PlayerPositionRequest;

        public delegate void PlatformPlayerNameChangedHandler(string name);
        public static event PlatformPlayerNameChangedHandler PlayerNameChanged;

        public delegate void PlatformPlayerDominantHandChangedHandler(DominantHand hand);
        public static event PlatformPlayerDominantHandChangedHandler PlayerDominantHandChanged;
        #endregion

        #region Properties
        /// <value>Gets if the integration API initialized and ready to use</value>
        public static bool Initialized { get; private set; }

        /// <value>Application Key, provided with API and used for entitlement checks</value>
        public static string AppKey { get; private set; }

        /// <value>Player's ID</value>
        public static string PlayerID { get; private set; }

        /// <value>Player's name</value>
        public static string PlayerName { get { return GetPlayerName(); } }

        /// <value>Player's dominant hand</value>
        public static DominantHand PlayerDominantHand { get { return GetPlayerDominantHand(); } }

        /// <value>Gets if the application is running in trial mode</value>
        public static bool IsApplicationInTrialMode { get; private set; }

        /// <value>Gets if the game should track cord twisting</value>
        public static bool ShouldApplicationTrackCordTwisting { get; private set; }

        /// <value>Unique game session identifier</value>
        public static string SessionID { get; private set; }

        /// <value>Game session language</value>
        public static string SessionLanguage { get; private set; }

        /// <value>Game session time in seconds</value>
        public static int SessionTime { get; private set; }

        /// <value>Number of players in the session</value>
        public static int PlayersCount { get; private set; }

        /// <value>Game server IP address</value>
        public static string ServerIP { get; private set; }

        /// <value>SDK version</value>
        public static string Version {
            get
            {
                return string.Format("{0}.{1}", Marshal.PtrToStringUni(API.GetSDKVersion()), SDK_INTEGRATION_VERSION);
            }
        }
        #endregion

        #region "Known messages"
        private const string MSG_TIMELEFT = "TIMELEFT";
        private const string MSG_POSITION = "POSITION";
        #endregion

        #region Methods
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
                            Requests.InitializeRequests();

                            PlatformMessages.OnMessageReceived(HandlePlatformMessage);
                            PlatformMessages.OnEventReceived(HandlePlatformEvent);

                            new GameObject("ARVI.Integration.ResponseHandler", typeof(ResponseHandler));
                            new GameObject("ARVI.Integration.PlatformMessageHandler", typeof(PlatformMessageHandler));

                            if (!Application.runInBackground)
                                Debug.LogWarning("<b><color=red>Warning!</color></b> For ARVI SDK to work correctly, you must to set the option <b>\"Run In Background\"</b> in the project settings (Edit -> Project Settings -> Player -> Resolution and Presentation).");

                            if (InitializeSessionVariables())
                            {
                                Initialized = true;
                            }
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

        private static bool InitializeSessionVariables()
        {
#if UNITY_EDITOR
            SessionVariables.FinalizeVariables();
#endif
            if (SessionVariables.InitializeVariables())
            {
                // Initialize player ID
                string playerID;
                if (TryGetPlayerID(out playerID))
                    PlayerID = playerID;
                else
                    Debug.LogWarning("Failed to initialize player ID");
                // Initialize session ID
                string sessionID;
                if (TryGetSessionID(out sessionID))
                    SessionID = sessionID;
                else
                    Debug.LogWarning("Failed to initialize session ID");
                // Initialize session language
                string sessionLanguage;
                if (TryGetSessionLanguage(out sessionLanguage))
                    SessionLanguage = sessionLanguage;
                else
                    Debug.LogWarning("Failed to initialize session language");
                // Initialize session time
                int sessionTime;
                if (TryGetSessionTime(out sessionTime))
                    SessionTime = sessionTime;
                else
                    Debug.LogWarning("Failed to initialize session time");
                // Initialize players count
                int playersCount;
                if (TryGetPlayersCount(out playersCount))
                    PlayersCount = playersCount;
                else
                    Debug.LogWarning("Failed to initialize players count");
                // Initialize game server IP
                string serverIP;
                if (TryGetServerIP(out serverIP))
                    ServerIP = serverIP;
                else
                    Debug.LogWarning("Failed to initialize server IP");
                // Initialize trial mode
                IsApplicationInTrialMode = GetIsApplicationInTrialMode();
                // Initialize cord twisting tracking
                ShouldApplicationTrackCordTwisting = GetShouldApplicationTrackCordTwisting();

                return true;
            }
            else
            {
                Debug.LogWarning(string.Format("<b><color=red>Warning!</color></b> Failed to initialize session variables: {0}. Is VRP Client running?", Marshal.PtrToStringUni(API.SessionVariables_GetErrorMessage())));
                return false;
            }
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
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 128 chars</remarks>
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
        /// Activates multiple in-game commands at once
        /// </summary>
        /// <param name="activationMessages">Activation messages</param>
        /// <returns>Queued request</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
        public static Request<Response> ActivateInGameCommands(string[] activationMessages)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.ActivateInGameCommands(activationMessages, activationMessages.Length));
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
        /// <param name="deactivationMessage">Deactivation message</param>
        /// <returns>Queued request</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 128 chars</remarks>
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
        /// Deactivates multiple in-game commands at once
        /// </summary>
        /// <param name="deactivationMessages">Deactivation messages</param>
        /// <returns>Queued request</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
        public static Request<Response> DeactivateInGameCommands(string[] deactivationMessages)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.DeactivateInGameCommands(deactivationMessages, deactivationMessages.Length));
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
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
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
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 10240 chars</remarks>
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
        /// <remarks>Limitations: no more than 10 time per second and 100 times per minute. Message length should not exceed 1024 chars</remarks>
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

        /// <summary>
        /// Sets the new player name
        /// </summary>
        /// <param name="name">Player name</param>
        /// <returns>Queued request</returns>
        /// <remarks>Limitations: name length should not exceed 128 chars</remarks>
        public static Request<Response> SetPlayerName(string name)
        {
            try
            {
                if (Initialized)
                {
                    bool changed;
                    var request = new Request<Response>(API.SetPlayerName(name, out changed));
                    if (changed)
                        HandlePlayerNameChangedEvent();
                    return request;
                }
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Sets the new player dominant hand
        /// </summary>
        /// <param name="hand">Player dominant hand</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetPlayerDominantHand(DominantHand hand)
        {
            if (Initialized)
            {
                bool changed;
                var request = new Request<Response>(API.SetPlayerDominantHand((int)hand, out changed));
                if (changed)
                    HandlePlayerDominantHandChangedEvent();
                return request;
            }
            else
                throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">bool data to save</param>
        /// <returns>Queued request</returns>
        /// <remarks>Limitations: name length should not exceed 256 chars. Max size of all sesion data variables (including name size) should not exceed 100Mb</remarks>
        public static Request<Response> SetSessionData(string name, bool value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">char data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, char value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">double data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, double value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">float data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, float value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">int data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, int value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">long data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, long value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">short data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, short value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">uint data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, uint value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">ulong data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, ulong value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">ushort data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, ushort value)
        {
            return SetSessionData(name, BitConverter.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">string data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, string value)
        {
            return SetSessionData(name, Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// Sets session-related data by name
        /// </summary>
        /// <param name="name">Data name. Chosen by you</param>
        /// <param name="value">Binary data to save</param>
        /// <returns>Queued request</returns>
        public static Request<Response> SetSessionData(string name, byte[] value)
        {
            try
            {
                if (Initialized)
                    return new Request<Response>(API.SetSessionData(name, value, value.Length));
                else
                    throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
            }
            catch (Exception ex)
            {
                return Request<Response>.ConstructInvalid(ex.Message);
            }
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Boolean value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out bool value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetBoolean() : false;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Char value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out char value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetChar() : '\0';
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Double value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out double value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetDouble() : 0D;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Float value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out float value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetFloat() : 0F;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Int32 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out int value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetInt32() : 0;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Int64 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out long value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetInt64() : 0L;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Int16 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out short value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetInt16() : (short)0;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">UInt32 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out uint value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetUInt32() : 0U;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">UInt64 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out ulong value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetUInt64(): 0UL;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">UInt16 value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out ushort value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? (ushort)0 : sessionVariable.GetUInt16();
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">String value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out string value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetString(): string.Empty;
            return success;
        }

        /// <summary>
        /// Gets session-related data by name
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="value">Byte array value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetSessionData(string name, out byte[] value)
        {
            SessionVariable sessionVariable;
            bool success = TryGetSessionVariable(name, out sessionVariable);
            value = success ? sessionVariable.GetBytes(): new byte[0];
            return success;
        }

        /// <summary>
        /// Gets UI settings by name
        /// </summary>
        /// <param name="name">UI setting name</param>
        /// <param name="value">UI setting string value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetUISettings(string name, out string value)
        {
            bool success = false;
            value = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetUISettingsData(name, chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetUISettingsData(name, chars, ref size))
                {
                    value = new string(chars);
                    success = true;
                }
            }
            return success;
        }

        /// <summary>
        /// Gets UI settings by name
        /// </summary>
        /// <param name="name">UI setting name</param>
        /// <param name="value">UI setting int value</param>
        /// <returns>True in case of success call</returns>
        public static bool TryGetUISettings(string name, out int value)
        {
            bool success = false;
            value = 0;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetUISettingsData(name, chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetUISettingsData(name, chars, ref size))
                {
                    string stringValue = new string(chars);
                    success = int.TryParse(stringValue, out value);
                }
            }
            return success;
        }
        #endregion

        private static readonly CultureInfo invariantCulture;

        static Integration()
        {
            Initialized = false;
            invariantCulture = CultureInfo.InvariantCulture;
        }

        private static string GetPlayerName()
        {
            string result = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetPlayerName(chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetPlayerName(chars, ref size))
                    result = new string(chars);
            }
            return result;
        }

        private static DominantHand GetPlayerDominantHand()
        {
            int handValue;
            if (API.TryGetPlayerDominantHand(out handValue))
                return Enum.IsDefined(typeof(DominantHand), handValue) ? (DominantHand)handValue : DominantHand.NotSet;
            else
                return DominantHand.NotSet;
        }

        private static bool GetIsApplicationInTrialMode()
        {
            return API.GetIsApplicationInTrialMode();
        }

        private static bool GetShouldApplicationTrackCordTwisting()
        {
            return API.GetShouldApplicationTrackCordTwisting();
        }

        private static bool TryGetPlayerID(out string playerID)
        {
            bool success = false;
            playerID = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetPlayerID(chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetPlayerID(chars, ref size))
                {
                    playerID = new string(chars);
                    success = true;
                }
            }
            return success;
        }

        private static bool TryGetSessionID(out string sessionID)
        {
            bool success = false;
            sessionID = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetSessionID(chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetSessionID(chars, ref size))
                {
                    sessionID = new string(chars);
                    success = true;
                }
            }
            return success;
        }

        private static bool TryGetSessionLanguage(out string sessionLanguage)
        {
            bool success = false;
            sessionLanguage = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetSessionLanguage(chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetSessionLanguage(chars, ref size))
                {
                    sessionLanguage = new string(chars);
                    success = true;
                }
            }
            return success;
        }

        private static bool TryGetServerIP(out string serverIP)
        {
            bool success = false;
            serverIP = string.Empty;
            char[] chars = null;
            int size = 0;
            if (!API.TryGetServerIP(chars, ref size) && (size > 0))
            {
                chars = new char[size];
                if (API.TryGetServerIP(chars, ref size))
                {
                    serverIP = new string(chars);
                    success = true;
                }
            }
            return success;
        }

        private static bool TryGetPlayersCount(out int playersCount)
        {
            return API.TryGetPlayersCount(out playersCount);
        }

        private static bool TryGetSessionTime(out int sessionTime)
        {
            return API.TryGetSessionTime(out sessionTime);
        }

        private static bool TryGetSessionVariable(string name, out SessionVariable variable)
        {
            if (Initialized)
            {
                bool success = false;
                variable = null;
                int size = 0;
                if (!API.TryGetSessionData(name, null, ref size) && (size > 0))
                {
                    byte[] bytes = new byte[size];
                    if (API.TryGetSessionData(name, bytes, ref size))
                    {
                        variable = new SessionVariable(bytes);
                        success = true;
                    }
                };
                return success;
            }
            else
                throw new Exception(Errors.INTEGRATION_NOT_INITIALIZED);
        }

        private static void HandlePlatformMessage(PlatformMessage message)
        {
            if (message == null)
                return;
            // Check for known messages
            string messageName = message.Name.ToUpperInvariant();
            switch(messageName)
            {
                case MSG_TIMELEFT:
                    message.SetResponse(HandleTimeLeftMessage());
                    break;
                case MSG_POSITION:
                    message.SetResponse(HandlePlayerPositionMessage(), "application/json");
                    break;
                default:
                    if (PlatformMessageReceived != null)
                        PlatformMessageReceived.Invoke(message);
                    break;

            }
        }
        
        private static void HandlePlatformEvent(PlatformEvent @event)
        {
            switch (@event.Type)
            {
                case PlatformEventType.PlayerNameChanged:
                    HandlePlayerNameChangedEvent();
                    break;
                case PlatformEventType.PlayerDominantHandChanged:
                    HandlePlayerDominantHandChangedEvent();
                    break;
                default:
                    Debug.LogWarning(string.Format("Unknown platform event type: {0}", @event.Type));
                    break;
            }
        }

        private static void HandlePlayerNameChangedEvent()
        {
            if (PlayerNameChanged != null)
                PlayerNameChanged.Invoke(GetPlayerName());
        }

        private static void HandlePlayerDominantHandChangedEvent()
        {
            if (PlayerDominantHandChanged != null)
                PlayerDominantHandChanged.Invoke(GetPlayerDominantHand());
        }

        private static string HandleTimeLeftMessage()
        {
            string response = "";
            if (TimeLeftRequest != null)
                try
                {
                    int seconds;
                    TimeLeftRequest.Invoke(out seconds);
                    response = seconds.ToString(invariantCulture);
                }
                catch { }
            return response;
        }

        private static string HandlePlayerPositionMessage()
        {
            string response = "";
            if (PlayerPositionRequest != null)
                try
                {
                    Vector3 playerPosition;
                    Vector3 playerForward;
                    Vector3 playerUp;
                    PlayerPositionRequest.Invoke(out playerPosition, out playerForward, out playerUp);
                    response = string.Format(invariantCulture,
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
                catch { }
            return response;
        }
    }
}