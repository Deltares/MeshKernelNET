using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableMesh1D : DisposableMeshObject
    {
        [ProtoMember(1)]
        public int[] edgeNodes;

        [ProtoMember(2)]
        public double[] nodeX;

        [ProtoMember(3)]
        public double[] nodeY;

        [ProtoMember(4)]
        public int numNodes;

        [ProtoMember(5)]
        public int numEdges;

        public DisposableMesh1D() { }

        public DisposableMesh1D(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            numNodes = nNodes;
            numEdges = nEdges;

            edgeNodes = new int[numEdges * 2];
            nodeX = new double[numNodes];
            nodeY = new double[numNodes];
        }

        public Mesh1DNative CreateMesh1D()
        {
            if (!IsMemoryPinned)
            {
                PinMemory();
            }

            return new Mesh1DNative
            {
                edge_nodes = GetPinnedObjectPointer(edgeNodes),
                node_x = GetPinnedObjectPointer(nodeX),
                node_y = GetPinnedObjectPointer(nodeY),
                num_nodes = numNodes,
                num_edges = numEdges
            };
        }

        ~DisposableMesh1D()
        {
            Dispose(false);
        }
    }
}