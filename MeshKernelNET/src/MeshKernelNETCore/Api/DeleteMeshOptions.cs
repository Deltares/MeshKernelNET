using System.ComponentModel;

namespace MeshKernelNETCore.Api
{
    public enum DeleteMeshOptions
    {
        [Description("Delete vertices inside the polygon")]
        AllVerticesInside = 0,

        [Description("All faces whose circumcenter are inside the polygon")]
        FacesWithIncludedCircumcenters = 1,

        [Description("All faces completely  inside the polygon")]
        FacesCompletelyIncluded = 2
    }
}
