using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Mesh1DGeometryDimensions
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public char[] name;
        
        public int NumberOfNodes;

        public int NumberOfEdges;
    }
}