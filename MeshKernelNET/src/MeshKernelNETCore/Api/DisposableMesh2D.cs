using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{

    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableMesh2D : DisposableNativeObject<Mesh2DNative>
    {
        [ProtoMember(1)] private int[] edgeNodes;
        [ProtoMember(2)] private int[] faceNodes;
        [ProtoMember(3)] private int[] nodesPerFace;
        [ProtoMember(4)] private double[] nodeX;
        [ProtoMember(5)] private double[] nodeY;
        [ProtoMember(6)] private double[] edgeX;
        [ProtoMember(7)] private double[] edgeY;
        [ProtoMember(8)] private double[] faceX;
        [ProtoMember(9)] private double[] faceY;
        [ProtoMember(10)] private int numNodes;
        [ProtoMember(11)] private int numEdges;
        [ProtoMember(12)] private int numFaces;
        [ProtoMember(13)] private int numFaceNodes;

        public DisposableMesh2D() { }

        public DisposableMesh2D(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            NumNodes = nNodes;
            NumEdges = nEdges;
            NumFaces = nFaces;
            NumFaceNodes = nFaceNodes;

            EdgeNodes = new int[NumEdges * 2];
            FaceNodes = new int[NumFaceNodes];
            NodesPerFace = new int[NumFaces];
            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
            EdgeX = new double[NumEdges];
            EdgeY = new double[NumEdges];
            FaceX = new double[NumFaces];
            FaceY = new double[NumFaces];
        }

        ~DisposableMesh2D()
        {
            Dispose(false);
        }

        public int[] EdgeNodes
        {
            get { return edgeNodes; }
            set { edgeNodes = value; }
        }

        public int[] FaceNodes
        {
            get { return faceNodes; }
            set { faceNodes = value; }
        }

        public int[] NodesPerFace
        {
            get { return nodesPerFace; }
            set { nodesPerFace = value; }
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

        public double[] FaceX
        {
            get { return faceX; }
            set { faceX = value; }
        }

        public double[] FaceY
        {
            get { return faceY; }
            set { faceY = value; }
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

        public int NumFaces
        {
            get { return numFaces; }
            set { numFaces = value; }
        }

        public int NumFaceNodes
        {
            get { return numFaceNodes; }
            set { numFaceNodes = value; }
        }

        protected override void SetNativeObject(ref Mesh2DNative nativeObject)
        {
            nativeObject.edge_nodes = GetPinnedObjectPointer(EdgeNodes);
            nativeObject.face_nodes = GetPinnedObjectPointer(FaceNodes);
            nativeObject.nodes_per_face = GetPinnedObjectPointer(NodesPerFace);
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.edge_x = GetPinnedObjectPointer(EdgeX);
            nativeObject.edge_y = GetPinnedObjectPointer(EdgeY);
            nativeObject.face_x = GetPinnedObjectPointer(FaceX);
            nativeObject.face_y = GetPinnedObjectPointer(FaceY);
            nativeObject.num_nodes = NumNodes;
            nativeObject.num_edges = NumEdges;
            nativeObject.num_faces = NumFaces;
            nativeObject.num_face_nodes = NumFaceNodes;
        }
    }

}
