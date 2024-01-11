namespace MeshKernelNET.Api
{
    /// <summary>
    /// Topology and node locations of a general 2D mesh
    /// </summary>
    public interface IReadOnly2DMesh
    {
        /// <summary>
        /// The number of cells
        /// </summary>
        /// <returns>a nonnegative number representing the number of cells</returns>
        int CellCount();

        /// <summary>
        /// The number of edges of a cell
        /// </summary>
        /// <param name="cellIndex">the index of the cell</param>
        /// <returns>a nonnegative number representing the number of edges of the cell</returns>
        int GetCellEdgeCount(int cellIndex);

        /// <summary>
        /// The total number of edges in the mesh
        /// </summary>
        /// <returns>a nonnegative number representing the number of edges</returns>
        int EdgeCount();

        /// <summary>
        /// The index of the first node of an edge
        /// </summary>
        /// <returns>a nonnegative number representing the node index</returns>
        int GetFirstNode(int edgeIndex);

        /// <summary>
        /// The index of the second node of an edge
        /// </summary>
        /// <param name="edgeIndex">the index of the cell</param>
        /// <returns>a nonnegative number representing the node index</returns>
        int GetLastNode(int edgeIndex);
        
        
        /// <summary>
        /// The total number of nodes in the mesh
        /// </summary>
        /// <returns>a nonnegative number representing the number of nodes</returns>
        int NodeCount();

        /// <summary>
        /// The x coordinate of a node
        /// </summary>
        /// <param name="nodeIndex">the index of the cell</param>
        /// <returns>the x-coordinate value of the node</returns>
        double GetNodeX(int nodeIndex);

        /// <summary>
        /// The y coordinate of a node
        /// </summary>
        /// <param name="nodeIndex">the index of the cell</param>
        /// <returns>the y-coordinate value of the node</returns>
        double GetNodeY(int nodeIndex);
    }
}
