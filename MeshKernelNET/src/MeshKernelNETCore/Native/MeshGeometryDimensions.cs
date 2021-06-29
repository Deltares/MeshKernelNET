using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MeshGeometryDimensions
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
        public char[] meshname;

        public int dim;
        public int numnode;
        public int numedge;
        public int numface;
        public int maxnumfacenodes;
        public int numlayer;
        public int layertype;
        public int nnodes;
        public int nbranches;
        public int ngeometry;
        public int epgs;
        public int numlinks;
    }
}