namespace MeshKernelNETCore.Native
{
    /// <summary>
    /// A struct used to describe the mesh refinement parameters in a C-compatible manner
    /// </summary>
    public struct MeshRefinementParametersNative
    {
        /// <summary>
        /// Maximum number of refinement iterations, set to 1 if only one refinement is wanted (10)
        /// </summary>
        public int MaxNumRefinementIterations { get; set; }

        /// <summary>
        /// Whether to compute faces intersected by polygon (yes=1/no=0)
        /// </summary>
        public int RefineIntersected { get; set; }

        /// <summary>
        /// Whether to use the mass center when splitting a face in the refinement process (yes=1/no=0)
        /// </summary>
        public int UseMassCenterWhenRefining { get; set; }

        /// <summary>
        /// Minimum cell size
        /// </summary>
        public double MinFaceSize { get; set; }

        /// <summary>
        /// Refinement criterion type
        /// </summary>
        public int RefinementType { get; set; }

        /// <summary>
        /// Connect hanging nodes at the end of the iteration, 1 yes or 0 no
        /// </summary>
        public int ConnectHangingNodes { get; set; }

        /// <summary>
        /// Take samples outside face into account , 1 yes 0 no
        /// </summary>
        public int AccountForSamplesOutside { get; set; }
    }
}
