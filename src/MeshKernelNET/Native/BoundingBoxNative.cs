using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BoundingBoxNative
    {
        /// <summary>
        /// The bounding box lower left x coordinate
        /// </summary>
        public double xLowerLeft;

        /// <summary>
        /// The bounding box lower left y coordinate
        /// </summary>
        public double yLowerLeft;

        /// <summary>
        /// The bounding box upper right x coordinate
        /// </summary>
        public double xUpperRight;

        /// <summary>
        /// The bounding box upper right y coordinate
        /// </summary>
        public double yUpperRight;
    }
}