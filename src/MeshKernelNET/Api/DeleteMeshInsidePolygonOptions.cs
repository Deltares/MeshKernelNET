using System.ComponentModel;

namespace MeshKernelNET.Api
{
    public enum DeleteMeshInsidePolygonOptions
    {
        [Description("Inside not intersecting")]
        InsideNotIntersecting = 0,

        [Description("Inside and intersecting")]
        InsideAndIntersecting = 1,

        [Description("Faces with included circumcenters")]
        FacesWithIncludedCircumcenters = 2
    }
}