using System;
using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNET.Api;
using NUnit.Framework;
using static MeshKernelNETTest.Api.TestUtilityFunctions;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETMesh2DTests")]
    public class MeshKernelTest
    {
        [Test]
        public void Mesh2dDeleteNodeThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                           1 ------- 2 ------- 3
            // |         |         |         |                           |         |         |
            // |         |         |         |                           |         |         |
            // |         |         |         |                           |         |         |
            // |         |         |         |                           |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11 
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |   
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2d = new DisposableMesh2D();
                try
                {
                    int numberOfVerticesBefore = mesh.NumNodes;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    Assert.That(api.Mesh2dDeleteNode(id, 0), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh.NumValidEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 1));
                }
                finally
                {
                    api.ClearState();
                    mesh2d.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dFlipEdgesThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                 0 ------- 1 ------- 2 ------- 3
            // |         |         |         |                 |      .  |      .  |      .  |
            // |         |         |         |                 |    .    |    .    |    .    |
            // |         |         |         |                 |  .      |  .      |  .      |
            // |         |         |         |                 |.        |.        |.        |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |      .  |      .  |      .  |
            // |         |         |         |                 |    .    |    .    |    .    |
            // |         |         |         |                 |  .      |  .      |  .      |
            // |         |         |         |                 |.        |.        |.        |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11 
            // |         |         |         |                 |      .  |      .  |      .  |
            // |         |         |         |                 |    .    |    .    |    .    |
            // |         |         |         |                 |  .      |  .      |  .      |   
            // |         |         |         |                 |.        |.        |.        |
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var geometryListIn = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dFlipEdges(id, true, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, geometryListIn, landBoundaries), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore + 9));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dInsertEdgeThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                 0 ------- 1 ------- 2 ------- 3
            // |         |         |         |                 |      .  |         |         |
            // |         |         |         |                 |    .    |         |         |
            // |         |         |         |                 |  .      |         |         |
            // |         |         |         |                 |.        |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |  
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                 2 ------ 13 ------ 14 ------ 15

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    var newEdgeIndex = 0;
                    Assert.That(api.Mesh2dInsertEdge(id, 4, 1, ref newEdgeIndex), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore + 1));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dInsertEdgeFromCoordinatesThroughApi()
        {
            // Before                                          After
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15
            // |         |         |         |                 |      .  |         |         |
            // |         |         |         |                 |    .    |         |         |
            // |         |         |         |                 |  .      |         |         |
            // |         |         |         |                 |.        |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 4 ------- 5 ------  6 ------  7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |  
            // |         |         |         |                 |         |         |         |
            // 0 ------  1 ------  2 ------  3                 0 ------  1 ------  2 ------  3

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                int numberOfEdgesBefore = mesh.NumEdges;
                Assert.That(numberOfEdgesBefore, Is.EqualTo(4*3 + 3*4));
                int numberOfNodesBefore = mesh.NumNodes;
                Assert.That(numberOfNodesBefore, Is.EqualTo(4*4));
                
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh),Is.EqualTo(0));

                    var firstNewNodeIndex = 0;
                    var secondNewNodeIndex = 0;
                    var newEdgeIndex = 0;
                    Assert.That(api.Mesh2dInsertEdgeFromCoordinates(id,
                                                                    0.0,
                                                                    400.0,
                                                                    100.0,
                                                                    600.0,
                                                                    ref firstNewNodeIndex,
                                                                    ref secondNewNodeIndex,
                                                                    ref newEdgeIndex), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumNodes, Is.EqualTo(numberOfNodesBefore));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore + 1));
                    Assert.That(firstNewNodeIndex, Is.EqualTo(8));
                    Assert.That(secondNewNodeIndex, Is.EqualTo(13));
                    Assert.That(newEdgeIndex, Is.EqualTo(numberOfEdgesBefore)); // appended at end
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeTwoNodesThroughApi()
        {
            // Before                                          After
            //     21        22        23
            //12 ------ 13 ------ 14 ------ 15                 12       13 ------ 14 ------ 15
            // |         |         |         |                        .  |         |         |
            // |8        |9        |10       |11                    .    |         |         |
            // |         |         |         |                    .      |         |         |
            // |   18    |   19    |   20    |                  .        |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11
            // |         |         |         |                 |         |         |         |
            // |4        |5        |6        |7                |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |   15    |   16    |   17    |                 |         |         |         |
            // 4 ------- 5 ------  6 ------  7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // | 0       |1        |2        |3                |         |         |         |
            // |         |         |         |                 |         |         |         |  
            // |   12    |   13    |   14    |                 |         |         |         |
            // 0 ------  1 ------  2 ------  3                 0 ------  1 ------  2 ------  3

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                int numberOfEdgesBefore = mesh.NumEdges;
                Assert.That(numberOfEdgesBefore, Is.EqualTo(4*3 + 3*4));
                int numberOfNodesBefore = mesh.NumNodes;
                Assert.That(numberOfNodesBefore, Is.EqualTo(4*4));

                var mesh2D = new DisposableMesh2D();
                try
                {
                    int id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    Assert.That(api.Mesh2dMergeTwoNodes(id, 12, 13), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    
                    Assert.That(mesh2D.NumNodes, Is.EqualTo(numberOfNodesBefore));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore));
                    
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(numberOfEdgesBefore-1));
                    
                    var mesh2DEdgeNodes = mesh2D.EdgeNodes;
                    
                    const int invalidatedEdge = 21;
                    int[] invalidatedEdgeNodes = { 42, 43 };
                    Assert.That(mesh2DEdgeNodes.Select((n,i) => (n,i))
                                               .Where(t => t.n < 0)
                                               .Select(t => t.i),Is.EquivalentTo(invalidatedEdgeNodes), AsString(mesh2DEdgeNodes));
                    const int reconnectedEdge = 8;
                    int[] reconnectedEdgeNodes = { 8, 13 };
                    Assert.That(mesh2DEdgeNodes.Skip(2 * 8).Take(2),Is.EquivalentTo(reconnectedEdgeNodes), AsString(mesh2DEdgeNodes));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeNodesThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                 0 ------- 1 ------- 2 ------- 3
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11 
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |   
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15
            //
            // Merges vertices, effectively removing small edges.
            // In this case no small edges are present, so no edges shall be removed.

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var geometryList = new DisposableGeometryList();
                    Assert.That(api.Mesh2dMergeNodes(id, geometryList), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeNodesWithMergeDistanceThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                 0 ------- 1 ------- 2 ------- 3
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11 
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |   
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15
            //
            // Merges vertices within a distance of 0.001 m, effectively removing small edges.
            // In this case no small edges are present, so no edges shall be removed.

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var geometryList = new DisposableGeometryList();
                    Assert.That(api.Mesh2dMergeNodesWithMergingDistance(id, geometryList, 0.001), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dInitializeOrthogonalizationThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                 0 ------- 1 ------- 2 ------- 3
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 4 ------- 5 ------- 6 ------- 7
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 8 ------- 9 ------ 10 ------ 11 
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |   
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                12 ------ 13 ------ 14 ------ 15

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    int numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    Assert.That(api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.Not.EqualTo(2));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(numberOfEdgesBefore));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void GetSplinesThroughAPI()
        {
            // Setup
            using (var geometryListIn = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                double geometrySeparator = api.GetSeparator();
                geometryListIn.GeometrySeparator = geometrySeparator;
                geometryListIn.NumberOfCoordinates = 3;
                geometryListIn.XCoordinates = new[] { 10.0, 20.0, 30.0 };
                geometryListIn.YCoordinates = new[] { -5.0, 5.0, -5.0 };
                geometryListIn.Values = new[] { 0.0, 0.0, 0.0 };

                var numberOfPointsBetweenVertices = 20;
                var geometryListOut = new DisposableGeometryList();
                geometryListOut.GeometrySeparator = geometrySeparator;
                geometryListOut.NumberOfCoordinates = 60;
                geometryListOut.XCoordinates = new double[60];
                geometryListOut.YCoordinates = new double[60];
                geometryListOut.Values = new double[60];

                Assert.That(api.GetSplines(geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices), Is.EqualTo(0));
                geometryListOut.Dispose();
            }
        }

        [Test]
        public void Mesh2dMakeTriangularMeshFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 17;

                    geometryListIn.XCoordinates = new[] { 415.319672, 390.271973, 382.330048, 392.715668, 418.374268, 453.807556, 495.960968, 532.005188, 565.605774, 590.653442, 598.595398, 593.708008, 564.994812, 514.899475, 461.138611, 422.039764, 415.319672 };

                    geometryListIn.YCoordinates = new[] { 490.293762, 464.024139, 438.365448, 411.484894, 386.437103, 366.276703, 363.222107, 370.553162, 386.437103, 412.095825, 445.085571, 481.129944, 497.624817, 504.955872, 501.290344, 493.348358, 490.293762 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                    Assert.That(api.Mesh2dMakeTriangularMeshFromPolygon(id, geometryListIn), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeTriangularMeshFromSamplesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 5;

                    geometryListIn.XCoordinates = new[] { 0.0, 10.0, 10.0, 0.0, 0.0 };

                    geometryListIn.YCoordinates = new[] { 0.0, 0.0, 10.0, 10.0, 0.0 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

                    Assert.That(api.Mesh2dMakeTriangularMeshFromSamples(id, geometryListIn), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetMeshBoundariesAsPolygonsThroughAPI()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var selectingPolygon = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int numberOfPolygonVertices = -1;
                    Assert.That(api.Mesh2dCountMeshBoundariesAsPolygons(id, selectingPolygon, ref numberOfPolygonVertices), Is.EqualTo(0));
                    Assert.That(numberOfPolygonVertices, Is.EqualTo(13));

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.Values = new double[numberOfPolygonVertices];
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = numberOfPolygonVertices;
                    Assert.That(api.Mesh2dGetMeshBoundariesAsPolygons(id, selectingPolygon, ref geometryListIn), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void PolygonOffsetThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var disposableGeometryListOut = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 5;

                    geometryListIn.XCoordinates = new[] { 0.0, 1.0, 1.0, 0.0, 0.0 };

                    geometryListIn.YCoordinates = new[] { 0.0, 0.0, 1.0, 1.0, 0.0 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

                    var distance = 10.0;
                    int numberOfPolygonVertices = -1;
                    var innerOffsetedPolygon = false;
                    Assert.That(api.PolygonCountOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref numberOfPolygonVertices), Is.EqualTo(0));
                    Assert.That(numberOfPolygonVertices, Is.EqualTo(5));
                    Assert.That(api.PolygonGetOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref disposableGeometryListOut), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    disposableGeometryListOut.Dispose();
                }
            }
        }

        [Test]
        public void PolygonRefineThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                var geometryListOut = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 4;

                    geometryListIn.XCoordinates = new[] { 76.251099, 498.503723, 505.253784, 76.251099 };

                    geometryListIn.YCoordinates = new[] { 92.626556, 91.126541, 490.130554, 92.626556 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };

                    var distance = 40.0;
                    var firstIndex = 0;
                    var secondIndex = 2;
                    int numberOfPolygonVertices = -1;
                    Assert.That(api.PolygonCountEquidistantRefine(id,
                                                              geometryListIn,
                                                              firstIndex,
                                                              secondIndex,
                                                              distance,
                                                              ref numberOfPolygonVertices), Is.EqualTo(0));

                    geometryListOut.GeometrySeparator = geometrySeparator;
                    geometryListOut.NumberOfCoordinates = numberOfPolygonVertices;

                    geometryListOut.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.Values = new double[numberOfPolygonVertices];

                    Assert.That(api.PolygonEquidistantRefine(id,
                                                         geometryListIn,
                                                         firstIndex,
                                                         secondIndex,
                                                         distance,
                                                         ref geometryListOut), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    geometryListOut.Dispose();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void PolygonLinearRefineThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var geometryListOut = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 4;

                    geometryListIn.XCoordinates = new[] { 76.251099, 498.503723, 505.253784, 76.251099 };

                    geometryListIn.YCoordinates = new[] { 92.626556, 91.126541, 490.130554, 92.626556 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };

                    var firstIndex = 0;
                    var secondIndex = 2;
                    int numberOfPolygonVertices = -1;
                    var result = api.PolygonCountLinearRefine(id,
                                                       geometryListIn,
                                                       firstIndex,
                                                       secondIndex,
                                                       ref numberOfPolygonVertices);
                    Assert.That(result, Is.EqualTo(0));

                    geometryListOut.GeometrySeparator = geometrySeparator;
                    geometryListOut.NumberOfCoordinates = numberOfPolygonVertices;

                    geometryListOut.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.Values = new double[numberOfPolygonVertices];

                    result = api.PolygonLinearRefine(id,
                                                     geometryListIn,
                                                     firstIndex,
                                                     secondIndex,
                                                     ref geometryListOut);

                    Assert.That(result, Is.EqualTo(0));

                }
                finally
                {
                    api.ClearState();
                    geometryListOut.Dispose();
                }
            }
        }

        [Test]
        public void PolygonSnapToLandBoundaryThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var landboundaries = new DisposableGeometryList())
            {
                var id = 0;
                var polygon = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    landboundaries.GeometrySeparator = geometrySeparator;
                    landboundaries.NumberOfCoordinates = 4;
                    landboundaries.XCoordinates = new[] { 139.251465, 527.753906, 580.254211, 194.001801 };
                    landboundaries.YCoordinates = new[] { 497.630615, 499.880676, 265.878296, 212.627762 };
                    landboundaries.NumberOfCoordinates = landboundaries.XCoordinates.Length;

                    polygon.XCoordinates = new[] { 170.001648, 263.002228, 344.002747,
                        458.753448, 515.753845, 524.753906,
                        510.503754, 557.754089, 545.004028,
                        446.003387, 340.252716, 242.752106,
                        170.001648 };
                    polygon.YCoordinates = new[] { 472.880371, 472.880371, 475.130432,
                        482.630493, 487.130554, 434.630005,
                        367.129333, 297.378601, 270.378357,
                        259.128235, 244.128067, 226.877884,
                        472.880371 };
                    polygon.NumberOfCoordinates = polygon.XCoordinates.Length;


                    var firstIndex = 0;
                    var secondIndex = 2;
                    var result = api.PolygonSnapToLandBoundary(id,
                                                               landboundaries,
                                                               ref polygon,
                                                               firstIndex,
                                                               secondIndex);
                    Assert.That(result, Is.EqualTo(0));


                    const double tolerance = 1.0e-3;
                    var expectedXCoordinates = new[] { 169.85727722422831,
                        262.854737816309,
                        343.86557098779792,
                        458.753448,
                        515.753845,
                        524.753906,
                        510.503754,
                        557.754089,
                        545.004028,
                        446.003387,
                        340.252716,
                        242.752106,
                        170.001648 };

                    var expectedYCoordinates = new[]
                    {   497.80787243056284,
                        498.34647897995455,
                        498.81566346133775,
                        482.630493,
                        487.130554,
                        434.630005,
                        367.129333,
                        297.378601,
                        270.378357,
                        259.128235,
                        244.128067,
                        226.877884,
                        472.880371};

                    Assert.That(polygon.XCoordinates, Is.EqualTo(expectedXCoordinates).Within(tolerance));
                    Assert.That(polygon.YCoordinates, Is.EqualTo(expectedYCoordinates).Within(tolerance));

                }
                finally
                {
                    api.ClearState();
                    polygon.Dispose();
                }
            }
        }

        [Test]
        public void SplineSnapToLandBoundaryThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var landboundaries = new DisposableGeometryList())
            {
                var splines = new DisposableGeometryList();
                try
                {
                    var id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    landboundaries.GeometrySeparator = geometrySeparator;
                    landboundaries.XCoordinates = new[] { 257.002197, 518.753845, 938.006470 };
                    landboundaries.YCoordinates = new[] { 442.130066, 301.128662, 416.629822 };
                    landboundaries.NumberOfCoordinates = landboundaries.XCoordinates.Length;

                    splines.XCoordinates = new[] { 281.0023, 367.2529, 461.7534, 517.2538, 614.0045, 720.5051, 827.7558, 923.7563 };
                    splines.YCoordinates = new[] { 447.3801, 401.6296, 354.3792, 318.3788, 338.629, 377.6294, 417.3798, 424.1299 };
                    splines.NumberOfCoordinates = splines.XCoordinates.Length;


                    var firstIndex = 0;
                    var secondIndex = splines.XCoordinates.Length - 1;
                    var result = api.SplinesToLandBoundary(id, 
                                                          landboundaries,
                                                          ref splines,
                                                          firstIndex,
                                                          secondIndex);
                    Assert.That(result, Is.EqualTo(0));


                    var expectedXCoordinates = new[] { 273.5868719643935, 359.5998304717778, 451.5303458337523, 517.7962262926076,
                        616.7325138813335, 725.7358644094627, 836.2627853156330, 923.5001778441060};

                    var expectedYCoordinates = new[]
                    {  434.2730022174478, 386.1712239047134, 338.3551703843473, 306.3259738916997,
                        327.9627689164845, 358.0902879743862, 388.6415116416172, 412.5818685325169};

                    const double tolerance = 1.0e-3;
                    Assert.That(splines.XCoordinates, Is.EqualTo(expectedXCoordinates).Within(tolerance));
                    Assert.That(splines.YCoordinates, Is.EqualTo(expectedYCoordinates).Within(tolerance));

                }
                finally
                {
                    api.ClearState();
                    splines.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dRefineBasedOnSamplesThroughAPI()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { 50.0, 150.0, 250.0, 50.0, 150.0, 250.0, 50.0, 150.0, 250.0 };

                    geometryListIn.YCoordinates = new[] { 50.0, 50.0, 50.0, 150.0, 150.0, 150.0, 250.0, 250.0, 250.0 };

                    geometryListIn.Values = new[] { 2.0, 2.0, 2.0, 3.0, 3.0, 3.0, 4.0, 4.0, 4.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    var relativeSearchRadius = 1.0;
                    var minimumNumSamples = 1;
                    Assert.That(api.Mesh2dRefineBasedOnSamples(id, geometryListIn, relativeSearchRadius, minimumNumSamples, meshRefinementParameters), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dRefineBasedOnPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { 250.0, 750.0, 750.0, 250.0, 250.0 };

                    geometryListIn.YCoordinates = new[] { 250.0, 250.0, 750.0, 750.0, 250.0 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    //Call
                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    meshRefinementParameters.MaxNumRefinementIterations = 2;
                    meshRefinementParameters.RefineIntersected = false;
                    Assert.That(api.Mesh2dRefineBasedOnPolygon(id, geometryListIn, meshRefinementParameters), Is.EqualTo(0));
                    //Assert
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        private static double[,] RotateNodes([In] double[] nodesX,
                                             [In] double[] nodesY,
                                             [In] double[] centre,
                                             [In] double angle)
        {
            if (nodesX.Length <= 0 || nodesX.Length != nodesY.Length)
            {
                throw new ArgumentException("NodesX and NodesY must not be empty and must be of equal size.");
            }

            if (centre.Length != 2)
            {
                throw new ArgumentException("The size of centre must be exactly 2.");
            }

            var rotatedNodes = new double[nodesX.Length, 2];

            double angleRad = (angle * Math.PI) / 180.0;
            double angleCos = Math.Cos(angleRad);
            double angleSin = Math.Sin(angleRad);

            for (var i = 0; i < nodesX.Length; i++)
            {
                double offsetX = nodesX[i] - centre[0];
                double offsetY = nodesY[i] - centre[1];
                rotatedNodes[i, 0] = (centre[0] + (angleCos * offsetX)) - (angleSin * offsetY);
                rotatedNodes[i, 1] = centre[1] + (angleSin * offsetX) + (angleCos * offsetY);
            }

            return rotatedNodes;
        }

        [Test]
        public void Mesh2dRotateThroughAPI()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 1, 2))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var meshInitial = new DisposableMesh2D();
                var meshRotated = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // Get data of initial mesh
                    Assert.That(api.Mesh2dGetData(id, out meshInitial), Is.EqualTo(0));
                    Assert.That(meshInitial, Is.Not.Null);

                    var rotationCentre = new[] { 1.0, 5.0 };
                    const double rotationAngle = 45.0;
                    Assert.That(api.Mesh2dRotate(id,
                                                        rotationCentre[0],
                                                        rotationCentre[1],
                                                        rotationAngle), Is.EqualTo(0));

                    // Get data of rotated mesh
                    Assert.That(api.Mesh2dGetData(id, out meshRotated), Is.EqualTo(0));
                    Assert.That(meshRotated, Is.Not.Null);

                    // Compute expected nodes
                    double[,] expectedNodes = RotateNodes(meshInitial.NodeX, meshInitial.NodeY, rotationCentre, rotationAngle);

                    // Expected and actual values must be equal within a tolerance
                    const double tolerance = 1.0e-6;
                    for (var i = 0; i < meshInitial.NumNodes; i++)
                    {
                        Assert.That(meshRotated.NodeX[i], Is.EqualTo(expectedNodes[i, 0]).Within(tolerance));
                        Assert.That(meshRotated.NodeY[i], Is.EqualTo(expectedNodes[i, 1]).Within(tolerance));
                    }
                }
                finally
                {
                    api.ClearState();
                    meshInitial.Dispose();
                    meshRotated.Dispose();
                }
            }
        }

        private static double[,] TranslateNodes([In] double[] nodesX,
                                                [In] double[] nodesY,
                                                [In] double[] translation)
        {
            if (nodesX.Length <= 0 || nodesX.Length != nodesY.Length)
            {
                throw new ArgumentException("NodesX and NodesY must not be empty and must be of equal size.");
            }

            if (translation.Length != 2)
            {
                throw new ArgumentException("The size of translation must be exactly 2.");
            }

            var translatedNodes = new double[nodesX.Length, 2];

            for (var i = 0; i < nodesX.Length; i++)
            {
                translatedNodes[i, 0] = nodesX[i] + translation[0];
                translatedNodes[i, 1] = nodesY[i] + translation[1];
            }

            return translatedNodes;
        }

        [Test]
        public void Mesh2dTranslateThroughAPI()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 1, 2))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var meshInitial = new DisposableMesh2D();
                var meshTranslated = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // Get data of initial mesh
                    Assert.That(api.Mesh2dGetData(id, out meshInitial), Is.EqualTo(0));
                    Assert.That(meshInitial, Is.Not.Null);

                    var translation = new[] { 1.0, 5.0 };
                    Assert.That(api.Mesh2dTranslate(id,
                                                           translation[0],
                                                           translation[1]), Is.EqualTo(0));

                    // Get data of rotated mesh
                    Assert.That(api.Mesh2dGetData(id, out meshTranslated), Is.EqualTo(0));
                    Assert.That(meshTranslated, Is.Not.Null);

                    // Compute expected nodes
                    double[,] expectedNodes = TranslateNodes(meshInitial.NodeX, meshInitial.NodeY, translation);

                    // Expected and actual values must be equal within a tolerance
                    const double tolerance = 1.0e-6;
                    for (var i = 0; i < meshInitial.NumNodes; i++)
                    {
                        Assert.That(meshTranslated.NodeX[i], Is.EqualTo(expectedNodes[i, 0]).Within(tolerance));
                        Assert.That(meshTranslated.NodeY[i], Is.EqualTo(expectedNodes[i, 1]).Within(tolerance));
                    }
                }
                finally
                {
                    api.ClearState();
                    meshInitial.Dispose();
                    meshTranslated.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeTransfiniteFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 10;

                    geometryListIn.XCoordinates = new[] { 273.502319, 274.252319, 275.002350, 458.003479, 719.005127, 741.505249, 710.755066, 507.503784, 305.002533, 273.502319 };

                    geometryListIn.YCoordinates = new[] { 478.880432, 325.128906, 172.127350, 157.127213, 157.127213, 328.128937, 490.880554, 494.630615, 493.130615, 478.880432 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                    // Call
                    Assert.That(api.CurvilinearComputeTransfiniteFromPolygon(id, geometryListIn, 0, 2, 4, true), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    // Assert a valid mesh is produced
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((3, 3)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeTransfiniteFromTriangleThroughAPI()
        {
            //Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 11;

                    geometryListIn.XCoordinates = new[] { 444.504791, 427.731781, 405.640503, 381.094666, 451.050354, 528.778931, 593.416260, 558.643005, 526.733398, 444.095703, 444.504791 };

                    geometryListIn.YCoordinates = new[] { 437.155945, 382.745758, 317.699005, 262.470612, 262.879700, 263.288788, 266.561584, 324.653687, 377.836578, 436.746857, 437.155945 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

                    // Call
                    Assert.That(api.CurvilinearComputeTransfiniteFromTriangle(id, geometryListIn, 0, 3, 6), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    // Assert a valid mesh is produced
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 4)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetClosestNodeThroughAPI()
        {
            //Setup

            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    //Call
                    var xCoordinateOut = 0.0;
                    var yCoordinateOut = 0.0;
                    Assert.That(api.Mesh2dGetClosestNode(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref xCoordinateOut, ref yCoordinateOut), Is.EqualTo(0));
                    //Assert
                    Assert.That(xCoordinateOut, Is.LessThanOrEqualTo(1e-6));
                    Assert.That(yCoordinateOut, Is.LessThanOrEqualTo(1e-6));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetNodeIndexThroughAPI()
        {
            //Setup

            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    //Execute
                    int nodeIndex = -1;
                    Assert.That(api.Mesh2dGetNodeIndex(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref nodeIndex), Is.EqualTo(0));
                    //Assert
                    Assert.That(nodeIndex, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dDeleteEdgeThroughAPI()
        {
            //Setup

            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int numEdges = mesh.NumEdges;

                    //Call
                    Assert.That(api.Mesh2dDeleteEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0), Is.EqualTo(0));
                    //Assert

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(numEdges - 1));
                    mesh2D.Dispose();
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void Mesh2dDeleteEdgeByIndexThroughAPI()
        {
            //Setup

            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int initNumEdges = mesh.NumEdges;

                    // Delete a valid edge
                    Assert.That(api.Mesh2dDeleteEdgeByIndex(id, 0), Is.EqualTo(0));
                    // Expect the number of edges to decrease by 1
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(initNumEdges - 1));

                    // Delete an invalid edge
                    Assert.That(api.Mesh2dDeleteEdgeByIndex(id, 666), Is.EqualTo(0));
                    // Expect the number of edges to remain unchanged
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(initNumEdges - 1));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetEdgeThroughAPI()
        {
            //Setup

            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int numEdges = mesh.NumEdges;

                    //Call
                    int edgeIndex = -1;
                    Assert.That(api.Mesh2dGetEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0, ref edgeIndex), Is.EqualTo(0));
                    //Assert
                    Assert.That(edgeIndex, Is.EqualTo(110)); // first horizontal edge, comes after all (110) vertical edges
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void ContactsComputeSingleContactsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));
                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    var projectionFactor = 0.0;
                    Assert.That(api.ContactsComputeSingle(id, onedNodeMaskPinnedAddress, geometryListIn, projectionFactor), Is.EqualTo(0));
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    Assert.That(contacts.NumContacts, Is.GreaterThan(0));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                    contacts.Dispose();
                }
            }
        }

        [Test]
        public void ContactsComputeMultipleThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));
                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeMultiple(id, onedNodeMaskPinnedAddress), Is.EqualTo(0));
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    Assert.That(contacts.NumContacts, Is.GreaterThan(0));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                    contacts.Dispose();
                }
            }
        }

        [Test]
        public void ContactsComputeWithPolygonsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeWithPolygons(id, onedNodeMaskPinnedAddress, geometryListIn), Is.EqualTo(0));
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    // Only one contact is generated, because only one polygon is present 
                    Assert.That(contacts.NumContacts, Is.EqualTo(1));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                    contacts.Dispose();
                }
            }
        }

        [Test]
        public void ContactsComputeWithPointsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeWithPoints(id, onedNodeMaskPinnedAddress, geometryListIn), Is.EqualTo(0));
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    // Four contacts are generated, one for each point
                    Assert.That(contacts.NumContacts, Is.EqualTo(4));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                    contacts.Dispose();
                }
            }
        }

        [Test]
        public void GetSelectedVerticesInPolygonThroughAPI()
        {
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 1, 1))
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    geometryListIn.XCoordinates = new[] { 1.5, 1.5, 3.5, 3.5, 1.5 };
                    geometryListIn.YCoordinates = new[] { -1.5, 1.5, 1.5, -1.5, -1.5 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    geometryListIn.GeometrySeparator = api.GetSeparator();

                    int[] selectedVertices = Array.Empty<int>();
                    Assert.That(api.GetSelectedVerticesInPolygon(id, geometryListIn, 1, ref selectedVertices), Is.EqualTo(0));
                    Assert.That(selectedVertices.Length, Is.EqualTo(4));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void GetAveragingMethodClosestPointThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodClosestPoint(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetAveragingMethodInverseDistanceWeightingThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodInverseDistanceWeighting(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(5));
            }
        }

        [Test]
        public void GetAveragingMethodMaxThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodMax(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(3));
            }
        }

        [Test]
        public void GetAveragingMethodMinThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodMin(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(4));
            }
        }

        [Test]
        public void GetAveragingMethodMinAbsoluteValueThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodMinAbsoluteValue(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(6));
            }
        }

        [Test]
        public void GetAveragingMethodSimpleAveragingThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.That(api.GetAveragingMethodSimpleAveraging(ref method), Is.EqualTo(0));
                // Assert
                Assert.That(method, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetEdgesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int locationType = -1;
                Assert.That(api.GetEdgesLocationType(ref locationType), Is.EqualTo(0));
                // Assert
                Assert.That(locationType, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var disposableMesh1D = new DisposableMesh1D();

                Assert.That(api.Mesh1dGetData(0, out disposableMesh1D), Is.Not.EqualTo(0));

                // Execute
                Assert.That(api.GetError(out string errorMessage), Is.EqualTo(0));
                Assert.That(errorMessage.Length > 0, Is.True);

                disposableMesh1D.Dispose();
            }
        }

        [Test]
        public void GetExitCodeSuccessThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeSuccess(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(0));
            }
        }

        [Test]
        public void GetExitCodeMeshKernelErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeMeshKernelError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetExitCodeNotImplementedErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeNotImplementedError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetExitCodeAlgorithmErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeAlgorithmError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(3));
            }
        }

        [Test]
        public void GetExitCodeConstraintErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeConstraintError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(4));
            }
        }

        [Test]
        public void GetExitCodeMeshGeometryErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeMeshGeometryError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(5));
            }
        }

        [Test]
        public void GetExitCodeLinearAlgebraErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeLinearAlgebraError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(6));
            }
        }

        [Test]
        public void GetExitCodeRangeErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeRangeError(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(7));
            }
        }

        [Test]
        public void GetExitCodeStdLibExceptionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeStdLibException(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(8));
            }
        }

        [Test]
        public void GetExitCodeUnknownExceptionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.That(api.GetExitCodeUnknownException(ref exitCode), Is.EqualTo(0));
                // Assert
                Assert.That(exitCode, Is.EqualTo(9));
            }
        }

        [Test]
        public void GetFacesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int locationType = -1;
                Assert.That(api.GetFacesLocationType(ref locationType), Is.EqualTo(0));

                // Assert
                Assert.That(locationType, Is.EqualTo(0));
            }
        }

        [Test]
        public void GetGeometryErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int invalidIndex = -1;
                int type = -1;
                Assert.That(api.GetGeometryError(ref invalidIndex, ref type), Is.EqualTo(0));
                // Assert
                Assert.That(invalidIndex, Is.EqualTo(0));
                Assert.That(type, Is.EqualTo(3));
            }
        }

        [Test]
        public void GetNodesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int type = -1;
                Assert.That(api.GetNodesLocationType(ref type), Is.EqualTo(0));
                // Assert
                Assert.That(type, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetProjectionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var id = 1;
                try
                {
                    id = api.AllocateState(0);

                    // Execute
                    int projection = -1;
                    Assert.That(api.GetProjection(id, ref projection), Is.EqualTo(0));
                    // Assert
                    Assert.That(projection, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void GetProjectionCartesianThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int projection = -1;
                Assert.That(api.GetProjectionCartesian(ref projection), Is.EqualTo(0));
                // Assert
                Assert.That(projection, Is.EqualTo(0));
            }
        }

        [Test]
        public void GetProjectionSphericalThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int projection = -1;
                Assert.That(api.GetProjectionSpherical(ref projection), Is.EqualTo(0));
                // Assert
                Assert.That(projection, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetProjectionSphericalAccurateThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int projection = -1;
                Assert.That(api.GetProjectionSphericalAccurate(ref projection), Is.EqualTo(0));
                // Assert
                Assert.That(projection, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetVersionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                string version;
                Assert.That(api.GetVersion(out version), Is.EqualTo(0));
                // Assert
                Assert.That(version.Length, Is.GreaterThan(0));
            }
        }

        [Test]
        public void Mesh1dGetDataThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var id = 1;
                var inDisposableMesh1D = new DisposableMesh1D(2, 1);
                var outDisposableMesh1D = new DisposableMesh1D();
                try
                {
                    id = api.AllocateState(0);

                    inDisposableMesh1D.NodeX = new[] { 0.0, 1.0 };
                    inDisposableMesh1D.NodeY = new[] { 0.0, 1.0 };
                    inDisposableMesh1D.EdgeNodes = new[] { 0, 1 };

                    Assert.That(api.Mesh1dSet(id, inDisposableMesh1D), Is.EqualTo(0));

                    Assert.That(api.Mesh1dGetData(id, out outDisposableMesh1D), Is.EqualTo(0));
                    Assert.That(outDisposableMesh1D.NumEdges, Is.EqualTo(inDisposableMesh1D.NumEdges));
                    Assert.That(outDisposableMesh1D.NumNodes, Is.EqualTo(inDisposableMesh1D.NumNodes));
                }
                finally
                {
                    api.ClearState();
                    outDisposableMesh1D.Dispose();
                    inDisposableMesh1D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dAveragingInterpolationThroughAPI()
        {
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D disposableMesh2D = CreateMesh2D(2, 3, 1, 1))
            {
                var id = 1;
                var results = new DisposableGeometryList();
                var samples = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, disposableMesh2D), Is.EqualTo(0));

                    results.XCoordinates = new double[disposableMesh2D.NumNodes];
                    results.YCoordinates = new double[disposableMesh2D.NumNodes];
                    results.Values = new double[disposableMesh2D.NumNodes];
                    results.NumberOfCoordinates = disposableMesh2D.NumNodes;

                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new[] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    var locationType = 1;
                    var averagingMethodType = 1;
                    var relativeSearchSize = 1.01;

                    Assert.That(api.Mesh2dAveragingInterpolation(id,
                                                                        samples,
                                                                        locationType,
                                                                        averagingMethodType,
                                                                        relativeSearchSize,
                                                                        0,
                                                                        ref results), Is.EqualTo(0));

                    Assert.That(results.Values[4], Is.EqualTo(3.0));
                }
                finally
                {
                    api.ClearState();
                    results.Dispose();
                    samples.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliDerefinementThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dCasulliDerefinement(id), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh.NumNodes, Is.GreaterThan(mesh2D.NumNodes));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliDerefinementOnPolygonThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                var polygon = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    polygon.NumberOfCoordinates = 4;
                    polygon.XCoordinates = new[] { 2.5, 7.5, 5.5, 2.5 };
                    polygon.YCoordinates = new[] { 2.5, 4.5, 8.5, 2.5 };
                    polygon.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    Assert.That(api.Mesh2dCasulliDerefinementOnPolygon(id, polygon), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh.NumNodes, Is.GreaterThan(mesh2D.NumNodes));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                    polygon?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliDerefinementElementsThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var elementsToRemove = new DisposableGeometryList();
                var expectedElementsToRemove = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    elementsToRemove.NumberOfCoordinates = mesh.NumNodes;
                    elementsToRemove.XCoordinates = new double[mesh.NumNodes];
                    elementsToRemove.YCoordinates = new double[mesh.NumNodes];
                    for (var i = 0; i < elementsToRemove.NumberOfCoordinates; ++i)
                    {
                        elementsToRemove.XCoordinates[i] = elementsToRemove.YCoordinates[i] = 0.0;
                    }

                    Assert.That(api.Mesh2dCasulliDerefinementElements(id, ref elementsToRemove), Is.EqualTo(0));


                    expectedElementsToRemove.NumberOfCoordinates = 25;
                    expectedElementsToRemove.XCoordinates = new[] { 1.5, 1.5, 1.5, 1.5, 1.5, 3.5, 3.5, 3.5, 3.5, 3.5, 5.5, 5.5, 5.5, 5.5, 5.5, 7.5, 7.5, 7.5, 7.5, 7.5, 9.5, 9.5, 9.5, 9.5, 9.5 };
                    expectedElementsToRemove.YCoordinates = new[] { 0.5, 2.5, 4.5, 6.5, 8.5, 0.5, 2.5, 4.5, 6.5, 8.5, 0.5, 2.5, 4.5, 6.5, 8.5, 0.5, 2.5, 4.5, 6.5, 8.5, 0.5, 2.5, 4.5, 6.5, 8.5 };

                    const double tolerance = 1.0e-12;
                    for (var i = 0; i < expectedElementsToRemove.NumberOfCoordinates; ++i)
                    {
                        Assert.That(elementsToRemove.XCoordinates[i], Is.EqualTo(expectedElementsToRemove.XCoordinates[i]).Within(tolerance));
                        Assert.That(elementsToRemove.YCoordinates[i], Is.EqualTo(expectedElementsToRemove.YCoordinates[i]).Within(tolerance));
                    }
                }
                finally
                {
                    api.ClearState();
                    elementsToRemove?.Dispose();
                    expectedElementsToRemove?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliDerefinementElementsOnPolygonThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var polygon = new DisposableGeometryList();
                var elementsToRemove = new DisposableGeometryList();
                var expectedElementsToRemove = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));


                    polygon.NumberOfCoordinates = 4;
                    polygon.XCoordinates = new[] { 2.5, 7.5, 5.5, 2.5 };
                    polygon.YCoordinates = new[] { 2.5, 4.5, 8.5, 2.5 };
                    polygon.Values = new[] { 0.0, 0.0, 0.0, 0.0 };

                    elementsToRemove.NumberOfCoordinates = mesh.NumNodes;
                    elementsToRemove.XCoordinates = new double[mesh.NumNodes];
                    elementsToRemove.YCoordinates = new double[mesh.NumNodes];
                    for (var i = 0; i < elementsToRemove.NumberOfCoordinates; ++i)
                    {
                        elementsToRemove.XCoordinates[i] = elementsToRemove.YCoordinates[i] = 0.0;
                    }

                    Assert.That(api.Mesh2dCasulliDerefinementElementsOnPolygon(id, polygon, ref elementsToRemove), Is.EqualTo(0));


                    expectedElementsToRemove.NumberOfCoordinates = 5;
                    expectedElementsToRemove.XCoordinates = new[] { 2.5, 4.5, 4.5, 6.5, 6.5 };
                    expectedElementsToRemove.YCoordinates = new[] { 2.5, 4.5, 6.5, 4.5, 6.5 };


                    const double tolerance = 1.0e-12;
                    for (var i = 0; i < expectedElementsToRemove.NumberOfCoordinates; ++i)
                    {
                        Assert.That(elementsToRemove.XCoordinates[i], Is.EqualTo(expectedElementsToRemove.XCoordinates[i]).Within(tolerance));
                        Assert.That(elementsToRemove.YCoordinates[i], Is.EqualTo(expectedElementsToRemove.YCoordinates[i]).Within(tolerance));
                    }
                }
                finally
                {
                    api.ClearState();
                    polygon?.Dispose();
                    elementsToRemove?.Dispose();
                    expectedElementsToRemove?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliRefinementThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dCasulliRefinement(id), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh.NumNodes, Is.LessThan(mesh2D.NumNodes));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCasulliRefinementOnPolygonThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(11, 11, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                var polygon = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    polygon.NumberOfCoordinates = 4;
                    polygon.XCoordinates = new[] { 2.5, 7.5, 5.5, 2.5 };
                    polygon.YCoordinates = new[] { 2.5, 4.5, 8.5, 2.5 };
                    polygon.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    Assert.That(api.Mesh2dCasulliRefinementOnPolygon(id, polygon), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh.NumNodes, Is.LessThan(mesh2D.NumNodes));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                    polygon?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dComputeOrthogonalizationThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();

                    Assert.That(api.Mesh2dComputeOrthogonalization(id,
                                                                          ProjectToLandBoundaryOptions.ToOriginalNetBoundary,
                                                                          orthogonalizationParametersList,
                                                                          polygon,
                                                                          landBoundaries), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumNodes, Is.EqualTo(16));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeMeshesThroughApi()
        {
            // Setup
            using (DisposableMesh2D firstMesh = CreateMesh2D(3, 3, 1, 1))
            using (DisposableMesh2D secondMesh = CreateMesh2D(6, 6, 0.5, 0.5, 100.0, 100.0))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    api.Mesh2dSet(id, firstMesh);
                    api.Mesh2dGetData(id, out mesh2D);

                    // Execute
                    var result = api.Mesh2dMergeMeshes(id, secondMesh);

                    // Assert
                    Assert.That(result, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(72));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeAndConnectMeshesThroughApi()
        {
            // Setup
            using (DisposableMesh2D firstMesh = CreateMesh2D(3, 3, 1, 1))
            using (DisposableMesh2D secondMesh = CreateMesh2D(6, 6, 0.5, 0.5, 1.0))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    api.Mesh2dSet(id, firstMesh);
                    api.Mesh2dGetData(id, out mesh2D);

                    // Execute
                    const double searchFraction = 0.4;
                    var result = api.Mesh2dMergeAndConnectMeshes(id, secondMesh, searchFraction);

                    // Assert
                    Assert.That(result, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(73));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dConvertProjectionThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D meshInitial = null;
                var meshFinal = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // Get data of initial mesh
                    meshInitial = new DisposableMesh2D();
                    Assert.That(api.Mesh2dGetData(id, out meshInitial), Is.EqualTo(0));
                    Assert.That(meshInitial, Is.Not.Null);

                    {
                        // Set the zone string
                        const string zone = "+proj=utm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";

                        // The mesh projection is initially cartesian, convert to spherical
                        Assert.That(api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone), Is.EqualTo(0));
                        int projection = -1;
                        Assert.That(api.GetProjection(id, ref projection), Is.EqualTo(0));
                        Assert.That((ProjectionOptions)projection, Is.EqualTo(ProjectionOptions.Spherical));

                        // Get data of final mesh
                        Assert.That(api.Mesh2dGetData(id, out meshFinal), Is.EqualTo(0));
                        Assert.That(meshFinal, Is.Not.Null);

                        // It must be different than the initial mesh
                        Assert.That(meshFinal.NodeX, Is.Not.EqualTo(meshInitial.NodeX));
                        Assert.That(meshFinal.NodeY, Is.Not.EqualTo(meshInitial.NodeY));

                        // Round trip, convert back to cartesian
                        Assert.That(api.Mesh2dConvertProjection(id, ProjectionOptions.Cartesian, zone), Is.EqualTo(0));
                        Assert.That(api.GetProjection(id, ref projection), Is.EqualTo(0));
                        Assert.That((ProjectionOptions)projection, Is.EqualTo(ProjectionOptions.Cartesian));
                    }

                    // Get data of final mesh
                    Assert.That(api.Mesh2dGetData(id, out meshFinal), Is.EqualTo(0));
                    Assert.That(meshFinal, Is.Not.Null);

                    // Compare meshes the initial and final meshes

                    // First comparison: all array sizes must be equal
                    Assert.That(meshFinal.NumNodes, Is.EqualTo(meshInitial.NumNodes));
                    Assert.That(meshFinal.NumFaces, Is.EqualTo(meshInitial.NumFaces));
                    Assert.That(meshFinal.NumEdges, Is.EqualTo(meshInitial.NumEdges));
                    Assert.That(meshFinal.NumFaceNodes, Is.EqualTo(meshInitial.NumFaceNodes));

                    // Second comparison: all integer arrays must be equal
                    Assert.That(meshFinal.EdgeFaces, Is.EqualTo(meshInitial.EdgeFaces));
                    Assert.That(meshFinal.EdgeNodes, Is.EqualTo(meshInitial.EdgeNodes));
                    Assert.That(meshFinal.FaceEdges, Is.EqualTo(meshInitial.FaceEdges));
                    Assert.That(meshFinal.FaceNodes, Is.EqualTo(meshInitial.FaceNodes));
                    Assert.That(meshFinal.NodesPerFace, Is.EqualTo(meshInitial.NodesPerFace));

                    // Third comparison: all double arrays must be equal within a tolerance
                    {
                        const double tolerance = 1.0e-6;
                        Assert.That(meshInitial.NodeX, Is.EqualTo(meshFinal.NodeX).Within(tolerance));
                        Assert.That(meshInitial.NodeY, Is.EqualTo(meshFinal.NodeY).Within(tolerance));
                        Assert.That(meshInitial.EdgeX, Is.EqualTo(meshFinal.EdgeX).Within(tolerance));
                        Assert.That(meshInitial.EdgeY, Is.EqualTo(meshFinal.EdgeY).Within(tolerance));
                        Assert.That(meshInitial.FaceX, Is.EqualTo(meshFinal.FaceX).Within(tolerance));
                        Assert.That(meshInitial.FaceY, Is.EqualTo(meshFinal.FaceY).Within(tolerance));
                    }
                }
                finally
                {
                    api.ClearState();
                    meshInitial?.Dispose();
                    meshFinal.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dConvertProjectionThroughApiFails()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D meshInitial = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // Get data of initial mesh
                    meshInitial = new DisposableMesh2D();
                    Assert.That(api.Mesh2dGetData(id, out meshInitial), Is.EqualTo(0));
                    Assert.That(meshInitial, Is.Not.Null);

                    {
                        int expectedExitCode = -1;
                        api.GetExitCodeStdLibException(ref expectedExitCode);

                        // Missing projection key-value pair in string
                        var zone = "+lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        int exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.That(exitCode, Is.EqualTo(expectedExitCode));

                        // Missing projection value pair in string
                        zone = "+proj= +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.That(exitCode, Is.EqualTo(expectedExitCode));

                        // Use invalid projection value in string
                        zone = "+proj=ratm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.That(exitCode, Is.EqualTo(expectedExitCode));

                        // Use invalid zone value in string
                        zone = "+proj=utm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=666";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.That(exitCode, Is.EqualTo(expectedExitCode));
                    }
                }
                finally
                {
                    api.ClearState();
                    meshInitial.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetHangingEdgesThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int[] hangingEdges = Array.Empty<int>();
                    Assert.That(api.Mesh2dGetHangingEdges(id, out hangingEdges), Is.EqualTo(0));
                    Assert.That(hangingEdges.Length, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }


        public void Mesh2dGetSmallFlowEdgeCentersThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableGeometryList disposableGeometryList = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var smallFlowEdgesLengthThreshold = 1000.0; //Large threshold: all edges will be included
                    int numSmallFlowEdges = -1;

                    Assert.That(api.Mesh2dCountSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges), Is.EqualTo(0));
                    disposableGeometryList = new DisposableGeometryList();
                    disposableGeometryList.XCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.YCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.NumberOfCoordinates = numSmallFlowEdges;

                    // Execute
                    Assert.That(api.Mesh2dGetSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref disposableGeometryList), Is.EqualTo(0));
                    // Assert
                    Assert.That(disposableGeometryList.XCoordinates.Length, Is.EqualTo(12));
                }
                finally
                {
                    api.ClearState();
                    disposableGeometryList?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dDeleteHangingEdgesThroughApi()
        {
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(24));

                    Assert.That(api.Mesh2dDeleteHangingEdges(id), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumEdges, Is.EqualTo(24)); // No hanging edges found
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dDeleteSmallFlowEdgesAndSmallTrianglesThroughApi()
        {
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    var smallFlowEdgesLengthThreshold = 10.0;
                    var minFractionalAreaTriangles = 0.01;

                    // Execute
                    Assert.That(api.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(id, smallFlowEdgesLengthThreshold, minFractionalAreaTriangles), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumValidEdges, Is.EqualTo(12));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetObtuseTrianglesMassCentersThroughApi()
        {
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var disposableGeometryList = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int numObtuseTriangles = -1;
                    Assert.That(api.Mesh2dCountObtuseTriangles(id, ref numObtuseTriangles), Is.EqualTo(0));
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.NumberOfCoordinates = numObtuseTriangles;

                    Assert.That(api.Mesh2dGetObtuseTrianglesMassCenters(id, ref disposableGeometryList), Is.EqualTo(0));
                    Assert.That(disposableGeometryList.XCoordinates.Length, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    disposableGeometryList.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dIntersectionsFromPolygonThroughApi()
        {
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 1.0, 1.0))
            using (var api = new MeshKernelApi())
            using (var disposableGeometryList = new DisposableGeometryList())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    disposableGeometryList.XCoordinates = new[] { 0.6, 0.6, 1.6, 1.6, 0.6 };
                    disposableGeometryList.XCoordinates = new[] { 2.5, 0.5, 0.5, 2.5, 2.5 };
                    disposableGeometryList.NumberOfCoordinates = 5;

                    int[] edgeNodes = Array.Empty<int>();
                    int[] edgeIndex = Array.Empty<int>();
                    double[] edgeDistances = Array.Empty<double>();
                    double[] segmentDistances = Array.Empty<double>();
                    int[] segmentIndexes = Array.Empty<int>();
                    int[] faceIndexes = Array.Empty<int>();
                    int[] faceNumEdges = Array.Empty<int>();
                    int[] faceEdgeIndex = Array.Empty<int>();

                    Assert.That(api.Mesh2dIntersectionsFromPolygon(id,
                                                                          disposableGeometryList,
                                                                          ref edgeNodes,
                                                                          ref edgeIndex,
                                                                          ref edgeDistances,
                                                                          ref segmentDistances,
                                                                          ref segmentIndexes,
                                                                          ref faceIndexes,
                                                                          ref faceNumEdges,
                                                                          ref faceEdgeIndex), Is.EqualTo(0));

                    Assert.That(segmentIndexes[0], Is.EqualTo(0));
                    Assert.That(segmentIndexes[1], Is.EqualTo(0));

                    Assert.That(segmentDistances[0], Is.EqualTo(0.25));
                    Assert.That(segmentDistances[1], Is.EqualTo(0.75));

                    Assert.That(faceIndexes[0], Is.EqualTo(6));
                    Assert.That(faceIndexes[1], Is.EqualTo(3));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeRectangularMeshThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2d = null;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridType = 0;
                    makeGridParameters.NumberOfColumns = 3;
                    makeGridParameters.NumberOfRows = 3;
                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = 10.0;
                    makeGridParameters.YGridBlockSize = 10.0;

                    Assert.That(api.Mesh2dMakeRectangularMesh(id, makeGridParameters), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d, Is.Not.Null);
                    Assert.That(mesh2d.NumNodes, Is.EqualTo(16));
                }
                finally
                {
                    api.ClearState();
                    mesh2d?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeRectangularMeshFromPolygonThroughAPIFails()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.XGridBlockSize = 1.0;
                    makeGridParameters.YGridBlockSize = 1.0;

                    // geometry list is empty, expect an algorithm error to be thrown in the backend
                    int algorithmErrorExitCode = -1;
                    api.GetExitCodeAlgorithmError(ref algorithmErrorExitCode);
                    Assert.That(api.Mesh2dMakeRectangularMeshFromPolygon(id, makeGridParameters, polygon), Is.EqualTo(algorithmErrorExitCode));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void Mesh2dMakeRectangularMeshFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            {
                var id = 0;
                DisposableMesh2D mesh2d = null;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.XGridBlockSize = 1.0;
                    makeGridParameters.YGridBlockSize = 1.0;

                    polygon.XCoordinates = new[] { 0.5, 2.5, 5.5, 3.5, 0.5 };
                    polygon.YCoordinates = new[] { 2.5, 0.5, 3.0, 5.0, 2.5 };
                    polygon.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
                    polygon.NumberOfCoordinates = 5;

                    Assert.That(api.Mesh2dMakeRectangularMeshFromPolygon(id, makeGridParameters, polygon), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d, Is.Not.Null);
                    Assert.That(mesh2d.NumNodes, Is.EqualTo(9));
                }
                finally
                {
                    api.ClearState();
                    mesh2d?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeRectangularMeshOnExtensionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridType = 0;
                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = 10.0;
                    makeGridParameters.YGridBlockSize = 10.0;
                    makeGridParameters.UpperRightCornerXCoordinate = 100.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 100.0;

                    // Execute
                    Assert.That(api.Mesh2dMakeRectangularMeshOnExtension(id, makeGridParameters), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    Assert.That(mesh2D.NumNodes, Is.EqualTo(121));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dRefineBasedOnFloatGriddedSamplesThroughAPI()
        {
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            {
                var id = 0;
                DisposableMesh2D meshOut = null;
                var griddedSamples = new DisposableGriddedSamples<float>(6, 7, 0, 0, 0, (int)InterpolationTypes.Float);
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();

                    double coordinate = -50.0;
                    var dx = 100.0;
                    for (var i = 0; i < griddedSamples.NumX; i++)
                    {
                        griddedSamples.CoordinatesX[i] = coordinate + (i * dx);
                    }

                    coordinate = -50.0;
                    var dy = 100.0;
                    for (var i = 0; i < griddedSamples.NumY; ++i)
                    {
                        griddedSamples.CoordinatesY[i] = coordinate + (i * dy);
                    }

                    for (var i = 0; i < griddedSamples.Values.Length; ++i)
                    {

                        griddedSamples.Values[i] = -0.05f;
                    }

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.Mesh2dRefineBasedOnGriddedSamples(id,
                                                                             griddedSamples,
                                                                             meshRefinementParameters,
                                                                             true), Is.EqualTo(0));
                    meshOut = new DisposableMesh2D();
                    Assert.That(api.Mesh2dGetData(id, out meshOut), Is.EqualTo(0));
                    // Assert
                    Assert.That(meshOut, Is.Not.Null);
                    Assert.That(meshOut.NumNodes, Is.EqualTo(16));
                }
                finally
                {
                    api.ClearState();
                    meshOut?.Dispose();
                    griddedSamples.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dRefineRidgesBasedOnGriddedSamplesThroughAPI()
        {
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(4, 
                                                        4, 
                                                        100,
                                                        200))
            {
                var id = 0;
                var griddedSamples = new DisposableGriddedSamples<double>(6, 7, 0, 0, 0, (int)InterpolationTypes.Double);
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var meshRefinementParameters = new MeshRefinementParameters
                    {
                        MaxNumRefinementIterations = 10,
                        RefineIntersected = false,
                        UseMassCenterWhenRefining = false,
                        MinEdgeSize = 2.0,
                        RefinementType = MeshRefinementTypes.RidgeDetection,
                        ConnectHangingNodes = true,
                        AccountForSamplesOutside = false,
                        SmoothingIterations = 5,
                        MaxCourantTime = 120.0,
                        DirectionalRefinement = false
                    };


                    double coordinate = -50.0;
                    double dx = 100.0;
                    for (var i = 0; i < griddedSamples.NumX; i++)
                    {
                        griddedSamples.CoordinatesX[i] = coordinate + (i * dx);
                    }

                    coordinate = -50.0;
                    double dy = 100.0;
                    for (var j = 0; j < griddedSamples.NumY; j++)
                    {
                        griddedSamples.CoordinatesY[j] = coordinate + (j * dy);
                    }

                    // Parameters for Gaussian bump
                    double amplitude = 1.0;
                    double sigma = 100.0; // Controls the width of the bump
                    double centerX = 0.0;
                    double centerY = 0.0;

                    // Fill values with Gaussian bump
                    for (int j = 0; j < griddedSamples.NumY; j++)
                    {
                        double y = griddedSamples.CoordinatesY[j];
                        for (int i = 0; i < griddedSamples.NumX; i++)
                        {
                            double x = griddedSamples.CoordinatesX[i];

                            // 2D Gaussian function
                            double value = amplitude * Math.Exp(-((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY)) / (2 * sigma * sigma));

                            griddedSamples.Values[j * griddedSamples.NumX + i] = value;
                        }
                    }

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out DisposableMesh2D startMesh), Is.EqualTo(0));
                    Assert.That(startMesh.NumNodes, Is.EqualTo(16));
                    // Execute
                    Assert.That(api.Mesh2dRefineRidgesBasedOnGriddedSamples(id,
                                                                                  griddedSamples,
                                                                                  meshRefinementParameters,
                                                                               1.0,
                                                                               1,
                                                                               10), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out DisposableMesh2D endMesh), Is.EqualTo(0));
                    // Assert refinement executed
                    Assert.That(endMesh.NumNodes, Is.EqualTo(758));
                }
                finally
                {
                    api.ClearState();
                    griddedSamples.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dTriangulationInterpolationThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(3, 3, 1, 1))
            using (var samples = new DisposableGeometryList())
            {
                var id = 0;
                var results = new DisposableGeometryList();
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    int locationType = -1;
                    Assert.That(api.GetNodesLocationType(ref locationType), Is.EqualTo(0));
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new[] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    results.XCoordinates = mesh2D.NodeX;
                    results.YCoordinates = mesh2D.NodeY;
                    results.Values = new double[mesh.NumNodes];
                    results.NumberOfCoordinates = mesh.NumNodes;

                    // Execute
                    Assert.That(api.Mesh2dTriangulationInterpolation(id, samples, locationType, ref results), Is.EqualTo(0));
                    // Assert
                    Assert.That(results.Values[4], Is.EqualTo(3));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                    results.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCountHangingEdgesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(3, 3, 1, 1))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    int numHangingEdges = -1;
                    Assert.That(api.Mesh2dCountHangingEdges(id, ref numHangingEdges), Is.EqualTo(0));
                    Assert.That(numHangingEdges, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        // The starting mesh looks as shown below. The grid spacing is uniform and equal to 1.
        // The asterisks represent the vertices of a polygon. See the body of the test for the
        // actual position of the vertices (they do not coincide with the cell centers).
        // 30--31--32--33--34--35
        // |   |   |   |   |   |
        // 24--25--26--27--28--29
        // |   | * |   | * |   |
        // 18--19--20--12--22--23
        // |   |   |   |   |   |
        // 12--13--14--15--16--17
        // |   | * |   | * |   |
        // 6---7---8---9---10--11
        // |   |   |   |   |   |
        // 0---1---2---3---4---5
        // nodes   6 * 6 = 36
        // edges   2 * 5 = 60
        // faces   5 * 5 = 25
        // The test is parametrized by all possible combinations of deletion options and selection inversion
        [TestCase(DeleteMeshInsidePolygonOptions.NotIntersecting,
                  true,
                  4,
                  4,
                  1)]
        // Case 1: should keep the central cell
        //  20--21
        //  |   |
        //  14--15
        // nodes = 4
        // edges = 1
        // faces = 1
        [TestCase(DeleteMeshInsidePolygonOptions.Intersecting,
                  true,
                  16,
                  24,
                  9)]
        // Case 2: should keep 3x3 central cells
        //  25--26--27--28
        //  |   |   |   |
        //  19--20--21--22
        //  |   |   |   |
        //  13--14--15--16
        //  |   |   |   |
        //  7---8---9---10
        // nodes = 4 * 4 = 16
        // edges = 2 * (3 * 4) = 24
        // faces = 1
        [TestCase(DeleteMeshInsidePolygonOptions.FacesWithIncludedCircumcenters,
                  true,
                  16,
                  24,
                  9)]
        // Case 3: should deletes all the outer cells
        //  25--26--27--28
        //  |   |   |   |
        //  19--20--21--22
        //  |   |   |   |
        //  13--14--15--16
        //  |   |   |   |
        //  7---8---9---10
        // nodes = 16
        // faces = 9
        [TestCase(DeleteMeshInsidePolygonOptions.NotIntersecting,
                  false,
                  36,
                  60,
                  25)]
        // Case 4: should keep all cells but the central cell, i.e. delete the central face
        //  30--31--32--33--34--35
        //  |   |   |   |   |   |
        //  24--25--26--27--28--29
        //  |   |   |   |   |   |
        //  18--19--20--21--22--23
        //  |   |   | / |   |   |
        //  12--13--14--15--16--17
        //  |   |   |   |   |   |
        //  6---7---8---9---10--11
        //  |   |   |   |   |   |
        //  0---1---2---3---4---5
        // nodes = 6 * 6 = 36 (no change)
        // edges = 2 * (5 * 6) = 60 (no change)
        // faces = 5 * 5 - 1 = 24
        [TestCase(DeleteMeshInsidePolygonOptions.Intersecting,
                  false,
                  32,
                  48,
                  16)]
        // Case 5: should delete 3x3 central cells
        //  30--31--32--33--34--35
        //  |   |   |   |   |   |
        //  24--25--26--27--28--29
        //  |   |           |   |
        //  18--19          22--23
        //  |   |           |   |
        //  12--13          16--17
        //  |   |           |   |
        //  6---7---8---9---10--11
        //  |   |   |   |   |   |
        //  0---1---2---3---4---5
        // nodes = 6 * 6 - 4 (central cell) = 32
        // faces = 25 - (3 * 3) = 16
        [TestCase(DeleteMeshInsidePolygonOptions.FacesWithIncludedCircumcenters,
                  false,
                  32,
                  48,
                  16)]
        // Case 6: should delete 3x3 central cells
        //  30--31--32--33--34--35
        //  |   |   |   |   |   |
        //  24--25--26--27--28--29
        //  |   |           |   |
        //  18--19          22--23
        //  |   |           |   |
        //  12--13          16--17
        //  |   |           |   |
        //  6---7---8---9---10--11
        //  |   |   |   |   |   |
        //  0---1---2---3---4---5
        // nodes = 6 * 6 - 4 (central cell) = 32
        // faces = 25 - (3 * 3) = 16
        public void Mesh2dDeleteInsidePolygon(DeleteMeshInsidePolygonOptions deleteMeshInsidePolygonOptions,
                                              bool invertSelection,
                                              int expectedNumNodes,
                                              int expectedNunEdges,
                                              int expectedNumFaces)
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(5, 5, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2d = null;
                var polygon = new DisposableGeometryList();
                try
                {
                    polygon.NumberOfCoordinates = 5;
                    polygon.XCoordinates = new[] { 1.25, 3.75, 3.75, 1.25, 1.25 };
                    polygon.XCoordinates = new[] { 1.25, 1.25, 3.75, 3.75, 1.25 };

                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(api.Mesh2dDelete(id,
                                                        in polygon,
                                                        deleteMeshInsidePolygonOptions,
                                                        invertSelection), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh.NumNodes, Is.Not.EqualTo(expectedNumNodes));
                    Assert.That(mesh.NumEdges, Is.Not.EqualTo(expectedNunEdges));
                    Assert.That(mesh.NumFaces, Is.Not.EqualTo(expectedNumFaces));
                }
                finally
                {
                    api.ClearState();
                    mesh2d?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeGlobalThroughApi()
        {
            // Generates a mesh in spherical coordinates around the globe

            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableMesh2D mesh2d = null;
                try
                {
                    int projectionType = 1;
                    id = api.AllocateState(projectionType);
                    Assert.That(api.Mesh2dMakeGlobal(id, 19, 25), Is.EqualTo(0));

                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NumEdges, Is.EqualTo(1225));
                    Assert.That(mesh2d.NumNodes, Is.EqualTo(621));
                }
                finally
                {
                    api.ClearState();
                    mesh2d?.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dUndoThenRedoTwoDeleteNodesThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2d = new DisposableMesh2D();
                try
                {
                    int numberOfVerticesBefore = mesh.NumNodes;
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // Do
                    Assert.That(api.Mesh2dDeleteNode(id, 0), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[0], Is.EqualTo(-999.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 1));
                    Assert.That(api.Mesh2dDeleteNode(id, 6), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[6], Is.EqualTo(-999.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 2));

                    // Un-do
                    bool undone = false;
                    int meshKernelId = -1;
                    Assert.That(api.UndoState(ref undone, ref meshKernelId), Is.EqualTo(0));
                    Assert.That(undone, Is.EqualTo(true));
                    Assert.That(meshKernelId, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[6], Is.EqualTo(100.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 1));

                    meshKernelId = -1;
                    undone = false;
                    Assert.That(api.UndoState(ref undone, ref meshKernelId), Is.EqualTo(0));
                    Assert.That(undone, Is.EqualTo(true));
                    Assert.That(meshKernelId, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[0], Is.EqualTo(0.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore));

                    // Re-do
                    meshKernelId = -1;
                    bool redone = false;
                    Assert.That(api.RedoState(ref redone, ref meshKernelId), Is.EqualTo(0));
                    Assert.That(redone, Is.EqualTo(true));
                    Assert.That(meshKernelId, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[0], Is.EqualTo(-999.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 1));

                    meshKernelId = -1;
                    redone = false;
                    Assert.That(api.RedoState(ref redone, ref meshKernelId), Is.EqualTo(0));
                    Assert.That(redone, Is.EqualTo(true));
                    Assert.That(meshKernelId, Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));
                    Assert.That(mesh2d.NodeX[6], Is.EqualTo(-999.0));
                    Assert.That(mesh2d.NumValidNodes, Is.EqualTo(numberOfVerticesBefore - 2));
                }
                finally
                {
                    api.ClearState();
                    mesh2d.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetEdgeLocationIndexThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetEdgeLocationIndex(id,
                                                   10.0,
                                                   0.0,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(12));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void Mesh2dGetFaceLocationIndexThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetFaceLocationIndex(id,
                                                   10.0,
                                                   0.0,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }


        [TestCase(10.0,0.0,1)]
        [TestCase(0.0,10.0,4)]
        public void Mesh2dGetNodeLocationIndexThroughApi(double x, double y, int expectedIndex)
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetNodeLocationIndex(id,
                                                   x,
                                                   y,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(expectedIndex));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }



        [TestCase(20.0, 16.0, 6)]
        [TestCase(20.0, 23.0, 10)]
        [TestCase(17.0, 20.0, 19)]
        [TestCase(24.0, 20.0, 20)]
        public void CurvilinearGetEdgeLocationIndexThroughApi(double x, double y, int expectedIndex)
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    var square = new MakeGridParameters()
                    {
                        NumberOfColumns = 3,
                        NumberOfRows = 3,
                        XGridBlockSize = 10,
                        YGridBlockSize = 10
                    };
                    api.CurvilinearComputeRectangularGrid(id, square);

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int actualIndex = -1;
                    api.CurvilinearGetEdgeLocationIndex(id, x, y, boundingBox, ref actualIndex);

                    Assert.That(actualIndex, Is.EqualTo(expectedIndex));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void CurvilinearGetFaceLocationIndexThroughApi()
        {
            // Setup
            using (DisposableCurvilinearGrid curvilinearGrid = CreateCurvilinearGrid(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, curvilinearGrid), Is.EqualTo(0));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.CurvilinearGetFaceLocationIndex(id,
                                                        20.0,
                                                        20.0,
                                                        boundingBox,
                                                        ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(4));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }


        [TestCase(20.0, 20.0, 10)]
        [TestCase(20.5, 23, 10)]
        [TestCase(15.5, 20.0, 10)]
        [TestCase(30.7, 40.7, 15)]
        [TestCase(29.7, 29.7, 15)]
        public void CurvilinearGetNodeLocationIndexThroughApi(double x, double y, int expectedIndex)
        {
            // Setup
            using (DisposableCurvilinearGrid curvilinearGrid = CreateCurvilinearGrid(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, curvilinearGrid), Is.EqualTo(0));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.CurvilinearGetNodeLocationIndex(id, x, y,
                                                        boundingBox,
                                                        ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(expectedIndex));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void Mesh2dConvertCurvilinearThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(10, 10, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                var meshOut = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));


                    api.Mesh2dConvertCurvilinear(id, 5, 5);

                    // Get curvilinear grid data after conversion
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((10, 10)));

                    // Get mesh data after conversion
                    Assert.That(api.Mesh2dGetData(id, out meshOut), Is.EqualTo(0));
                    Assert.That(meshOut.NumNodes, Is.EqualTo(100));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                    meshOut?.Dispose();
                }
            }
        }

        [TestCase(0,8,5)]
        [TestCase(4,8,5)]
        [TestCase(8,8,5)]
        [TestCase(9,11,7)]
        [TestCase(16,11,7)]
        public void Mesh2dSplitEdgesThroughApi(int edgeId, int expectedExtraEdges, int expectedExtraValidEdges)
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableMesh2D mesh = CreateMesh2D(4, 3, 10, 10))
            {
                DisposableMesh2D mesh2D = null;
                try
                {
                    // Setup
                    int id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));
                    Assert.That(mesh.NumEdges,Is.EqualTo(3*3 + 2*4));
                    int numberOfValidEdges = ComputeNumberOfValidEdges(mesh); 
                    Assert.That(numberOfValidEdges, Is.EqualTo(mesh.NumEdges));
                    
                    // Execute
                    Assert.That(api.Mesh2dSplitEdges(id, mesh.GetFirstNode(edgeId), mesh.GetLastNode(edgeId)), Is.EqualTo(0));
                    
                    // Assert
                    Assert.That(api.Mesh2dGetData(id, out mesh2D), Is.EqualTo(0));
                    int extraEdges = mesh2D.NumEdges - mesh.NumEdges;
                    Assert.That(extraEdges, Is.EqualTo(expectedExtraEdges));
                    int newNumberOfValidEdges = ComputeNumberOfValidEdges(mesh2D);
                    int extraValidEdges = newNumberOfValidEdges - numberOfValidEdges;
                    Assert.That(extraValidEdges, Is.EqualTo(expectedExtraValidEdges));
                }
                finally
                {
                    api.ClearState();
                    mesh2D?.Dispose();
                }
            }

            int ComputeNumberOfValidEdges(DisposableMesh2D mesh)
            {
                int count = 0;
                for (int i = 0; i < mesh.NumEdges; ++i)
                {
                    if (mesh.EdgeNodes[2 * i] > -1 && mesh.EdgeNodes[2 * i + 1] > -1) 
                        ++count;
                }

                return count;
            }
        }

        [Test]
        public void Mesh2dSnapToAnEmptyLandBoundaryThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(10, 10, 10, 10))
            using (var selectingPolygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var meshOut = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    api.Mesh2dSnapToLandBoundary(id, in selectingPolygon, in landBoundaries);

                    // Get mesh data after conversion
                    Assert.That(api.Mesh2dGetData(id, out meshOut), Is.EqualTo(0));
                    Assert.That(meshOut.NumNodes, Is.EqualTo(100));
                    Assert.That(meshOut.NodeX[0], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeX[1], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeY[0], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeY[1], Is.EqualTo(10.0));
                }
                finally
                {
                    api.ClearState();
                    meshOut?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dSnapToLandBoundaryThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(10, 10, 10, 10))
            using (var selectingPolygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var meshOut = new DisposableMesh2D();
                try
                {
                    // prepare
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    selectingPolygon.XCoordinates = new[] { -10.0, 11.0, 15.0, -10.0, -10.0 };
                    selectingPolygon.YCoordinates = new[] { -10.0, -10.0, 15.0, 15.0, -10.0 };
                    selectingPolygon.NumberOfCoordinates = selectingPolygon.XCoordinates.Length;

                    landBoundaries.XCoordinates = new[] { -1.0, 11.0 };
                    landBoundaries.YCoordinates = new[] { -1.0, 11.0 };
                    landBoundaries.NumberOfCoordinates = landBoundaries.XCoordinates.Length;

                    // execute
                    api.Mesh2dSnapToLandBoundary(id, in selectingPolygon, in landBoundaries);

                    // assert
                    Assert.That(api.Mesh2dGetData(id, out meshOut), Is.EqualTo(0));
                    Assert.That(meshOut.NumNodes, Is.EqualTo(100));
                    Assert.That(meshOut.NodeX[0], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeX[1], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeY[0], Is.EqualTo(0.0));
                    Assert.That(meshOut.NodeY[1], Is.EqualTo(10.0));
                }
                finally
                {
                    api.ClearState();
                    meshOut?.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dExpungeStateThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(10, 10, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh0 = new DisposableMesh2D();
                var mesh1 = new DisposableMesh2D();
                try
                {
                    // prepare
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // execute
                    Assert.That(api.Mesh2dGetData(id, out mesh0), Is.EqualTo(0));
                    Assert.That(api.ExpungeState(id), Is.EqualTo(0));
                    Assert.That(api.Mesh2dGetData(id, out mesh1), Is.EqualTo(1)); //Once the id is expunged, no data can be retrieved and the exitcode is 1

                    // assert
                    Assert.That(mesh0.NumNodes, Is.EqualTo(100));
                    Assert.That(mesh1.NumNodes, Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    mesh1?.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetNodeEdgeDataThroughApi()
        {

            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(7, 10, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var meshRetrieved = new DisposableMesh2D();
                try
                {
                    // prepare
                    id = api.AllocateState(0);
                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    // execute Mesh2dGetNodeEdgeData
                    Assert.That(api.Mesh2dGetNodeEdgeData(id, out meshRetrieved), Is.EqualTo(0));

                    // assert non empty
                    Assert.That(meshRetrieved.NumNodes, Is.EqualTo(70));
                    Assert.That(meshRetrieved.NumValidNodes, Is.EqualTo(70));
                    Assert.That(meshRetrieved.NumEdges, Is.EqualTo(123));
                    Assert.That(meshRetrieved.NodeX.Length, Is.EqualTo(70));
                    Assert.That(meshRetrieved.NodeY.Length, Is.EqualTo(70));
                    Assert.That(meshRetrieved.EdgeNodes.Length, Is.EqualTo(246));

                    // assert null or empty
                    Assert.That(meshRetrieved.EdgeFaces, Is.Null);
                    Assert.That(meshRetrieved.EdgeX, Is.Null);
                    Assert.That(meshRetrieved.EdgeY, Is.Null);
                    Assert.That(meshRetrieved.FaceEdges, Is.Null);
                    Assert.That(meshRetrieved.FaceNodes, Is.Null);
                    Assert.That(meshRetrieved.FaceX, Is.Null);
                    Assert.That(meshRetrieved.FaceY, Is.Null);
                    Assert.That(meshRetrieved.NumFaceNodes, Is.EqualTo(0));
                    Assert.That(meshRetrieved.NumFaces, Is.EqualTo(0));
                    Assert.That(meshRetrieved.NodesPerFace, Is.Null);
                }
                finally
                {
                    api.ClearState();
                    meshRetrieved?.Dispose();
                }
            }
        }
    }

}
