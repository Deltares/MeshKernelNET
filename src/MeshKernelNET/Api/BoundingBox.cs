using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public class BoundingBox
    {
        public static BoundingBox CreateDefault()
        {
            return new BoundingBox
            {
                xLowerLeft = double.MinValue,
                yLowerLeft = double.MinValue,
                xUpperRight = double.MaxValue,
                yUpperRight = double.MaxValue
            };
        }

        /// <summary>
        /// The bounding box lower left x coordinate
        /// </summary>
        [ProtoMember(1)]
        public double xLowerLeft { get; set; }

        /// <summary>
        /// The bounding box lower left y coordinate
        /// </summary>
        [ProtoMember(2)]
        public double yLowerLeft { get; set; }

        /// <summary>
        /// The bounding box upper right x coordinate
        /// </summary>
        [ProtoMember(3)]
        public double xUpperRight { get; set; }

        /// <summary>
        /// The bounding box upper right y coordinate
        /// </summary>
        [ProtoMember(4)]
        public double yUpperRight { get; set; }
    }
}