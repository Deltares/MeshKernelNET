using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Network1DGeometryDimensions
    {
        public int NumberOfNodes;

        public int NumberOfBranches;

        public int NumberOfBranchGeometryPoints;
    }
}