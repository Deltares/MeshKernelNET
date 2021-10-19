namespace MeshKernelNETCore.Api
{
    public enum AveragingMethod
    {
        SimpleAveraging = 1,
        ClosestPoint = 2,
        Max = 3,
        Min = 4,
        InverseWeightDistance = 5,
        MinAbs = 6,
        KdTree = 7
    };
}