namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class API
    {
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
        /// Free up response resources
        /// </summary>
        /// <param name="response">Response pointer</param>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Response_Free(IntPtr response);
    }
}