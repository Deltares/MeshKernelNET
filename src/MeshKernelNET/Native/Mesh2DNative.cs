using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    /// <summary>
    /// Data object for defining a 2D grid. This object will be
    /// used for communicating with the MeshKernel.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Mesh2DNative
    {
        /// <summary>
        /// The left and right face indices of each face
        /// </summary>
        public IntPtr edge_faces;

        /// <summary>
        /// The nodes composing each mesh 2d edge
        /// </summary>
        public IntPtr edge_nodes;

        /// <summary>
        /// The edges composing each face
        /// </summary>
        public IntPtr face_edges;

        /// <summary>
        /// The nodes composing each mesh 2d face
        /// </summary>
        public IntPtr face_nodes;

        /// <summary>
        /// The number of nodes for each mesh 2d face
        /// </summary>
        public IntPtr nodes_per_face;

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
        /// The x-coordinates of the mesh faces mass centers
        /// </summary>
        public IntPtr face_x;

        /// <summary>
        /// The y-coordinates of the mesh faces mass centers
        /// </summary>
        public IntPtr face_y;

        /// <summary>
        /// The number of mesh nodes
        /// </summary>
        public int num_nodes;

        /// <summary>
        /// The number of edges
        /// </summary>
        public int num_edges;

        /// <summary>
        /// The number of faces
        /// </summary>
        public int num_faces;

        /// <summary>
        /// The total number of nodes composing the mesh 2d faces
        /// </summary>
        public int num_face_nodes;
    }
}
