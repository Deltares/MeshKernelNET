using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableSplineIntersections : DisposableNativeObject<SplineIntersectionsNative>
    {
        [ProtoMember(1)]
        private int numIntersections;

        [ProtoMember(2)]
        private int[] splineIndex;

        [ProtoMember(3)]
        private double[] intersectionAngle;

        [ProtoMember(4)]
        private double[] intersectionX;

        [ProtoMember(5)]
        private double[] intersectionY;

        public DisposableSplineIntersections()
        {
        }

        public DisposableSplineIntersections(int numIntersections)
        {
            this.numIntersections = numIntersections;
            splineIndex = new int[numIntersections];
            intersectionAngle = new double[numIntersections];
            intersectionX = new double[numIntersections];
            intersectionY = new double[numIntersections];
        }

        ~DisposableSplineIntersections()
        {
            Dispose(false);
        }

        public int NumIntersections
        {
            get => numIntersections;
            set => numIntersections = value;
        }

        public int[] SplineIndex
        {
            get => splineIndex;
            set => splineIndex = value;
        }

        public double[] IntersectionAngle
        {
            get => intersectionAngle;
            set => intersectionAngle = value;
        }

        public double[] IntersectionX
        {
            get => intersectionX;
            set => intersectionX = value;
        }

        public double[] IntersectionY
        {
            get => intersectionY;
            set => intersectionY = value;
        }

        protected override void SetNativeObject(ref SplineIntersectionsNative nativeObject)
        {
            nativeObject.NumIntersections = numIntersections;
            nativeObject.SplineIndex = GetPinnedObjectPointer(splineIndex);
            nativeObject.IntersectionAngle = GetPinnedObjectPointer(intersectionAngle);
            nativeObject.IntersectionX = GetPinnedObjectPointer(intersectionX);
            nativeObject.IntersectionY = GetPinnedObjectPointer(intersectionY);
        }

        public void UpdateFromNativeObject(ref SplineIntersectionsNative nativeObject)
        {
            numIntersections = nativeObject.NumIntersections;
            splineIndex = nativeObject.SplineIndex.CreateValueArray<int>(nativeObject.NumIntersections);
            intersectionAngle = nativeObject.IntersectionAngle.CreateValueArray<double>(nativeObject.NumIntersections);
            intersectionX = nativeObject.IntersectionX.CreateValueArray<double>(nativeObject.NumIntersections);
            intersectionY = nativeObject.IntersectionY.CreateValueArray<double>(nativeObject.NumIntersections);
        }
    }
}