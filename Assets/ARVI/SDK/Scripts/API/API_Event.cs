namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class API
    {
        /// <summary>
        /// Handles internal message
        /// </summary>
        /// <param name="event">Event pointer</param>
        /// <returns>Event type</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern PlatformEventType Event_GetType(IntPtr @event);

        /// <summary>
        /// Gets items count in the event list
        /// </summary>
        /// <param name="eventList">Pointer to event list</param>
        /// <returns>Items count</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int EventList_GetCount(IntPtr eventList);

        /// <summary>
        /// Gets event by index
        /// </summary>
        /// <param name="eventList">Pointer to event list</param>
        /// <param name="index">Event index</param>
        /// <returns>Pointer to event</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr EventList_GetEvent(IntPtr eventList, int index);

        /// <summary>
        /// Free up event list resources
        /// </summary>
        /// <param name="eventList">Pointer to event list</param>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void EventList_Free(IntPtr eventList);
    }
}