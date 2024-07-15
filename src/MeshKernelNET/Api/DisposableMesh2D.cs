using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            NumNodes = 0;
            NumEdges = 0;
            NumFaces = 0;
            NumFaceNodes = 0;
        }

        public DisposableMesh2D(DisposableMesh2D source)
        {
            NumNodes = source.NumNodes;
            NumEdges = source.NumEdges;
            NumFaces = source.NumFaces;
            NumFaceNodes = source.NumFaceNodes;


            edgeFaces = (int[])source.edgeFaces?.Clone();
            edgeNodes = (int[])source.edgeNodes?.Clone();
            faceEdges = (int[])source.faceEdges?.Clone();
            faceNodes = (int[])source.faceNodes?.Clone();
            nodesPerFace = (int[])source.nodesPerFace?.Clone();
            nodeX = (double[])source.nodeX?.Clone();
            nodeY = (double[])source.nodeY?.Clone();
            edgeX = (double[])source.edgeX?.Clone();
            edgeY = (double[])source.edgeY?.Clone();
            faceX = (double[])source.faceX?.Clone();
            faceY = (double[])source.faceY?.Clone();
            numValidNodes = source.numValidNodes;
            numValidEdges = source.numValidEdges;
        }

        public DisposableMesh2D(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            NumNodes = nNodes;
            NumEdges = nEdges;
            NumFaces = nFaces;
            NumFaceNodes = nFaceNodes;

            EdgeFaces = new int[NumEdges * 2];
            EdgeNodes = new int[NumEdges * 2];
            FaceEdges = new int[NumFaceNodes];
            FaceNodes = new int[NumFaceNodes];
            NodesPerFace = new int[NumFaces];
            NodeX = new double[NumNodes];
            NodeY = new double[NumNodes];
            EdgeX = new double[NumEdges];
            EdgeY = new double[NumEdges];
            FaceX = new double[NumFaces];
            FaceY = new double[NumFaces];
        }

        private bool HasChanged(int nNodes, int nEdges, int nFaces)
        {
            if (nNodes > NumNodes)
            {
                return true;
            }
            if (nEdges > NumEdges)
            {
                return true;
            }
            if (nFaces > NumFaces)
            {
                return true;
            }

            return false;
        }


        public void Resize(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            bool hasChanged = HasChanged(nNodes, nEdges, nFaces);
            if (hasChanged)
            {
                UnPinMemory();
            }

            if (nNodes > NumNodes)
            {
                double[] newNodeX = new double[nNodes];
                double[] newNodeY = new double[nNodes];
                if (NodeX != null)
                {
                    Array.Copy(NodeY, newNodeY, NumNodes);
                }
                if (NodeY != null)
                {
                    Array.Copy(NodeY, newNodeY, NumNodes);
                }
                NodeX = newNodeX;
                NodeY = newNodeY;
                NumNodes = nNodes;
            }

            if (nEdges > NumEdges)
            {
                double[] newEdgeX = new double[nEdges];
                double[] newEdgeY = new double[nEdges];
                int[] newEdgeFaces = new int[nEdges * 2];
                int[] newEdgeNodes = new int[nEdges * 2];
                if (EdgeX != null)
                {
                    Array.Copy(EdgeX, newEdgeX, NumEdges);
                }
                if (EdgeY != null)
                {
                    Array.Copy(EdgeY, newEdgeY, NumEdges);
                }
                if (EdgeFaces != null)
                {
                    Array.Copy(EdgeFaces, newEdgeFaces, NumEdges * 2);
                }
                if (EdgeNodes != null)
                {
                    Array.Copy(EdgeNodes, newEdgeNodes, NumEdges * 2);
                }
                EdgeX = newEdgeX;
                EdgeY = newEdgeY;
                EdgeFaces = newEdgeFaces;
                EdgeNodes = newEdgeNodes;
                NumEdges = nEdges;
            }

            if (nFaces > NumFaces)
            {
                double[] newFaceX = new double[nFaces];
                double[] newFaceY = new double[nFaces];
                int[] newNodesPerFace = new int[nFaces];
                if (FaceX != null)
                {
                    Array.Copy(FaceX, newFaceX, NumFaces);
                }
                if (FaceY != null)
                {
                    Array.Copy(FaceY, newFaceY, NumFaces);
                }
                if (NodesPerFace != null)
                {
                    Array.Copy(NodesPerFace, newNodesPerFace, NumFaces);
                }
                FaceX = newFaceX;
                FaceY = newFaceY;
                NodesPerFace = newNodesPerFace;
                NumFaces = nFaces;
            }

            if (nFaceNodes > NumFaceNodes)
            {
                int[] newFaceEdges = new int[nFaceNodes];
                int[] newFaceNodes = new int[nFaceNodes];
                if (FaceEdges != null)
                {
                    Array.Copy(FaceEdges, newFaceEdges, NumFaceNodes);
                }
                if (FaceNodes != null)
                {
                    Array.Copy(FaceNodes, newFaceNodes, NumFaceNodes);
                }
                
                FaceEdges = newFaceEdges;
                FaceNodes = newFaceNodes;
                NumFaceNodes = nFaceNodes;
            }

            if (hasChanged)
            {
                PinMemory();
            }
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
            nativeObject.edge_faces = GetPinnedObjectPointer(0);
            nativeObject.edge_nodes = GetPinnedObjectPointer(1);
            nativeObject.face_edges = GetPinnedObjectPointer(2);
            nativeObject.face_nodes = GetPinnedObjectPointer(3);
            nativeObject.nodes_per_face = GetPinnedObjectPointer(4);
            nativeObject.node_x = GetPinnedObjectPointer(5);
            nativeObject.node_y = GetPinnedObjectPointer(6);
            nativeObject.edge_x = GetPinnedObjectPointer(7);
            nativeObject.edge_y = GetPinnedObjectPointer(8);
            nativeObject.face_x = GetPinnedObjectPointer(9);
            nativeObject.face_y = GetPinnedObjectPointer(10);

            nativeObject.num_nodes = NumNodes;
            nativeObject.num_valid_nodes = NumValidNodes;
            nativeObject.num_edges = NumEdges;
            nativeObject.num_valid_edges = NumValidEdges;
            nativeObject.num_faces = NumFaces;
            nativeObject.num_face_nodes = NumFaceNodes;
        }
    }
}