using System.ComponentModel;

namespace MeshKernelNET.Api
{
    /// <summary>
    /// Specifies the different types of mesh refinement techniques
    /// available in the mesh kernel.
    /// </summary>
    public enum MeshRefinementTypes
    {
        /// <summary>
        /// Mesh refinement based on wave Courant numbers,
        /// typically used in coastal and wave modeling applications.
        /// </summary>
        [Description("Wave courant")]
        WaveCourant = 1,

        /// <summary>
        /// Mesh refinement using specified levels to control
        /// resolution across different areas of the mesh.
        /// </summary>
        [Description("Refinement levels")]
        RefinementLevels = 2,

        /// <summary>
        /// Mesh refinement based on ridge detection algorithms,
        /// often used to better capture bathymetric features.
        /// </summary>
        [Description("Ridge detection")]
        RidgeDetection = 3
    }
}