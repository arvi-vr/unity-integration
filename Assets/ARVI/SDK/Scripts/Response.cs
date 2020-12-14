namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public enum ResponseType : int
    {
        Unknown = 0,
        Default = 1,
    }

    /// <summary>
    /// This class provides error information in case the request failed
    /// </summary>
    public class Error
    {
        /// <value>Integer error code</value>
        public int Code { get; private set; }

        /// <value>Human-readable error description</value>
        public string Message { get; private set; }

        /// <summary>
        /// Error constructor
        /// </summary>
        /// <param name="code">Error code</param>
        /// <param name="message">Error description</param>
        public Error(int code, string message)
        {
            Code = code;
            Message = message;
        }
    }

    public static class Responses
    {
        private const int RESPONSE_BUFFER_SIZE = 1024;
        private static readonly IntPtr[] api_responses = new IntPtr[RESPONSE_BUFFER_SIZE];

        /// <summary>
        /// Dequeues available responses from internal queue for further processing
        /// </summary>
        /// <returns>Array of responses</returns>
        public static Response[] Dequeue()
        {
            Response[] responses = null;
            int count = api_responses.Length;
            if (API.Responses_Get(api_responses, ref count) && (count > 0))
            {
                responses = new Response[count];
                for (int i = 0; i < count; ++i)
                {
                    responses[i] = Response.Construct(api_responses[i]);
                    API.Response_Free(api_responses[i]);
                }
            }
            return responses;
        }
    }

    public class Response
    {
        /// <value>Gets the type of response</value>
        public ResponseType Type { get; private set; }

        /// <value>ID of the request, the response to which is this object</value>
        public ulong RequestID { get; private set; }

        /// <value>Indicates whether the request was processed successfully</value>
        public bool Success { get { return (Error == null); } }

        /// <value>Gets the error information in case if request failed</value>
        public Error Error { get; private set; }

        /// <summary>
        /// Response constructor
        /// </summary>
        /// <param name="api_response">Pointer to low-level response in memory</param>
        /// <param name="type">Response type</param>
        public Response(IntPtr api_response, ResponseType type)
        {
            Type = type;
            RequestID = API.Response_GetRequestID(api_response);
            var errorCode = API.Response_GetErrorCode(api_response);
            if (errorCode != 0)
            {
                var errorMessage = Marshal.PtrToStringUni(API.Response_GetErrorMessage(api_response));
                Error = new Error(errorCode, errorMessage);
            }
        }

        public Response(ResponseType type, Error error = null)
        {
            Type = type;
            Error = error;
        }

        internal static Response Construct(IntPtr api_response)
        {
            var responseType = API.Response_GetType(api_response);

            switch (responseType)
            {
                default:
                    return new Response(api_response, responseType);
            }
        }
    }
}