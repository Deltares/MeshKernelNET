using System.ComponentModel;

namespace MeshKernelNETCore.Api
{
    public enum DeleteMeshOptions
    {
        [Description("Inside the polygon and intersecting with it")]
        InsideNotIntersected = 0,

        [Description("Inside the polygon and not intersecting with it")]
        InsideAndIntersected = 1
    }
}
