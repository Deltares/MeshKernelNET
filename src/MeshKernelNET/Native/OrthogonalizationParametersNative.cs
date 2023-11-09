namespace MeshKernelNET.Native
{
    /// <summary>
    /// All orthogonalization options
    /// </summary>
    public struct OrthogonalizationParametersNative
    {
        /// <summary>
        /// *Number of outer iterations in orthogonalization. Increase this parameter for complex grids (2)
        /// </summary>
        public int OuterIterations { get; set; }

        /// <summary>
        /// *Number of boundary iterations in grid/net orthogonalization within itatp (25)
        /// </summary>
        public int BoundaryIterations { get; set; }

        /// <summary>
        /// *Number of inner    iterations in grid/net orthogonalization within itbnd (25)
        /// </summary>
        public int InnerIterations { get; set; }

        /// <summary>
        /// *Factor from 0 to 1. between grid smoothing and grid orthogonality (0.975)
        /// </summary>
        public double OrthogonalizationToSmoothingFactor { get; set; }

        /// <summary>
        /// Minimum ATPF on the boundary (1.0)
        /// </summary>
        public double OrthogonalizationToSmoothingFactorAtBoundary { get; set; }

        /// <summary>
        /// Factor between smoother 1d0 and area-homogenizer 0d0 (1.0)
        /// </summary>
        public double ArealToAngleSmoothingFactor { get; set; }
    }
}