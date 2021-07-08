using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    /// @brief A struct used to describe the values of a curvilinear grid in a C-compatible manner
    [StructLayout(LayoutKind.Sequential)]
    public struct CurvilinearGridNative
    {
        /// <summary>
        /// The nodes composing each mesh 2d edge
        /// </summary>
        public IntPtr edge_nodes;

        /// <summary>
        /// The x-coordinates of network1d nodes
        /// </summary>
        public IntPtr node_x;

        /// <summary>
        /// The y-coordinates of network1d nodes
        /// </summary>
        public IntPtr node_y;

        /// <summary>
        /// The x-coordinates of the mesh edges middle points
        /// </summary>
        public IntPtr edge_x;

        /// <summary>
        /// The y-coordinates of the mesh edges middle points
        /// </summary>
        public IntPtr edge_y;

        /// <summary>
        /// The number of mesh nodes
        /// </summary>
        public int num_nodes;

        /// <summary>
        /// The number of edges
        /// </summary>
        public int num_edges;
    }
}