using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GeometryListNative
    {
        /// <summary>
        /// The value used as separator in coordinates_x, coordinates_y and values
        /// </summary>
        public double geometrySeperator;

        /// <summary>
        /// The value used to separate the inner part of a polygon from its outer part
        /// </summary>
        public double innerOuterSeperator;

        /// <summary>
        /// The number of coordinate values present
        /// </summary>
        public int numberOfCoordinates;

        /// <summary>
        /// The x coordinate values
        /// </summary>
        public IntPtr xCoordinates;

        /// <summary>
        /// The y coordinate values
        /// </summary>
        public IntPtr yCoordinates;

        /// <summary>
        /// The values at the point
        /// </summary>
        public IntPtr zCoordinates;
    }
}
