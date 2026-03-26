namespace MeshKernelNET.Api
{
    /// <summary>
    /// Enum for different properties on a 2D mesh.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// The orthogonality of the mesh.
        /// </summary>
        Orthogonality = 0,

        /// <summary>
        /// The length of the mesh edges.
        /// </summary>
        EdgeLength = 1,

        /// <summary>
        /// The circumcenter of the mesh faces.
        /// </summary>
        FaceCircumcenter = 2,

        /// <summary>
        /// The control volume polygons around mesh edges.
        /// </summary>
        NetlinkContourPolygon = 3,
        
        /// <summary>
        /// The bounding box of the mesh faces.
        /// </summary>
        FaceBounds = 4
    }
}