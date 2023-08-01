using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{

    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableGriddedSamples : DisposableNativeObject<GriddedSamplesNative>
    {
        [ProtoMember(1)]  private int numX;
        [ProtoMember(2)]  private int numY;
        [ProtoMember(3)]  private double originX;
        [ProtoMember(4)]  private double originY;
        [ProtoMember(5)]  private double cellSize;
        [ProtoMember(6)]  private double[] coordinatesX;
        [ProtoMember(7)]  private double[] coordinatesY;
        [ProtoMember(8)]  private double[] values;
        [ProtoMember(9)]  private double upperRightX;
        [ProtoMember(10)] private double upperRightY;

        public DisposableGriddedSamples() { }

        public DisposableGriddedSamples(int nX, int nY, double orgX, double orgY, double upperRX, double upperRY)
        {
            numX = nX;
            numY = nY;
            originX = orgX;
            originY = orgY;
            upperRightX = upperRX;
            upperRightY = upperRY;

            coordinatesX = new double[numX];
            coordinatesY = new double[numY];
            values = new double[numX * numY];
        }

        ~DisposableGriddedSamples()
        {
            Dispose(false);
        }

        public int NumX
        {
            get { return numX; }
            set { numX = value; }
        }

        public int NumY
        {
            get { return numY; }
            set { numY = value; }
        }

        public double OriginX
        {
            get { return originX; }
            set { originX = value; }
        }

        public double OriginY
        {
            get { return originY; }
            set { originY = value; }
        }

        public double CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        public double[] CoordinatesX
        {
            get { return coordinatesX; }
            set { coordinatesX = value; }
        }

        public double[] CoordinatesY
        {
            get { return coordinatesY; }
            set { coordinatesY = value; }
        }

        public double[] Values
        {
            get { return values; }
            set { values = value; }
        }

        public double UpperRightX
        {
            get { return upperRightX; }
            set { upperRightX = value; }
        }

        public double UpperRightY
        {
            get { return upperRightY; }
            set { upperRightY = value; }
        }

        protected override void SetNativeObject(ref GriddedSamplesNative nativeObject)
        {
            nativeObject.num_x = numX;
            nativeObject.num_y = numY;
            nativeObject.origin_x = originX;
            nativeObject.origin_y = originY;
            nativeObject.cell_size = cellSize;
            nativeObject.coordinates_x = GetPinnedObjectPointer(coordinatesX);
            nativeObject.coordinates_y = GetPinnedObjectPointer(coordinatesY);
            nativeObject.values = GetPinnedObjectPointer(values);
            nativeObject.upper_right_x = upperRightX;
            nativeObject.upper_right_y = upperRightY;
        }
    }
}