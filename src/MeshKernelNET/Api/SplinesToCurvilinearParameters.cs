using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public class SplinesToCurvilinearParameters
    {
        public static SplinesToCurvilinearParameters CreateDefault()
        {
            return new SplinesToCurvilinearParameters
            {
                AspectRatio = 0.1,
                AspectRatioGrowFactor = 1.1,
                AverageWidth = 500.0,
                CurvatureAdaptedGridSpacing = true,
                GrowGridOutside = true,
                MaximumNumberOfGridCellsInTheUniformPart = 5,
                GridsOnTopOfEachOtherTolerance = 0.0001,
                MinimumCosineOfCrossingAngles = 0.95,
                CheckFrontCollisions = false,
                UniformGridSize = 0.0,
                RemoveSkinnyTriangles = true,
            };
        }

        /// <summary>
        /// * Aspect ratio (0.1)
        /// </summary>
        [ProtoMember(1)]
        public double AspectRatio { get; set; }

        /// <summary>
        /// * Grow factor of aspect ratio (1.1)
        /// </summary>
        [ProtoMember(2)]
        public double AspectRatioGrowFactor { get; set; }

        /// <summary>
        /// * Average mesh width on center spline (0.005)
        /// </summary>
        [ProtoMember(3)]
        public double AverageWidth { get; set; }

        /// <summary>
        /// * Curvature adapted grid spacing, 1 or not 0 (1)
        /// </summary>
        [ProtoMember(4)]
        public bool CurvatureAdaptedGridSpacing { get; set; }

        /// <summary>
        /// * Grow the grid outside the prescribed grid height (1)
        /// </summary>
        [ProtoMember(5)]
        public bool GrowGridOutside { get; set; }

        /// <summary>
        /// * Maximum number of layers in the uniform part (5)
        /// </summary>
        [ProtoMember(6)]
        public int MaximumNumberOfGridCellsInTheUniformPart { get; set; }

        /// <summary>
        /// * On-top-of-each-other tolerance (0.0001)
        /// </summary>
        [ProtoMember(7)]
        public double GridsOnTopOfEachOtherTolerance { get; set; }

        /// <summary>
        /// * Minimum allowed absolute value of crossing-angle cosine (0.95)
        /// </summary>
        [ProtoMember(8)]
        public double MinimumCosineOfCrossingAngles { get; set; }

        /// <summary>
        /// * Check for collisions with other parts of the front, 1 or not 0 (0)
        /// </summary>
        [ProtoMember(9)]
        public bool CheckFrontCollisions { get; set; }

        /// <summary>
        /// * Uniform grid size, netboundary to grid only (0.0)
        /// </summary>
        [ProtoMember(10)]
        public double UniformGridSize { get; set; }

        /// <summary>
        /// * Remove skinny triangles (1)
        /// </summary>
        [ProtoMember(11)]
        public bool RemoveSkinnyTriangles { get; set; }
    }
}