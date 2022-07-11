namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class API
    {
        /// <summary>
        /// Initializes the request module
        /// </summary>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Requests_Initialize();

        /// <summary>
        /// Finalizes the request module
        /// </summary>
        /// <param name="waitForComplete">Whether to wait for requests to complete</param>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Requests_Finalize(bool waitForComplete);

        /// <summary>
        /// Dequeue responses from internal response queue
        /// </summary>
        /// <param name="responses">Array of pointers that you should allocate. After calling this function this array contains the pointers of dequeued responses</param>
        /// <param name="count">Before calling this function, the caller sets the value to the number of elements in responses array. Upon return, the value contains the number of dequeued responses</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Responses_Get([In, Out] IntPtr[] responses, ref int count);

        /// <summary>
        /// Retrieve type of response
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <returns>Type of response</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ResponseType Response_GetType(IntPtr response);

        /// <summary>
        /// Returns Request ID, associated with response
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <returns>Associated Request ID</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong Response_GetRequestID(IntPtr response);

        /// <summary>
        /// Gets response error code
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <returns>Error code</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Response_GetErrorCode(IntPtr response);

        /// <summary>
        /// Gets response error message
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <returns>Pointer to error description</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Response_GetErrorMessage(IntPtr response);

        /// <summary>
        /// Gets the size of response data
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <returns>The size of response data in bytes</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Response_GetDataSize(IntPtr response);

        /// <summary>
        /// Gets the response data
        /// </summary>
        /// <param name="response">Response pointer</param>
        /// <param name="data">Array of bytes that you should allocate. After calling this function this array contains the raw data bytes</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of bytes in data array. Upon return, the value contains the number of data bytes</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Response_GetData(IntPtr response, [In, Out] byte[] data, ref int size);

        /// <summary>
        /// Free up response resources
        /// </summary>
        /// <param name="response">Response pointer</param>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Response_Free(IntPtr response);
    }
}