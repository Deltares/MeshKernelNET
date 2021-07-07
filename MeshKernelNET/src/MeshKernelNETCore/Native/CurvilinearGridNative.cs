using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    /// @brief A struct used to describe the values of a curvilinear grid in a C-compatible manner
    [StructLayout(LayoutKind.Sequential)]
    public struct CurvilinearGridNative
    {
        /// @brief The nodes composing each mesh 2d edge
        public IntPtr edge_nodes;

        /// @brief The x-coordinates of network1d nodes
        public IntPtr node_x;

        /// @brief The y-coordinates of network1d nodes
        public IntPtr node_y;

        /// @brief The x-coordinates of the mesh edges middle points
        public IntPtr edge_x;

        /// @brief The y-coordinates of the mesh edges middle points
        public IntPtr edge_y;

        /// @brief The number of mesh nodes
        public int num_nodes;

        /// @brief The number of edges
        public int num_edges;
    }
}