using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public class MeshRefinementParameters
    {
        public static MeshRefinementParameters CreateDefault()
        {
            return new MeshRefinementParameters
            {
                MaxNumRefinementIterations = 10,
                RefineIntersected = false,
                UseMassCenterWhenRefining = true,
                MinEdgeSize = 0.5,
                RefinementType = 2,
                ConnectHangingNodes = true,
                AccountForSamplesOutside = false,
                SmoothingIterations = 5,
                MaxCourantTime = 120.0,
                DirectionalRefinement = false
            };
        }

        /// <summary>
        /// Maximum number of refinement iterations, set to 1 if only one refinement is wanted (10)
        /// </summary>
        [ProtoMember(1)]
        public int MaxNumRefinementIterations { get; set; }

        /// <summary>
        /// Whether to compute faces intersected by polygon (yes=1/no=0)
        /// </summary>
        [ProtoMember(2)]
        public bool RefineIntersected { get; set; }

        /// <summary>
        /// Whether to use the mass center when splitting a face in the refinement process (yes=1/no=0)
        /// </summary>
        [ProtoMember(3)]
        public bool UseMassCenterWhenRefining { get; set; }

        /// <summary>
        /// Minimum edge size
        /// </summary>
        [ProtoMember(4)]
        public double MinEdgeSize { get; set; }

        /// <summary>
        /// Refinement criterion type
        /// </summary>
        [ProtoMember(5)]
        public int RefinementType { get; set; }

        /// <summary>
        /// Connect hanging nodes at the end of the iteration, 1 yes or 0 no
        /// </summary>
        [ProtoMember(6)]
        public bool ConnectHangingNodes { get; set; }

        /// <summary>
        /// Take samples outside face into account , 1 yes 0 no
        /// </summary>
        [ProtoMember(7)]
        public bool AccountForSamplesOutside { get; set; }

        /// <summary>
        /// The number of smoothing iterations
        /// </summary>
        [ProtoMember(8)]
        public int SmoothingIterations { get; set; }

        /// <summary>
        /// Maximum courant time in seconds
        /// </summary>
        [ProtoMember(9)]
        public double MaxCourantTime { get; set; }

        /// <summary>
        /// Directional refinement, cannot be used when the number of smoothing iterations is larger than 0
        /// </summary>
        [ProtoMember(10)]
        public bool DirectionalRefinement { get; set; }
    }
}
