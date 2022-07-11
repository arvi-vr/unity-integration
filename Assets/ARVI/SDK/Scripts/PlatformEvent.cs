namespace ARVI.SDK
{
    using System;

    public enum PlatformEventType : int
    {
        Unknown = 0,
        PlayerNameChanged = 1,
        PlayerDominantHandChanged = 2
    }

    public class PlatformEvent
    {
        public PlatformEventType Type { get; private set; }

        public PlatformEvent(IntPtr api_event)
        {
            Type = API.Event_GetType(api_event);
        }
    }
}