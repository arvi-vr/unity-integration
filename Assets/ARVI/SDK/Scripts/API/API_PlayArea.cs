namespace ARVI.SDK
{
    using System.Runtime.InteropServices;

    public static partial class API
    {
        /// <summary>
        /// Gets the play area checking mode
        /// </summary>
        /// <param name="mode">Mode value</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayAreaCheckingMode(out int mode);

        /// <summary>
        /// Gets the mode indicating what should happen when the player moves out of play area bounds
        /// </summary>
        /// <param name="mode">Mode value</param>
        /// <returns>True in case of success call</returns>
        [DllImport(LIB_ARVI_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool TryGetPlayAreaOutOfBoundsMode(out int mode);
    }
}