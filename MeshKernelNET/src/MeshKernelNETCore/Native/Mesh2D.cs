using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    /// <summary>
    /// Data object for defining a 2D grid. This object will be
    /// used for communicating with the MeshKernel.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Mesh2D
    {
        public IntPtr edge_nodes;

        public IntPtr face_nodes;

        public IntPtr nodes_per_face;

        public IntPtr node_x;

        public IntPtr node_y;

        public IntPtr edge_x;

        public IntPtr edge_y;

        public IntPtr face_x;

        public IntPtr face_y;

        public int num_nodes;

        public int num_edges;

        public int num_faces;

        public int num_face_nodes;
    }
}
