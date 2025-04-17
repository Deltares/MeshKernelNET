using System.ComponentModel;

namespace MeshKernelNET.Api
{
    public enum MeshRefinementTypes
    {
        [Description("Wave courant")]
        WaveCourant = 1,

        [Description("Refinement levels")]
        RefinementLevels = 2,

        [Description("Ridge detection")]
        RidgeDetection = 3
    }
}