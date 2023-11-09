using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    /// @brief A struct describing gridded samples
    [StructLayout(LayoutKind.Sequential)]
    public struct GriddedSamplesNative
    {
        /// <summary>
        /// Number of x gridded samples coordinates
        /// </summary>
        public int num_x;

        /// <summary>
        /// Number of y gridded samples coordinates
        /// </summary>
        public int num_y;

        /// <summary>
        /// X coordinate of the grid origin (lower left corner)
        /// </summary>
        public double origin_x;

        /// <summary>
        /// Y coordinate of the grid origin (lower left corner)
        /// </summary>
        public double origin_y;

        /// <summary>
        /// Constant grid cell size
        /// </summary>
        public double cell_size;

        /// <summary>
        /// If not nullptr, coordinates for non-uniform grid spacing in x direction
        /// </summary>
        public IntPtr coordinates_x;

        /// <summary>
        /// If not nullptr, coordinates for non-uniform grid spacing in y direction
        /// </summary>
        public IntPtr coordinates_y;

        /// <summary>
        /// Sample values
        /// </summary>
        public IntPtr values;

        /// <summary>
        /// X coordinate of the upper right
        /// </summary>
        public double upper_right_x;

        /// <summary>
        /// Y coordinate of the upper right
        /// </summary>
        public double upper_right_y;
    }
}