namespace ARVI.SDK
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.Text;

    public enum MessageMethod
    {
        Unknown,
        GET,
        POST
    }

    public class PlatformMessage
    {
        private const int STATUS_CODE_OK = 200;
        private const string STATUS_TEXT_OK = "OK";

        private const int STATUS_CODE_ERROR = 500;
        private const string STATUS_TEXT_ERROR = "Internal Server Error";

        /// <value>Gets the HTTP method used for message (e.g., GET, POST)</value>
        public MessageMethod Method { get; private set; }

        /// <value>Gets the name of message (e.g., "skip")</value>
        public string Name { get; private set; }

        /// <value>"Name=Value" collection of message parameters</value>
        public NameValueCollection Params { get; private set; }

        private readonly IntPtr api_message = IntPtr.Zero;
        private readonly byte[] data;

        /// <summary>
        /// Message constructor
        /// </summary>
        /// <param name="api_message">Pointer to low-level message in memory</param>
        public PlatformMessage(IntPtr api_message)
        {
            this.api_message = api_message;
            Method = GetMessageMethodFromString(Marshal.PtrToStringUni(API.Message_GetMethod(api_message)));
            Name = Marshal.PtrToStringUni(API.Message_GetName(api_message));
            var paramsCount = API.Message_GetParamsCount(api_message);
            Params = new NameValueCollection(paramsCount);
            for (int i = 0; i < paramsCount; ++i)
                Params.Add(Marshal.PtrToStringUni(API.Message_GetParamName(api_message, i)), Marshal.PtrToStringUni(API.Message_GetParamValue(api_message, i)));
            var dataSize = API.Message_GetDataSize(api_message);
            if (dataSize > 0)
            {
                data = new byte[dataSize];
                if (!API.Message_GetData(api_message, data, ref dataSize))
                    data = new byte[0];
            }
            else
                data = new byte[0];
        }

        /// <summary>
        /// Gets raw binary data, if available
        /// </summary>
        /// <returns>Binary data, if available. Otherwise - null</returns>
        public byte[] GetData()
        {
            return data;
        }

        /// <summary>
        /// Gets string representation of data, if available
        /// </summary>
        /// <returns>String data</returns>
        public string GetDataAsString()
        {
            return Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Sets the binary data for message response
        /// </summary>
        /// <param name="response">Binary data</param>
        /// <returns>True in case of success call</returns>
        public bool SetResponse(byte[] response)
        {
            return API.Message_SetResponse(api_message, "application/octet-stream", STATUS_CODE_OK, STATUS_TEXT_OK, response, response.Length);
        }

        /// <summary>
        /// Sets the string data for message response
        /// </summary>
        /// <param name="response">String data</param>
        /// <returns>True in case of success call</returns>
        public bool SetResponse(string response)
        {
            return SetResponse(response, "text/plain");
        }

        /// <summary>
        /// Sets the string data for message response with specific Content-Type
        /// </summary>
        /// <param name="response">String data</param>
        /// <param name="contentType">Content-Type</param>
        /// <returns>True in case of success call</returns>
        public bool SetResponse(string response, string contentType)
        {
            var buffer = Encoding.UTF8.GetBytes(response);
            return API.Message_SetResponse(api_message, contentType, STATUS_CODE_OK, STATUS_TEXT_OK, buffer, buffer.Length);
        }

        /// <summary>
        /// Sets the error description for message
        /// </summary>
        /// <param name="error">Error description</param>
        /// <returns>True in case of success call</returns>
        public bool SetError(string error)
        {
            var buffer = Encoding.UTF8.GetBytes(error);
            return API.Message_SetResponse(api_message, "text/plain", STATUS_CODE_ERROR, STATUS_TEXT_ERROR, buffer, buffer.Length);
        }

        private MessageMethod GetMessageMethodFromString(string methodName)
        {
            switch (methodName.ToUpperInvariant())
            {
                case "GET":
                    return MessageMethod.GET;
                case "POST":
                    return MessageMethod.POST;
                default:
                    return MessageMethod.Unknown;
            }
        }
    }

    public static class PlatformMessages
    {
        private const int MESSAGE_BUFFER_SIZE = 1024;
        private static readonly IntPtr[] api_messages = new IntPtr[MESSAGE_BUFFER_SIZE];
        private static int api_messages_count = 0;
        private static Integration.PlatformMessageReceivedHandler messageReceivedCallback = null;
        private static Integration.PlatformEventReceivedHandler eventReceivedCallback = null;

        public static bool InitializeMessages()
        {
            return API.Messages_Initialize();
        }

        public static void FinalizeMessages()
        {
            API.Messages_Finalize();
        }

        /// <summary>
        /// Dequeues and executes available messages from platform
        /// </summary>
        public static void Execute()
        {
            api_messages_count = api_messages.Length;
            if (API.Messages_Get(api_messages, ref api_messages_count) && (api_messages_count > 0))
            {
                for (int i = 0; i < api_messages_count; ++i)
                {
                    if (API.Message_GetIsInternalMessage(api_messages[i]))
                    {
                        IntPtr events;
                        if (API.Message_HandleInternalMessage(api_messages[i], out events))
                        {
                            int eventsCount = API.EventList_GetCount(events);
                            for (int eventIndex = 0; eventIndex < eventsCount; ++eventIndex)
                            {
                                IntPtr @event = API.EventList_GetEvent(events, eventIndex);
                                HandleEvent(new PlatformEvent(@event));
                            }
                            API.EventList_Free(events);
                        }
                    }
                    else
                        HandleMessage(new PlatformMessage(api_messages[i]));
                    API.Message_Free(api_messages[i]);
                }
            }
        }

        /// <summary>
        /// Sets the method to be executed when received new platform message
        /// </summary>
        /// <param name="callback">Callback method</param>
        public static void OnMessageReceived(Integration.PlatformMessageReceivedHandler callback)
        {
            messageReceivedCallback = callback;
        }

        /// <summary>
        /// Sets the method to be executed when received new platform event
        /// </summary>
        /// <param name="callback">Callback method</param>
        public static void OnEventReceived(Integration.PlatformEventReceivedHandler callback)
        {
            eventReceivedCallback = callback;
        }

        /// <summary>
        /// Gets error message of messages module
        /// </summary>
        /// <returns>Error message string</returns>
        public static string GetErrorMessage()
        {
            return Marshal.PtrToStringUni(API.Messages_GetErrorMessage());
        }

        private static void HandleMessage(PlatformMessage message)
        {
            if (messageReceivedCallback != null)
                messageReceivedCallback.Invoke(message);
        }

        private static void HandleEvent(PlatformEvent @event)
        {
            if (eventReceivedCallback != null)
                eventReceivedCallback.Invoke(@event);
        }
    }
}
