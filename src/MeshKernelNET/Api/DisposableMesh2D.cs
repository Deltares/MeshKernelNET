using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableMesh2D : DisposableNativeObject<Mesh2DNative>, IReadOnly2DMesh
    {
        [ProtoMember(1)]
        private int[] edgeFaces;

        [ProtoMember(2)]
        private int[] edgeNodes;

        [ProtoMember(3)]
        private int[] faceEdges;

        [ProtoMember(4)]
        private int[] faceNodes;

        [ProtoMember(5)]
        private int[] nodesPerFace;

        [ProtoMember(6)]
        private double[] nodeX;

        [ProtoMember(7)]
        private double[] nodeY;

        [ProtoMember(8)]
        private double[] edgeX;

        [ProtoMember(9)]
        private double[] edgeY;

        [ProtoMember(10)]
        private double[] faceX;

        [ProtoMember(11)]
        private double[] faceY;

        [ProtoMember(12)]
        private int numNodes;

        [ProtoMember(13)]
        private int numValidNodes;

        [ProtoMember(14)]
        private int numEdges;

        [ProtoMember(15)]
        private int numValidEdges;

        [ProtoMember(16)]
        private int numFaces;

        [ProtoMember(17)]
        private int numFaceNodes;

        public DisposableMesh2D()
        {
        }

        public DisposableMesh2D(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            NumNodes = nNodes;
            NumEdges = nEdges;
            NumFaces = nFaces;
            NumFaceNodes = nFaceNodes;



            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
            EdgeNodes = new int[NumEdges * 2];


            EdgeFaces = new int[NumEdges * 2];
            FaceEdges = new int[NumFaceNodes];
            EdgeX = new double[NumEdges];
            EdgeY = new double[NumEdges];
            FaceNodes = new int[NumFaceNodes];
            NodesPerFace = new int[NumFaces];
            FaceX = new double[NumFaces];
            FaceY = new double[NumFaces];
        }

        ~DisposableMesh2D()
        {
            Dispose(false);
        }

        public int[] EdgeFaces
        {
            get { return edgeFaces; }
            set { edgeFaces = value; }
        }

        public int[] EdgeNodes
        {
            get { return edgeNodes; }
            set { edgeNodes = value; }
        }

        public int[] FaceEdges
        {
            get { return faceEdges; }
            set { faceEdges = value; }
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

        #region IReadOnly2DMesh
        /// <inheritdoc/>
        public int CellCount()
        {
            return NumFaces;
        }

        /// <inheritdoc/>
        public int GetCellEdgeCount(int cellIndex)
        {
            return NodesPerFace[cellIndex];
        }

        /// <inheritdoc/>
        public int EdgeCount()
        {
            return NumEdges;
        }

        /// <inheritdoc/>
        public int GetFirstNode(int edgeIndex)
        {
            return EdgeNodes[2 * edgeIndex];
        }

        /// <inheritdoc/>
        public int GetLastNode(int edgeIndex)
        {
            return EdgeNodes[2 * edgeIndex + 1];
        }

        /// <inheritdoc/>
        public int NodeCount()
        {
            return NumNodes;
        }

        /// <inheritdoc/>
        public double GetNodeX(int nodeIndex)
        {
            return NodeX[nodeIndex];
        }

        /// <inheritdoc/>
        public double GetNodeY(int nodeIndex)
        {
            return NodeY[nodeIndex];
        }
        #endregion

        protected override void SetNativeObject(ref Mesh2DNative nativeObject)
        {
            nativeObject.edge_faces = GetPinnedObjectPointer(EdgeFaces);
            nativeObject.edge_nodes = GetPinnedObjectPointer(EdgeNodes);
            nativeObject.face_edges = GetPinnedObjectPointer(FaceEdges);
            nativeObject.face_nodes = GetPinnedObjectPointer(FaceNodes);
            nativeObject.nodes_per_face = GetPinnedObjectPointer(NodesPerFace);
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.edge_x = GetPinnedObjectPointer(EdgeX);
            nativeObject.edge_y = GetPinnedObjectPointer(EdgeY);
            nativeObject.face_x = GetPinnedObjectPointer(FaceX);
            nativeObject.face_y = GetPinnedObjectPointer(FaceY);
            nativeObject.num_nodes = NumNodes;
            nativeObject.num_valid_nodes = NumValidNodes;
            nativeObject.num_edges = NumEdges;
            nativeObject.num_valid_edges = NumValidEdges;
            nativeObject.num_faces = NumFaces;
            nativeObject.num_face_nodes = NumFaceNodes;
        }
    }
}