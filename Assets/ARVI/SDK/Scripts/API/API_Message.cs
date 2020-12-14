namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class API
    {
        /// <summary>
        /// Initializes the message module
        /// </summary>
        /// <returns>True if message module initialized</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Messages_Initialize();

        /// <summary>
        /// Checks if the message module initialized and is ready to receive messages from platform
        /// </summary>
        /// <returns>True if message module initialized</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Messages_Initialized();

        /// <summary>
        /// Finalizes the message module
        /// </summary>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Messages_Finalize();

        /// <summary>
        /// Dequeue game messages from internal message queue
        /// </summary>
        /// <param name="messages">Array of pointers that you should allocate. After calling this function this array contains the pointers of dequeued messages</param>
        /// <param name="count">Before calling this function, the caller sets the value to the number of elements in messages array. Upon return, the value contains the number of dequeued messages</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Messages_Get([In, Out] IntPtr[] messages, ref int count);

        /// <summary>
        /// Gets error message of message module
        /// </summary>
        /// <returns>Pointer to error description</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Messages_GetErrorMessage();

        /// <summary>
        /// Gets the name of message method. GET, POST, etc.
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <returns>Pointer to method name</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Message_GetMethod(IntPtr message);

        /// <summary>
        /// Gets the name of message. For example "skip"
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <returns>Pointer to message name</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Message_GetName(IntPtr message);

        /// <summary>
        /// Gets the count of message parameters
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <returns>Parameters count</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Message_GetParamsCount(IntPtr message);

        /// <summary>
        /// Gets the name of message parameter by index
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <param name="index">Parameter's index</param>
        /// <returns>Pointer to parameter's name</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Message_GetParamName(IntPtr message, int index);

        /// <summary>
        /// Gets the value of message parameter by index
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <param name="index">Parameter's index</param>
        /// <returns>Pointer to parameter's value</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Message_GetParamValue(IntPtr message, int index);

        /// <summary>
        /// Gets the size of message data (if available)
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <returns>The size of message data in bytes</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Message_GetDataSize(IntPtr message);

        /// <summary>
        /// Gets the message data (if available)
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <param name="data">Array of bytes that you should allocate. After calling this function this array contains the raw data bytes</param>
        /// <param name="size">Before calling this function, the caller sets the value to the number of bytes in data array. Upon return, the value contains the number of data bytes</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Message_GetData(IntPtr message, [In, Out] byte[] data, ref int size);

        /// <summary>
        /// Sets the response data for message
        /// </summary>
        /// <param name="message">Message pointer</param>
        /// <param name="contentType">Value of HTTP "Content-Type" header</param>
        /// <param name="responseCode">Value of HTTP response status code</param>
        /// <param name="responseText">Value of HTTP response status text</param>
        /// <param name="data">Array of response data bytes</param>
        /// <param name="size">Size of the data array</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool Message_SetResponse(IntPtr message, [MarshalAs(UnmanagedType.LPWStr)] string contentType, int statusCode, [MarshalAs(UnmanagedType.LPWStr)] string statusText, [In] byte[] data, int size);

        /// <summary>
        /// Free up message resources
        /// </summary>
        /// <param name="message">Message pointer</param>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Message_Free(IntPtr message);
    }
}