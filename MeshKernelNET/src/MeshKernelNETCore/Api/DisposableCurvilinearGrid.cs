using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableCurvilinearGrid : DisposableNativeObject<CurvilinearGridNative>
    {
        [ProtoMember(1)] private int[] edgeNodes;
        [ProtoMember(2)] private double[] nodeX;
        [ProtoMember(3)] private double[] nodeY;
        [ProtoMember(4)] private double[] edgeX;
        [ProtoMember(5)] private double[] edgeY;
        [ProtoMember(6)] private int numNodes;
        [ProtoMember(7)] private int numEdges;

        public DisposableCurvilinearGrid() { }

        public DisposableCurvilinearGrid(int nNodes, int nEdges)
        {
            NumNodes = nNodes;
            NumEdges = nEdges;

            EdgeNodes = new int[NumEdges * 2];
            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
            EdgeX = new double[NumEdges];
            EdgeY = new double[NumEdges];
        }

        ~DisposableCurvilinearGrid()
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

        public double[] EdgeX
        {
            get { return edgeX; }
            set { edgeX = value; }
        }

        public double[] EdgeY
        {
            get { return edgeY; }
            set { edgeY = value; }
        }

        public int NumNodes
        {
            get { return numNodes; }
            set { numNodes = value; }
        }

        public int NumEdges
        {
            get { return numEdges; }
            set { numEdges = value; }
        }

        protected override void SetNativeObject(ref CurvilinearGridNative nativeObject)
        {
            nativeObject.edge_nodes = GetPinnedObjectPointer(EdgeNodes);
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.edge_x = GetPinnedObjectPointer(EdgeX);
            nativeObject.edge_y = GetPinnedObjectPointer(EdgeY);
            nativeObject.num_nodes = NumNodes;
            nativeObject.num_edges = NumEdges;
        }
    }
}
