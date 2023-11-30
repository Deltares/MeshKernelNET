using System.ComponentModel;

namespace MeshKernelNET.Api
{
    public enum ProjectionOptions
    {
        [Description("Cartesian")]
        Cartesian = 0,

        [Description("Spherical")]
        Spherical = 1,

        [Description("Spherical Accurate")]
        SphericalAccurate = 2
    }
}