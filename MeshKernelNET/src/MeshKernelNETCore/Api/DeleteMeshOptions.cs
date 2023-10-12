using System.ComponentModel;

namespace MeshKernelNETCore.Api
{
    public enum DeleteMeshOptions
    {
        [Description("Mesh inside the polygon and not intersecting with it")]
        InsideNotIntersected = 0,

        [Description("Mesh inside the polygon and intersecting with it")]
        InsideAndIntersected = 1
    }
}
