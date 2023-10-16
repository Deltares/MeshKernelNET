using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    /// @brief A struct used to describe the values of a curvilinear grid in a C-compatible manner
    [StructLayout(LayoutKind.Sequential)]
    public struct CurvilinearGridNative
    {
        /// <summary>
        /// The x-coordinates of network1d nodes
        /// </summary>
        public IntPtr node_x;

        /// <summary>
        /// The y-coordinates of network1d nodes
        /// </summary>
        public IntPtr node_y;

        /// <summary>
        /// The number of curvilinear grid nodes along m
        /// </summary>
        public int num_m;

        /// <summary>
        /// The number of curvilinear grid nodes along n
        /// </summary>
        public int num_n;
    }
}