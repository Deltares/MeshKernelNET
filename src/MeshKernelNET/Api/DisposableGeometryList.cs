using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableGeometryList : DisposableNativeObject<GeometryListNative>
    {
        [ProtoMember(1)]
        private double geometrySeparator;

        [ProtoMember(2)]
        private double innerOuterSeparator;

        [ProtoMember(3)]
        private int numberOfCoordinates;

        [ProtoMember(4)]
        private double[] xCoordinates;

        [ProtoMember(5)]
        private double[] yCoordinates;

        [ProtoMember(6)]
        private double[] values;

        ~DisposableGeometryList()
        {
            Dispose(false);
        }

        /// <summary>
        /// Separator used for separating multiple geometries
        /// </summary>

        public double GeometrySeparator
        {
            get { return geometrySeparator; }
            set { geometrySeparator = value; }
        }

        /// <summary>
        /// Separator used for separating the outer and inner rings of a polygon geometry
        /// </summary>

        public double InnerOuterSeparator
        {
            get { return innerOuterSeparator; }
            set { innerOuterSeparator = value; }
        }

        /// <summary>
        /// Total number of coordinates (including separators)
        /// </summary>

        public int NumberOfCoordinates
        {
            get { return numberOfCoordinates; }
            set { numberOfCoordinates = value; }
        }

        /// <summary>
        /// The x coordinates of the geometries
        /// </summary>

        public double[] XCoordinates
        {
            get { return xCoordinates; }
            set { xCoordinates = value; }
        }

        /// <summary>
        /// The y coordinates of the geometries
        /// </summary>

        public double[] YCoordinates
        {
            get { return yCoordinates; }
            set { yCoordinates = value; }
        }

        /// <summary>
        /// The values for the coordinates of the geometries
        /// </summary>

        public double[] Values
        {
            get { return values; }
            set { values = value; }
        }

        protected override void SetNativeObject(ref GeometryListNative nativeObject)
        {
            nativeObject.xCoordinates = GetPinnedObjectPointer(XCoordinates);
            nativeObject.yCoordinates = GetPinnedObjectPointer(YCoordinates);
            nativeObject.zCoordinates = GetPinnedObjectPointer(Values);
            nativeObject.numberOfCoordinates = NumberOfCoordinates;
            nativeObject.geometrySeperator = GeometrySeparator;
            nativeObject.innerOuterSeperator = InnerOuterSeparator;
        }
    }
}