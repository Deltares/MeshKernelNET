using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    /// @brief A struct used to describe contacts between a 1d and a 2d mesh
    [StructLayout(LayoutKind.Sequential)]
    public struct ContactsNative
    {
        /// <summary>
        /// The indices of the 1d mesh
        /// </summary>
        public IntPtr mesh1d_indices;

        /// <summary>
        /// The indices of the 2d face
        /// </summary>
        public IntPtr mesh2d_indices;

        /// <summary>
        /// The number of contacts
        /// </summary>
        public int num_contacts;
    }
}