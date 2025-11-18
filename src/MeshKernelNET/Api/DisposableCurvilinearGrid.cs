using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableCurvilinearGrid : DisposableNativeObject<CurvilinearGridNative>, IReadOnly2DMesh
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

        public DisposableCurvilinearGrid(int nN, int nM)
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

        #region IReadOnly2DMesh
        /// <inheritdoc/>
        public int CellCount()
        {
            return (NumM - 1) * (NumN-1);
        }

        /// <inheritdoc/>
        /// <remarks>Note that there is no trivial relationship between cell index and edge indexes</remarks>
        public int GetCellEdgeCount(int cellIndex)
        {
            return 4;
        }

        /// <inheritdoc/>
        public int EdgeCount()
        {
            return (NumM-1)*NumN +   // row-oriented edges 
                   NumM*(NumN - 1); // column-oriented edges
        }

        /// <remarks>The edge numbering is equal to CurvilinearGrid::ConvertCurvilinearToNodesAndEdges.
        /// First column-oriented edges, then row-oriented edges, in row major order</remarks>
        private (int first, int last) GetEdgeNodes(int edgeIndex)
        {
            int numNodesInRow = NumM;
            int numNodesInColumn = NumN;
            int numColumnOrientedEdges = numNodesInRow * (numNodesInColumn - 1);             
            if (edgeIndex < numColumnOrientedEdges)
            {
                return (edgeIndex, edgeIndex + numNodesInRow);
            }

            edgeIndex -= numColumnOrientedEdges;

            int numRowOrientedEdgesInRow = (numNodesInRow - 1); 
            int row = edgeIndex / numRowOrientedEdgesInRow;
            int first = row*numNodesInRow + edgeIndex % numRowOrientedEdgesInRow;
            return (first, first + 1);
        }
        
        /// <inheritdoc/>
        public int GetFirstNode(int edgeIndex)
        {
            return GetEdgeNodes(edgeIndex).first;
        }

        /// <inheritdoc/>
        public int GetLastNode(int edgeIndex)
        {
            return GetEdgeNodes(edgeIndex).last;
        }

        /// <inheritdoc/>
        public int NodeCount()
        {
            return NumM*NumN;
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

        protected override void SetNativeObject(ref CurvilinearGridNative nativeObject)
        {
            nativeObject.node_x = GetPinnedObjectPointer(NodeX);
            nativeObject.node_y = GetPinnedObjectPointer(NodeY);
            nativeObject.num_m = numM;
            nativeObject.num_n = numN;
        }

        public void UpdateFromNativeObject(ref CurvilinearGridNative nativeObject)
        {
            NumM = nativeObject.num_m;
            NumN = nativeObject.num_n;
            NodeX = nativeObject.node_x.CreateValueArray<double>(nativeObject.num_m * nativeObject.num_n);
            NodeY = nativeObject.node_y.CreateValueArray<double>(nativeObject.num_m * nativeObject.num_n);
        }
    }
}