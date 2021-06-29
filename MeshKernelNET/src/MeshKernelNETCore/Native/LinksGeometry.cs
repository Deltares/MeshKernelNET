using System;
using System.Runtime.InteropServices;

namespace MeshKernelNETCore.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LinksGeometry
    {
        public IntPtr Mesh1DFrom;
        public IntPtr Mesh2DTo;
        public IntPtr LinkType;
        public IntPtr LinkId;
        public IntPtr LinkLongName;
    }
}