namespace MeshKernelNET.Api
{
    /// <summary>
    /// Extends the general 2d mesh interface with curvilinear mesh methods
    /// </summary>
    public interface IReadOnlyCurvilinearGrid : IReadOnly2DMesh
    {
        /// <summary>
        /// Determine the (n,m) index pair from the vertex index, where n is the row number and m is the column number
        /// </summary>
        /// <param name="vertex">an index in the range [0, NumM*NumN - 1]</param>
        /// <returns>the index pair</returns>
        (int n, int m) VertexIndexToCurvilinearIndexPair(int vertex);

        /// <summary>
        /// Determine the vertex index from an (n,m) index pair, where n is the row number and m is the column number
        /// </summary>
        /// <param name="n">row number the range [0, NumN - 1]</param>
        /// <param name="m">column number the range [0, NumM - 1]</param>
        /// <returns>the vertex index</returns>
        int VertexIndexFromCurvilinearIndexPair(int n, int m);
    }
}