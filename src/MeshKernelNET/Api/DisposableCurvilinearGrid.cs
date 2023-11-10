using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableCurvilinearGrid : DisposableNativeObject<CurvilinearGridNative>
    {
        [ProtoMember(1)]
        private double[] nodeX;

        [ProtoMember(2)]
        private double[] nodeY;

        [ProtoMember(3)]
        private int numM;

        [ProtoMember(4)]
        private int numN;

        public DisposableCurvilinearGrid()
        {
        }

        public DisposableCurvilinearGrid(int nM, int nN)
        {
            numM = nM;
            numN = nN;
            int NumNodes = numM * numN;

            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
        }

        ~DisposableCurvilinearGrid()
        {
            Dispose(false);
        }

        public double[] NodeX
        {
            get { return nodeX; }
            set { nodeX = value; }
        }

        public double[] NodeY
        {
            get { return nodeY; }
            set { nodeY = value; }
        }

        public int NumM
        {
            get { return numM; }
            set { numM = value; }
        }

        public int NumN
        {
            get { return numN; }
            set { numN = value; }
        }

        protected override void SetNativeObject(ref CurvilinearGridNative nativeObject)
        {
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.num_m = numM;
            nativeObject.num_n = numN;
        }
    }
}