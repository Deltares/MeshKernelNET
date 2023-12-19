using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    /// @brief A struct used to describe the values of a mesh 1d in a C-compatible manner
    [StructLayout(LayoutKind.Sequential)]
    public struct Mesh1DNative
    {
        /// <summary>
        /// The nodes composing each mesh 1d edge
        /// </summary>
        public IntPtr edge_nodes;

        /// <summary>
        /// The x-coordinates of the mesh nodes
        /// </summary>
        public IntPtr node_x;

        /// <summary>
        /// The y-coordinates of the mesh nodes
        /// </summary>
        public IntPtr node_y;

        /// <summary>
        /// The number of 1d nodes
        /// </summary>
        public int num_nodes;

        /// <summary>
        /// The number of 1d edges
        /// </summary>
        public int num_edges;
    }

} // namespace meshkernelapi
