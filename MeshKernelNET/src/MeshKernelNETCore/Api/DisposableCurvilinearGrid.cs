using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableCurvilinearGrid : DisposableMeshObject
    {
        [ProtoMember(1)]
        public int[] edgeNodes;

        [ProtoMember(2)]
        public double[] nodeX;

        [ProtoMember(3)]
        public double[] nodeY;

        [ProtoMember(4)]
        public double[] edgeX;

        [ProtoMember(5)]
        public double[] edgeY;

        [ProtoMember(6)]
        public int numNodes;

        [ProtoMember(7)]
        public int numEdges;

        public DisposableCurvilinearGrid() { }

        public DisposableCurvilinearGrid(int nNodes, int nEdges)
        {
            numNodes = nNodes;
            numEdges = nEdges;

            edgeNodes = new int[numEdges * 2];
            nodeX = new double[numNodes];
            nodeY = new double[numNodes];
            edgeX = new double[numEdges];
            edgeY = new double[numEdges];
        }

        public CurvilinearGridNative CreateCurvilinearGrid()
        {
            if (!IsMemoryPinned)
            {
                PinMemory();
            }

            return new CurvilinearGridNative
            {
                edge_nodes = GetPinnedObjectPointer(edgeNodes),
                node_x = GetPinnedObjectPointer(nodeX),
                node_y = GetPinnedObjectPointer(nodeY),
                edge_x = GetPinnedObjectPointer(edgeX),
                edge_y = GetPinnedObjectPointer(edgeY),
                num_nodes = numNodes,
                num_edges = numEdges,
            };
        }

        ~DisposableCurvilinearGrid()
        {
            Dispose(false);
        }
    }
}
