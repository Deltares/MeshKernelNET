using ProtoBuf;

namespace MeshKernelNETCore.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public class MeshRefinementParameters
    {
        public static MeshRefinementParameters CreateDefault()
        {
            return new MeshRefinementParameters
            {
                MaxNumRefinementIterations = 3,
                RefineIntersected = 0,
                RefinementType = 2
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
        public int RefineIntersected { get; set; }

        /// <summary>
        /// Whether to use the mass center when splitting a face in the refinement process (yes=1/no=0)
        /// </summary>
        [ProtoMember(3)]
        public int UseMassCenterWhenRefining { get; set; }

        /// <summary>
        /// Minimum cell size
        /// </summary>
        [ProtoMember(4)]
        public double MinFaceSize { get; set; }

        /// <summary>
        /// Refinement criterion type
        /// </summary>
        [ProtoMember(5)]
        public int RefinementType { get; set; }

        /// <summary>
        /// Connect hanging nodes at the end of the iteration, 1 yes or 0 no
        /// </summary>
        [ProtoMember(6)]
        public int ConnectHangingNodes { get; set; }

        /// <summary>
        /// Take samples outside face into account , 1 yes 0 no
        /// </summary>
        [ProtoMember(7)]
        public int AccountForSamplesOutside { get; set; }
    }
}
