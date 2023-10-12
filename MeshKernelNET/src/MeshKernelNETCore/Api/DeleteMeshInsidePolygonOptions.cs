using System.ComponentModel;

namespace MeshKernelNETCore.Api
{
    public enum DeleteMeshInsidePolygonOptions
    {
        [Description("Not intersecting")]
        NotIntersecting = 0,

        [Description("Intersecting")]
        Intersecting = 1
    }
}
