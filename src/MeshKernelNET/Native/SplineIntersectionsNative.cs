using System;
using System.Runtime.InteropServices;

namespace MeshKernelNET.Native
{
    /// <summary>
    /// A struct used to describe the intersection points of a spline with a number of other splines.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SplineIntersectionsNative
    {
        /// <summary>
        /// The number of spline intersections
        /// </summary>
        public int NumIntersections { get; set; }

        /// <summary>
        /// The index of the intersected spline
        /// </summary>
        public IntPtr SplineIndex { get; set; }

        /// <summary>
        /// The angle of the intersection
        /// </summary>
        public IntPtr IntersectionAngle { get; set; }

        /// <summary>
        /// The x coordinate of the intersection point
        /// </summary>
        public IntPtr IntersectionX { get; set; }

        /// <summary>
        /// The y coordinate of the intersection point
        /// </summary>
        public IntPtr IntersectionY { get; set; }
    }
}