using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MakeGridParametersNative
    {
        /// <summary>   
        /// * The number of columns in x direction (3)
        /// </summary>
        public int NumberOfColumns;

        /// <summary>
        /// * The number of columns in y direction (3)
        /// </summary>
        public int NumberOfRows;

        /// <summary>
        /// * The grid angle (0.0)
        /// </summary>
        public double GridAngle;

        /// <summary>
        /// * The x coordinate of the origin, located at the bottom left corner (0.0)	 
        /// </summary>
        public double OriginXCoordinate;

        /// <summary>
        /// * The y coordinate of the origin, located at the bottom left corner (0.0)	 
        /// </summary>
        public double OriginYCoordinate;

        /// <summary>
        /// * The grid block size in x dimension, used only for squared grids (10.0) 
        /// </summary>
        public double XGridBlockSize;

        /// <summary>
        /// * The grid block size in y dimension, used only for squared grids (10.0) 
        /// </summary>
        public double YGridBlockSize;

        /// <summary>
        /// * The x coordinate of the upper right corner (0.0) 
        /// </summary>
        public double UpperRightCornerXCoordinate;

        /// <summary>
        /// * The y coordinate of the upper right corner (0.0) 
        /// </summary>
        public double UpperRightCornerYCoordinate;
    }
}
