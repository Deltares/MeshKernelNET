using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableMesh1D : DisposableNativeObject<Mesh1DNative>
    {
        [ProtoMember(1)]
        private int[] edgeNodes;

        [ProtoMember(2)]
        private double[] nodeX;

        [ProtoMember(3)]
        private double[] nodeY;

        [ProtoMember(4)]
        private int numNodes;

        [ProtoMember(5)]
        private  int numValidNodes;

        [ProtoMember(6)]
        private int numEdges;

        [ProtoMember(7)]
        private  int numValidEdges;

        public DisposableMesh1D()
        {
        }

        public DisposableMesh1D(int nNodes, int nEdges)
        {
            NumNodes = nNodes;
            NumEdges = nEdges;

            EdgeNodes = new int[NumEdges * 2];
            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
        }

        ~DisposableMesh1D()
        {
            Dispose(false);
        }

        public int[] EdgeNodes
        {
            get { return edgeNodes; }
            set { edgeNodes = value; }
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

        public int NumNodes
        {
            get { return numNodes; }
            set { numNodes = value; }
        }

        public int NumValidNodes
        {
            get { return numValidNodes; }
            set { numValidNodes = value; }
        }

        public int NumEdges
        {
            get { return numEdges; }
            set { numEdges = value; }
        }

        public int NumValidEdges
        {
            get { return numValidEdges; }
            set { numValidEdges = value; }
        }
        
        protected override void SetNativeObject(ref Mesh1DNative nativeObject)
        {
            nativeObject.edge_nodes = GetPinnedObjectPointer(EdgeNodes);
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.num_valid_nodes = numValidNodes;
            nativeObject.num_nodes = NumNodes;
            nativeObject.num_edges = NumEdges;
            nativeObject.num_valid_edges = numValidEdges;
        }
    }
}