using System;
using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableGriddedSamples : DisposableNativeObject<GriddedSamplesNative>
    {
        [ProtoMember(1)]
        private int numX;

        [ProtoMember(2)]
        private int numY;

        [ProtoMember(3)]
        private double originX;

        [ProtoMember(4)]
        private double originY;

        [ProtoMember(5)]
        private double cellSize;

        [ProtoMember(6)]
        private double[] coordinatesX;

        [ProtoMember(7)]
        private double[] coordinatesY;

        [ProtoMember(8)]
        private short[] shortValues;

        [ProtoMember(9)]
        private float[] floatValues;

        [ProtoMember(10)]
        private int[] intValues;

        [ProtoMember(11)]
        private double[] doubleValues;

        [ProtoMember(12)]
        private InterpolationType valueType;

        public DisposableGriddedSamples()
        {
        }

        public DisposableGriddedSamples(int nX, int nY, double orgX, double orgY, double cSize, InterpolationType type)
        {
            numX = nX;
            numY = nY;
            originX = orgX;
            originY = orgY;
            cellSize = cSize;
            coordinatesX = new double[numX];
            coordinatesY = new double[numY];
            valueType = type;

            int length = numX * numY;

            switch (type)
            {
                case InterpolationType.Short:
                    shortValues = new short[length];
                    break;
                case InterpolationType.Float:
                    floatValues = new float[length];
                    break;
                case InterpolationType.Int:
                    intValues = new int[length];
                    break;
                case InterpolationType.Double:
                    doubleValues = new double[length];
                    break;
                default:
                    throw new ArgumentException($"Unsupported interpolation type: {type}", nameof(type));
            }
        }

        ~DisposableGriddedSamples()
        {
            Dispose(false);
        }

        public int NumX
        {
            get => numX;
            set => numX = value;
        }

        public int NumY
        {
            get => numY;
            set => numY = value;
        }

        public double OriginX
        {
            get => originX;
            set => originX = value;
        }

        public double OriginY
        {
            get => originY;
            set => originY = value;
        }

        public double CellSize
        {
            get => cellSize;
            set => cellSize = value;
        }

        public double[] CoordinatesX
        {
            get => coordinatesX;
            set => coordinatesX = value;
        }

        public double[] CoordinatesY
        {
            get => coordinatesY;
            set => coordinatesY = value;
        }

        public InterpolationType ValueType
        {
            get => valueType;
            set => valueType = value;
        }

        public short[] ShortValues
        {
            get => shortValues;
            set => shortValues = value;
        }

        public float[] FloatValues
        {
            get => floatValues;
            set => floatValues = value;
        }

        public int[] IntValues
        {
            get => intValues;
            set => intValues = value;
        }
        
        public double[] DoubleValues
        {
            get => doubleValues;
            set => doubleValues = value;
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
            nativeObject.value_type = (int)valueType;

            switch (valueType)
            {
                case InterpolationType.Short:
                    nativeObject.values = GetPinnedObjectPointer(shortValues);
                    break;
                case InterpolationType.Float:
                    nativeObject.values = GetPinnedObjectPointer(floatValues);
                    break;
                case InterpolationType.Int:
                    nativeObject.values = GetPinnedObjectPointer(intValues);
                    break;
                case InterpolationType.Double:
                    nativeObject.values = GetPinnedObjectPointer(doubleValues);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported value type: {valueType}");
            }
        }
    }
}