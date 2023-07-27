using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
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
        double x_origin;

        /// <summary>
        /// Y coordinate of the grid origin (lower left corner)
        /// </summary>
        double y_origin;

        /// <summary>
        /// Constant grid cell size
        /// </summary>
        double cell_size;

        /// <summary>
        /// If not nullptr, coordinates for non-uniform grid spacing in x direction
        /// </summary>
        IntPtr x_coordinates;

        /// <summary>
        /// If not nullptr, coordinates for non-uniform grid spacing in y direction
        /// </summary>
        IntPtr y_coordinates;

        /// <summary>
        /// Sample values
        /// </summary>
        IntPtr values;

        /// <summary>
        /// X coordinate of the upper right
        /// </summary>
        double x_upper_right;

        /// <summary>
        /// Y coordinate of the upper right
        /// </summary>
        double y_upper_right;
    }
}