﻿using System.ComponentModel;

namespace MeshKernelNET.Api
{
    public enum DeleteMeshInsidePolygonOptions
    {
        [Description("Not intersecting")]
        NotIntersecting = 0,

        [Description("Intersecting")]
        Intersecting = 1,

        [Description("Faces with included circumcenters")]
        FacesWithIncludedCircumcenters = 2
    }
}