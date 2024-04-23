namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;
    
    public static partial class API
    {
        internal const string LIB_ARVI_DLL_NAME = "libARVI";

        /// <summary>
        /// Gets the SDK version
        /// </summary>
        /// <returns>Pointer to version string</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr GetSDKVersion();

        /// <summary>
        /// Checks application entitlement
        /// </summary>
        /// <param name="appKey">Application Key, provided with API</param>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong IsApplicationEntitled([MarshalAs(UnmanagedType.LPWStr)] string appKey);

        /// <summary>
        /// Checks application entitlement (Deprecated)
        /// </summary>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, EntryPoint = "IsApplicationEntitledOld", CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong IsApplicationEntitled_v1();

        /// <summary>
        /// Notifies platform about starting game server
        /// </summary>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong ServerStarted();

        /// <summary>
        /// Notifies platform about completing the game
        /// </summary>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong GameCompleted();

        /// <summary>
        /// Notifies platform about operator calling
        /// </summary>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong CallOperator();

        /// <summary>
        /// Sets an audio chat channel
        /// </summary>
        /// <param name="channel">Channel number. Acceptable values are 0..10. Set 0, if you want join to default channel</param>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SetAudioChatChannel(AudioChatChannel channel);

        /// <summary>
        /// Activates in-game command. If several commands have the same activation message, then they will all be activated
        /// </summary>
        /// <param name="activationMessage">Activation message</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 128 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong ActivateInGameCommand([MarshalAs(UnmanagedType.LPWStr)] string activationMessage);

        /// <summary>
        /// Activates multiple in-game commands at once
        /// </summary>
        /// <param name="activationMessages">Activation messages</param>
        /// <param name="count">Length of activationMessages array</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong ActivateInGameCommands([In, Out] string[] activationMessages, int count);

        /// <summary>
        /// Deactivates in-game command. If several commands have the same deactivation message, then they will all be deactivated
        /// </summary>
        /// <param name="deactivationMessage">Deactivation message</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 128 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong DeactivateInGameCommand([MarshalAs(UnmanagedType.LPWStr)] string deactivationMessage);

        /// <summary>
        /// Deactivates multiple in-game commands at once
        /// </summary>
        /// <param name="deactivationMessages">Deactivation messages</param>
        /// <param name="count">Length of deactivationMessages array</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong DeactivateInGameCommands([In, Out] string[] deactivationMessages, int count);

        /// <summary>
        /// Sends text message to platform
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="messageGroup">Optional parameter. The name of the group by which the messages will be grouped</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2048 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendGameMessage([MarshalAs(UnmanagedType.LPWStr)] string message, [MarshalAs(UnmanagedType.LPWStr)] string messageGroup);

        /// <summary>
        /// Sends service/debug message which will be logged on platform
        /// </summary>
        /// <param name="message">Message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 10240 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendLogMessage([MarshalAs(UnmanagedType.LPWStr)] string message);

        /// <summary>
        /// Sends warning message which will be shown in administrative panel
        /// </summary>
        /// <param name="message">Message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 1 time per second and 10 times per minute. Message length should not exceed 2048 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendWarningMessage([MarshalAs(UnmanagedType.LPWStr)] string message);

        /// <summary>
        /// Sends tracking message which will be tracked on log timeline
        /// </summary>
        /// <param name="message">Tracking message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 time per second and 100 times per minute. Message length should not exceed 1024 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendTrackingMessage([MarshalAs(UnmanagedType.LPWStr)] string message);

        /// <summary>
        /// Sets the session-related data
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="data">Data value</param>
        /// <param name="size">Data size in bytes</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: name length should not exceed 256 chars. Max size of all session data variables (including name size) should not exceed 100Mb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SetSessionData([MarshalAs(UnmanagedType.LPWStr)] string name, [In] byte[] data, int size);

        /// <summary>
        /// Gets the session-related data
        /// </summary>
        /// <param name="name">Data name</param>
        /// <param name="data">Array of bytes that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of bytes in the array. Upon return, the value contains the number of data bytes</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetSessionData([MarshalAs(UnmanagedType.LPWStr)] string name, [In, Out] byte[] data, ref int size);

        /// <summary>
        /// Gets the UI Settings data
        /// </summary>
        /// <param name="name">Name of UI Setting</param>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetUISettingsData([MarshalAs(UnmanagedType.LPWStr)] string name, [In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Gets the number of players in the session
        /// </summary>
        /// <param name="playersCount">Players count</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayersCount(out int playersCount);

        /// <summary>
        /// Gets the game server IP
        /// </summary>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetServerIP([In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Gets the selected session language
        /// </summary>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetSessionLanguage([In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Gets the session time limit in seconds
        /// </summary>
        /// <param name="sessionTime">Session time in seconds</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetSessionTime(out int sessionTime);

        /// <summary>
        /// Gets the session ID
        /// </summary>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetSessionID([In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Gets the player ID
        /// </summary>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayerID([In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Gets the player name
        /// </summary>
        /// <param name="buffer">Array of chars that you should allocate</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of chars in the array. Upon return, the value contains the number of data chars</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayerName([In, Out] char[] buffer, ref int size);

        /// <summary>
        /// Sets the new player name and send it to platform
        /// </summary>
        /// <param name="name">Player name</param>
        /// <param name="changed">True if new player name successfully changed</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: name length should not exceed 128 chars</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SetPlayerName([MarshalAs(UnmanagedType.LPWStr)] string name, out bool changed);

        /// <summary>
        /// Gets the player dominant hand
        /// </summary>
        /// <param name="hand">Dominant hand value</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayerDominantHand(out int hand);

        /// <summary>
        /// Sets the new player dominant hand and send it to platform
        /// </summary>
        /// <param name="hand">Dominant hand value</param>
        /// <param name="changed">True if dominant hand value successfully changed</param>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SetPlayerDominantHand(int hand, out bool changed);

        /// <summary>
        /// Checks if the application should track cord twisting
        /// </summary>
        /// <returns>True in case of application should track cord twisting</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetShouldApplicationTrackCordTwisting();

        /// <summary>
        /// Checks if the application is running in trial mode
        /// </summary>
        /// <returns>True in case of application is running in trial mode</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool GetIsApplicationInTrialMode();
    }
}