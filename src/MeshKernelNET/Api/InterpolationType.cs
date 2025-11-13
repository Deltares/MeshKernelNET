using System.ComponentModel;

namespace MeshKernelNET.Api
{
    public enum InterpolationType
    {
        [Description("Short")]
        Short = 0,

        [Description("Float")]
        Float = 1,

        [Description("Int")]
        Int = 2,

        [Description("Double")]
        Double = 3
    }
}