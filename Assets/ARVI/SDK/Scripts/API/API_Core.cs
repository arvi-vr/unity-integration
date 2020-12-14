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
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong ActivateInGameCommand([MarshalAs(UnmanagedType.LPWStr)] string activationMessage);

        /// <summary>
        /// Deactivates in-game command. If several commands have the same deactivation message, then they will all be deactivated
        /// </summary>
        /// <param name="deactivationMessage">Deactivation message</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong DeactivateInGameCommand([MarshalAs(UnmanagedType.LPWStr)] string deactivationMessage);

        /// <summary>
        /// Sends text message to platform
        /// </summary>
        /// <param name="message">Message text</param>
        /// <param name="messageGroup">Optional parameter. The name of the group by which the messages will be grouped</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 2Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendGameMessage([MarshalAs(UnmanagedType.LPWStr)] string message, [MarshalAs(UnmanagedType.LPWStr)] string messageGroup);

        /// <summary>
        /// Sends service/debug message which will be logged on platform
        /// </summary>
        /// <param name="message">Message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 times per second and 100 times per minute. Message length should not exceed 10Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendLogMessage([MarshalAs(UnmanagedType.LPWStr)] string message);

        /// <summary>
        /// Sends warning message which will be shown in administrative panel
        /// </summary>
        /// <param name="message">Message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 1 time per second and 10 times per minute. Message length should not exceed 2Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendWarningMessage([MarshalAs(UnmanagedType.LPWStr)] string message);

        /// <summary>
        /// Sends tracking message which will be tracked on log timeline
        /// </summary>
        /// <param name="message">Tracking message text</param>
        /// <returns>Associated Request ID</returns>
        /// <remarks>Limitations: no more than 10 time per second and 100 times per minute. Message length should not exceed 1Kb</remarks>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong SendTrackingMessage([MarshalAs(UnmanagedType.LPWStr)] string message);
    }
}