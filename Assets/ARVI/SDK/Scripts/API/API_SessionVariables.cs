namespace ARVI.SDK
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class API
    {
        /// <summary>
        /// Initializes session variables
        /// </summary>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SessionVariables_Initialize();

        /// <summary>
        /// Checks if session variables initialized
        /// </summary>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool SessionVariables_Initialized();

        /// <summary>
        /// Clear session variables and reset the internal state
        /// </summary>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SessionVariables_Finalize();

        /// <summary>
        /// Gets error message of session variables module
        /// </summary>
        /// <returns>Pointer to error description</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr SessionVariables_GetErrorMessage();
    }
}