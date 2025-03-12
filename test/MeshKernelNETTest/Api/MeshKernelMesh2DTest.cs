using NUnit.Framework;

// Added alias to still be able to use the classic assert functions
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class MeshKernelMesh2DTest
    {
        [Test]
        public void Mesh2D_GetNodeXY_AccessesNodeXY()
        {
            int numRows = 3;
            int numColumns = 4;
            using (var mesh = TestUtilityFunctions.CreateMesh2D(numRows,numColumns, 15.0, 4.0, 2.0, 3.0))
            {
                for (int row = 0; row < numRows; ++row)
                {
                    for (int column = 0; column < numColumns; ++column)
                    {
                        int index = row * numColumns + column;
                        Assert.AreEqual(mesh.GetNodeX(index), mesh.NodeX[index]);
                        Assert.AreEqual(mesh.GetNodeY(index), mesh.NodeY[index]);
                    }
                }
            }
        }
        
        [TestCase(5,3,5*3)]
        [TestCase(15,42,15*42)]
        public void Mesh2D_GetNodeCount_CountsAllNodes(int nodesInRow, int nodesInColumn, int count)
        {
            using (var grid = TestUtilityFunctions.CreateMesh2D(nodesInRow,nodesInColumn, 15.0, 4.0, 2.0, 3.0))
            {
                Assert.AreEqual(count, grid.NodeCount());
            }
        }

        [TestCase(5,3,4*3+5*2)]
        [TestCase(15,42,15*41+14*42)]
        public void Mesh2D_GetEdgeCount_CountsRowOrientedAndColumnOrientedEdges(int nodesInRow, int nodesInColumn, int count)
        {
            using (var grid = TestUtilityFunctions.CreateMesh2D(nodesInRow,nodesInColumn, 15.0, 4.0, 2.0, 3.0))
            {
                Assert.AreEqual(count, grid.EdgeCount());
            }
        }
    }
}