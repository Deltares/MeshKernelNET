using System.Collections.Generic;
using System.Linq;
using MeshKernelNET.Api;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    /// <summary>
    /// This test fixture tests whether the utility functions for creating grids produce the same geometry and
    /// connectivity arrays as corresponding grids produced by MeshKernel. 
    /// </summary>
    [TestFixture]
    public class MeshKernelTestUtilsTest
    {
        MeshKernelApi api;

        [SetUp]
        public void Setup()
        {
            api = new MeshKernelApi();
        }

        [TearDown]
        public void TearDown()
        {
            api.Dispose();
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateMesh2D_CreatesExpectedNumberOfNodesAndEdges(int nx, int ny)
        {
            var meshFromKernel = RectangleMeshFromMeshKernel(MakeGridParameters(nx, ny));

            var mesh = TestUtilityFunctions.CreateMesh2D(nx, ny, 1, 1);

            Assert.That(mesh.NodeCount(), Is.EqualTo(meshFromKernel.NodeCount()));
            Assert.That(mesh.EdgeCount(), Is.EqualTo(meshFromKernel.EdgeCount()));
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateMesh2D_CreatesExpectedNodeX(int nx, int ny)
        {
            var meshFromKernel = RectangleMeshFromMeshKernel(MakeGridParameters(nx, ny));

            var mesh = TestUtilityFunctions.CreateMesh2D(nx, ny, 1, 1);
            
            Assert.That(mesh.NodeX.SequenceEqual(meshFromKernel.NodeX), Is.True, 
                $"{AsText(mesh.NodeX)} != {AsText(meshFromKernel.NodeX)}");
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateMesh2D_CreatesExpectedNodeY(int nx, int ny)
        {
            var meshFromKernel = RectangleMeshFromMeshKernel(MakeGridParameters(nx, ny));

            var mesh = TestUtilityFunctions.CreateMesh2D(nx, ny, 1, 1);
            
            Assert.That(mesh.NodeY.SequenceEqual(meshFromKernel.NodeY), Is.True,
                $"{AsText(mesh.NodeY)} != {AsText(meshFromKernel.NodeY)}");
        }
        
        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateMesh2D_CreatesExpectedEdgeNodes(int nx, int ny)
        {
            var meshFromKernel = RectangleMeshFromMeshKernel(MakeGridParameters(nx, ny));

            var mesh = TestUtilityFunctions.CreateMesh2D(nx, ny, 1, 1);
            
            Assert.That(mesh.EdgeNodes.SequenceEqual(meshFromKernel.EdgeNodes), Is.True,
                $"{AsText(mesh.EdgeNodes)} != {AsText(meshFromKernel.EdgeNodes)}");
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateCurvilinearGrid_CreatesExpectedNumberOfNodesAndEdges(int nx, int ny)
        {
            var gridFromKernel = RectangleCurvilinearGridFromMeshKernel(MakeGridParameters(nx, ny));

            var grid = TestUtilityFunctions.CreateCurvilinearGrid(ny, nx, 1, 1);

            Assert.That(grid.NodeCount(), Is.EqualTo(gridFromKernel.NodeCount()));
            Assert.That(grid.EdgeCount(), Is.EqualTo(gridFromKernel.EdgeCount()));
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateCurvilinearGrid_CreatesExpectedNodeX(int nx, int ny)
        {
            var gridFromKernel = RectangleCurvilinearGridFromMeshKernel(MakeGridParameters(nx, ny));

            var grid = TestUtilityFunctions.CreateCurvilinearGrid(ny, nx, 1, 1);
            
            Assert.That(grid.NodeX.SequenceEqual(gridFromKernel.NodeX), Is.True, 
                        $"{AsText(grid.NodeX)} != {AsText(gridFromKernel.NodeX)}");
        }

        [TestCase(3,4)]
        [TestCase(4,3)]
        public void CreateCurvilinearGrid_CreatesExpectedNodeY(int nx, int ny)
        {
            var gridFromKernel = RectangleCurvilinearGridFromMeshKernel(MakeGridParameters(nx, ny));

            var grid = TestUtilityFunctions.CreateCurvilinearGrid(ny, nx, 1, 1);
            
            Assert.That(grid.NodeY.SequenceEqual(gridFromKernel.NodeY), Is.True,
                        $"{AsText(grid.NodeY)} != {AsText(gridFromKernel.NodeY)}");
        }
        
        /// <summary>
        /// Create a data structure used to create a rectangle grid. 
        /// </summary>
        /// <param name="numNodesX">The number of nodes in x direction</param>
        /// <param name="numNodesY">The number of nodes in y direction</param>
        /// <returns></returns>
        private static MakeGridParameters MakeGridParameters(int numNodesX, int numNodesY)
        {
            return new MakeGridParameters
            {
                NumberOfColumns = numNodesX - 1, NumberOfRows = numNodesY - 1, XGridBlockSize = 1, YGridBlockSize = 1
            };
        }

        private DisposableMesh2D RectangleMeshFromMeshKernel(MakeGridParameters rectangle)
        {
            int id = api.AllocateState(0);
            api.Mesh2dMakeRectangularMesh(id, rectangle);
            api.Mesh2dGetData(id, out DisposableMesh2D mesh);
            api.DeallocateState(id);
            return mesh;
        }

        private DisposableCurvilinearGrid RectangleCurvilinearGridFromMeshKernel(MakeGridParameters rectangle)
        {
            int id = api.AllocateState(0);
            api.CurvilinearComputeRectangularGrid(id, rectangle);
            api.CurvilinearGridGetData(id, out DisposableCurvilinearGrid grid);
            api.DeallocateState(id);
            return grid;
        }

        private static string AsText<T>(IEnumerable<T> sequence)
        {
            return "[" + string.Join(",", sequence.Select(x => x.ToString())) + "]";
        }
    }
}