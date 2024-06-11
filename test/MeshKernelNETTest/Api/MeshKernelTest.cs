using System;
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    Assert.AreEqual(0, api.Mesh2dDeleteNode(id, 0));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreNotEqual(2, mesh.NumValidEdges);
                    Assert.AreEqual(numberOfVerticesBefore - 1, mesh2d.NumValidNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    Assert.AreEqual(0, api.Mesh2dFlipEdges(id, true, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, geometryListIn, landBoundaries));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 9, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    var newEdgeIndex = 0;
                    Assert.AreEqual(0, api.Mesh2dInsertEdge(id, 4, 1, ref newEdgeIndex));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 1, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dInsertEdgeFromCoordinatesThroughApi()
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    var firstNewNodeIndex = 0;
                    var secondNewNodeIndex = 0;
                    var newEdgeIndex = 0;
                    Assert.AreEqual(0, api.Mesh2dInsertEdgeFromCoordinates(id,
                                                                           0.0,
                                                                           400.0,
                                                                           100.0,
                                                                           600.0,
                                                                           ref firstNewNodeIndex,
                                                                           ref secondNewNodeIndex,
                                                                           ref newEdgeIndex));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 1, mesh2D.NumEdges);
                    Assert.AreEqual(2, firstNewNodeIndex);
                    Assert.AreEqual(7, secondNewNodeIndex);
                    Assert.AreEqual(24, newEdgeIndex);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMergeTwoNodesThroughApi()
        {
            // Before                                          After
            // 0 ------- 1 ------- 2 ------- 3                           0 ------- 1 ------- 2
            // |         |         |         |                        .  |         |         |
            // |         |         |         |                      .    |         |         |
            // |         |         |         |                    .      |         |         |
            // |         |         |         |                  .        |         |         |
            // 4 ------- 5 ------- 6 ------- 7                 3 ------- 4 ------- 5 ------- 6
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                 7 ------- 8 ------  9 ------ 10
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |  
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                 11 ------ 12 ------ 13 ------ 14

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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    Assert.AreEqual(0, api.Mesh2dMergeTwoNodes(id, 0, 4));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore - 1, mesh2D.NumValidEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var geometryList = new DisposableGeometryList();
                    Assert.AreEqual(0, api.Mesh2dMergeNodes(id, geometryList));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var geometryList = new DisposableGeometryList();
                    Assert.AreEqual(0, api.Mesh2dMergeNodesWithMergingDistance(id, geometryList, 0.001));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    Assert.AreEqual(0, api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                Assert.AreEqual(0, api.GetSplines(geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices));
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

                    Assert.AreEqual(0, api.Mesh2dMakeTriangularMeshFromPolygon(id, geometryListIn));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dMakeTriangularMeshFromSamples(id, geometryListIn));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int numberOfPolygonVertices = -1;
                    Assert.AreEqual(0, api.Mesh2dCountMeshBoundariesAsPolygons(id, ref numberOfPolygonVertices));
                    Assert.AreEqual(13, numberOfPolygonVertices);

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.Values = new double[numberOfPolygonVertices];
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = numberOfPolygonVertices;

                    Assert.AreEqual(0, api.Mesh2dGetMeshBoundariesAsPolygons(id, ref geometryListIn));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.PolygonCountOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref numberOfPolygonVertices));
                    Assert.AreEqual(5, numberOfPolygonVertices);
                    Assert.AreEqual(0, api.PolygonGetOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref disposableGeometryListOut));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.PolygonCountRefine(id,
                                                              geometryListIn,
                                                              firstIndex,
                                                              secondIndex,
                                                              distance,
                                                              ref numberOfPolygonVertices));

                    geometryListOut.GeometrySeparator = geometrySeparator;
                    geometryListOut.NumberOfCoordinates = numberOfPolygonVertices;

                    geometryListOut.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.Values = new double[numberOfPolygonVertices];

                    Assert.AreEqual(0, api.PolygonRefine(id,
                                                         geometryListIn,
                                                         firstIndex,
                                                         secondIndex,
                                                         distance,
                                                         ref geometryListOut));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
                    geometryListOut.Dispose();
                    mesh2D.Dispose();
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { 50.0, 150.0, 250.0, 50.0, 150.0, 250.0, 50.0, 150.0, 250.0 };

                    geometryListIn.YCoordinates = new[] { 50.0, 50.0, 50.0, 150.0, 150.0, 150.0, 250.0, 250.0, 250.0 };

                    geometryListIn.Values = new[] { 2.0, 2.0, 2.0, 3.0, 3.0, 3.0, 4.0, 4.0, 4.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    var relativeSearchRadius = 1.0;
                    var minimumNumSamples = 1;
                    Assert.AreEqual(0, api.Mesh2dRefineBasedOnSamples(id, geometryListIn, relativeSearchRadius, minimumNumSamples, meshRefinementParameters));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
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
                    Assert.AreEqual(0, api.Mesh2dRefineBasedOnPolygon(id, geometryListIn, meshRefinementParameters));
                    //Assert
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    // Get data of initial mesh
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshInitial));
                    Assert.IsNotNull(meshInitial);

                    var rotationCentre = new[] { 1.0, 5.0 };
                    const double rotationAngle = 45.0;
                    Assert.AreEqual(0, api.Mesh2dRotate(id,
                                                        rotationCentre[0],
                                                        rotationCentre[1],
                                                        rotationAngle));

                    // Get data of rotated mesh
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshRotated));
                    Assert.IsNotNull(meshRotated);

                    // Compute expected nodes
                    double[,] expectedNodes = RotateNodes(meshInitial.NodeX, meshInitial.NodeY, rotationCentre, rotationAngle);

                    // Expected and actual values must be equal within a tolerance
                    const double tolerance = 1.0e-6;
                    for (var i = 0; i < meshInitial.NumNodes; i++)
                    {
                        Assert.AreEqual(expectedNodes[i, 0], meshRotated.NodeX[i], tolerance);
                        Assert.AreEqual(expectedNodes[i, 1], meshRotated.NodeY[i], tolerance);
                    }
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    // Get data of initial mesh
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshInitial));
                    Assert.IsNotNull(meshInitial);

                    var translation = new[] { 1.0, 5.0 };
                    Assert.AreEqual(0, api.Mesh2dTranslate(id,
                                                           translation[0],
                                                           translation[1]));

                    // Get data of rotated mesh
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshTranslated));
                    Assert.IsNotNull(meshTranslated);

                    // Compute expected nodes
                    double[,] expectedNodes = TranslateNodes(meshInitial.NodeX, meshInitial.NodeY, translation);

                    // Expected and actual values must be equal within a tolerance
                    const double tolerance = 1.0e-6;
                    for (var i = 0; i < meshInitial.NumNodes; i++)
                    {
                        Assert.AreEqual(expectedNodes[i, 0], meshTranslated.NodeX[i], tolerance);
                        Assert.AreEqual(expectedNodes[i, 1], meshTranslated.NodeY[i], tolerance);
                    }
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearComputeTransfiniteFromPolygon(id, geometryListIn, 0, 2, 4, true));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((3, 3), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearComputeTransfiniteFromTriangle(id, geometryListIn, 0, 3, 6));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((5, 4), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    //Call
                    var xCoordinateOut = 0.0;
                    var yCoordinateOut = 0.0;
                    Assert.AreEqual(0, api.Mesh2dGetClosestNode(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref xCoordinateOut, ref yCoordinateOut));
                    //Assert
                    Assert.LessOrEqual(xCoordinateOut, 1e-6);
                    Assert.LessOrEqual(yCoordinateOut, 1e-6);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    //Execute
                    int nodeIndex = -1;
                    Assert.AreEqual(0, api.Mesh2dGetNodeIndex(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref nodeIndex));
                    //Assert
                    Assert.AreEqual(0, nodeIndex);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int numEdges = mesh.NumEdges;

                    //Call
                    Assert.AreEqual(0, api.Mesh2dDeleteEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0));
                    //Assert

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(numEdges - 1, mesh2D.NumValidEdges);
                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int numEdges = mesh.NumEdges;

                    //Call
                    int edgeIndex = -1;
                    Assert.AreEqual(0, api.Mesh2dGetEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0, ref edgeIndex));
                    //Assert
                    Assert.AreEqual(0, edgeIndex);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.AreEqual(0, api.Mesh1dSet(id, mesh1d));
                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    var projectionFactor = 0.0;
                    Assert.AreEqual(0, api.ContactsComputeSingle(id, onedNodeMaskPinnedAddress, geometryListIn, projectionFactor));
                    Assert.AreEqual(0, api.ContactsGetData(id, out contacts));
                    Assert.Greater(contacts.NumContacts, 0);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.AreEqual(0, api.Mesh1dSet(id, mesh1d));
                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.AreEqual(0, api.ContactsComputeMultiple(id, onedNodeMaskPinnedAddress));
                    Assert.AreEqual(0, api.ContactsGetData(id, out contacts));
                    Assert.Greater(contacts.NumContacts, 0);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.AreEqual(0, api.Mesh1dSet(id, mesh1d));
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.AreEqual(0, api.ContactsComputeWithPolygons(id, onedNodeMaskPinnedAddress, geometryListIn));
                    Assert.AreEqual(0, api.ContactsGetData(id, out contacts));
                    // Only one contact is generated, because only one polygon is present 
                    Assert.AreEqual(1, contacts.NumContacts);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.AreEqual(0, api.Mesh1dSet(id, mesh1d));
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.AreEqual(0, api.ContactsComputeWithPoints(id, onedNodeMaskPinnedAddress, geometryListIn));
                    Assert.AreEqual(0, api.ContactsGetData(id, out contacts));
                    // Four contacts are generated, one for each point
                    Assert.AreEqual(4, contacts.NumContacts);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    geometryListIn.XCoordinates = new[] { 1.5, 1.5, 3.5, 3.5, 1.5 };
                    geometryListIn.YCoordinates = new[] { -1.5, 1.5, 1.5, -1.5, -1.5 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    geometryListIn.GeometrySeparator = api.GetSeparator();

                    int[] selectedVertices = Array.Empty<int>();
                    Assert.AreEqual(0, api.GetSelectedVerticesInPolygon(id, geometryListIn, 1, ref selectedVertices));
                    Assert.AreEqual(4, selectedVertices.Length);
                }
                finally
                {
                    api.DeallocateState(id);
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
                Assert.AreEqual(0, api.GetAveragingMethodClosestPoint(ref method));
                // Assert
                Assert.AreEqual(2, method);
            }
        }

        [Test]
        public void GetAveragingMethodInverseDistanceWeightingThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.AreEqual(0, api.GetAveragingMethodInverseDistanceWeighting(ref method));
                // Assert
                Assert.AreEqual(5, method);
            }
        }

        [Test]
        public void GetAveragingMethodMaxThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.AreEqual(0, api.GetAveragingMethodMax(ref method));
                // Assert
                Assert.AreEqual(3, method);
            }
        }

        [Test]
        public void GetAveragingMethodMinThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.AreEqual(0, api.GetAveragingMethodMin(ref method));
                // Assert
                Assert.AreEqual(4, method);
            }
        }

        [Test]
        public void GetAveragingMethodMinAbsoluteValueThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.AreEqual(0, api.GetAveragingMethodMinAbsoluteValue(ref method));
                // Assert
                Assert.AreEqual(6, method);
            }
        }

        [Test]
        public void GetAveragingMethodSimpleAveragingThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int method = -1;
                Assert.AreEqual(0, api.GetAveragingMethodSimpleAveraging(ref method));
                // Assert
                Assert.AreEqual(1, method);
            }
        }

        [Test]
        public void GetEdgesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int locationType = -1;
                Assert.AreEqual(0, api.GetEdgesLocationType(ref locationType));
                // Assert
                Assert.AreEqual(2, locationType);
            }
        }

        [Test]
        public void GetErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var disposableMesh1D = new DisposableMesh1D();

                Assert.AreNotEqual(0, api.Mesh1dGetData(0, out disposableMesh1D));

                // Execute
                Assert.AreEqual(0, api.GetError(out string errorMessage));
                Assert.True(errorMessage.Length > 0);

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
                Assert.AreEqual(0, api.GetExitCodeSuccess(ref exitCode));
                // Assert
                Assert.AreEqual(0, exitCode);
            }
        }

        [Test]
        public void GetExitCodeMeshKernelErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeMeshKernelError(ref exitCode));
                // Assert
                Assert.AreEqual(1, exitCode);
            }
        }

        [Test]
        public void GetExitCodeNotImplementedErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeNotImplementedError(ref exitCode));
                // Assert
                Assert.AreEqual(2, exitCode);
            }
        }

        [Test]
        public void GetExitCodeAlgorithmErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeAlgorithmError(ref exitCode));
                // Assert
                Assert.AreEqual(3, exitCode);
            }
        }

        [Test]
        public void GetExitCodeConstraintErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeConstraintError(ref exitCode));
                // Assert
                Assert.AreEqual(4, exitCode);
            }
        }

        [Test]
        public void GetExitCodeMeshGeometryErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeMeshGeometryError(ref exitCode));
                // Assert
                Assert.AreEqual(5, exitCode);
            }
        }

        [Test]
        public void GetExitCodeLinearAlgebraErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeLinearAlgebraError(ref exitCode));
                // Assert
                Assert.AreEqual(6, exitCode);
            }
        }

        [Test]
        public void GetExitCodeRangeErrorThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeRangeError(ref exitCode));
                // Assert
                Assert.AreEqual(7, exitCode);
            }
        }

        [Test]
        public void GetExitCodeStdLibExceptionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeStdLibException(ref exitCode));
                // Assert
                Assert.AreEqual(8, exitCode);
            }
        }

        [Test]
        public void GetExitCodeUnknownExceptionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int exitCode = -1;
                Assert.AreEqual(0, api.GetExitCodeUnknownException(ref exitCode));
                // Assert
                Assert.AreEqual(9, exitCode);
            }
        }

        [Test]
        public void GetFacesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int locationType = -1;
                Assert.AreEqual(0, api.GetFacesLocationType(ref locationType));

                // Assert
                Assert.AreEqual(0, locationType);
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
                Assert.AreEqual(0, api.GetGeometryError(ref invalidIndex, ref type));
                // Assert
                Assert.AreEqual(0, invalidIndex);
                Assert.AreEqual(3, type);
            }
        }

        [Test]
        public void GetNodesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int type = -1;
                Assert.AreEqual(0, api.GetNodesLocationType(ref type));
                // Assert
                Assert.AreEqual(1, type);
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
                    Assert.AreEqual(0, api.GetProjection(id, ref projection));
                    // Assert
                    Assert.AreEqual(0, projection);
                }
                finally
                {
                    api.DeallocateState(id);
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
                Assert.AreEqual(0, api.GetProjectionCartesian(ref projection));
                // Assert
                Assert.AreEqual(0, projection);
            }
        }

        [Test]
        public void GetProjectionSphericalThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int projection = -1;
                Assert.AreEqual(0, api.GetProjectionSpherical(ref projection));
                // Assert
                Assert.AreEqual(1, projection);
            }
        }

        [Test]
        public void GetProjectionSphericalAccurateThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int projection = -1;
                Assert.AreEqual(0, api.GetProjectionSphericalAccurate(ref projection));
                // Assert
                Assert.AreEqual(2, projection);
            }
        }

        [Test]
        public void GetVersionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                string version;
                Assert.AreEqual(0, api.GetVersion(out version));
                // Assert
                Assert.Greater(version.Length, 0);
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

                    Assert.AreEqual(0, api.Mesh1dSet(id, inDisposableMesh1D));

                    Assert.AreEqual(0, api.Mesh1dGetData(id, out outDisposableMesh1D));
                    Assert.AreEqual(inDisposableMesh1D.NumEdges, outDisposableMesh1D.NumEdges);
                    Assert.AreEqual(inDisposableMesh1D.NumNodes, outDisposableMesh1D.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, disposableMesh2D));

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

                    Assert.AreEqual(0, api.Mesh2dAveragingInterpolation(id,
                                                                        samples,
                                                                        locationType,
                                                                        averagingMethodType,
                                                                        relativeSearchSize,
                                                                        0,
                                                                        ref results));

                    Assert.AreEqual(3.0, results.Values[4]);
                }
                finally
                {
                    api.DeallocateState(id);
                    results.Dispose();
                    samples.Dispose();
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();

                    Assert.AreEqual(0, api.Mesh2dComputeOrthogonalization(id,
                                                                          ProjectToLandBoundaryOptions.ToOriginalNetBoundary,
                                                                          orthogonalizationParametersList,
                                                                          polygon,
                                                                          landBoundaries));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(16, mesh2D.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    // Get data of initial mesh
                    meshInitial = new DisposableMesh2D();
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshInitial));
                    Assert.IsNotNull(meshInitial);

                    {
                        // Set the zone string
                        const string zone = "+proj=utm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";

                        // The mesh projection is initially cartesian, convert to spherical
                        Assert.AreEqual(0, api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone));
                        int projection = -1;
                        Assert.AreEqual(0, api.GetProjection(id, ref projection));
                        Assert.AreEqual(ProjectionOptions.Spherical, (ProjectionOptions)projection);

                        // Get data of final mesh
                        Assert.AreEqual(0, api.Mesh2dGetData(id, out meshFinal));
                        Assert.IsNotNull(meshFinal);

                        // It must be different than the initial mesh
                        Assert.AreNotEqual(meshInitial.NodeX, meshFinal.NodeX);
                        Assert.AreNotEqual(meshInitial.NodeY, meshFinal.NodeY);

                        // Round trip, convert back to cartesian
                        Assert.AreEqual(0, api.Mesh2dConvertProjection(id, ProjectionOptions.Cartesian, zone));
                        Assert.AreEqual(0, api.GetProjection(id, ref projection));
                        Assert.AreEqual(ProjectionOptions.Cartesian, (ProjectionOptions)projection);
                    }

                    // Get data of final mesh
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshFinal));
                    Assert.IsNotNull(meshFinal);

                    // Compare meshes the initial and final meshes

                    // First comparison: all array sizes must be equal
                    Assert.AreEqual(meshInitial.NumNodes, meshFinal.NumNodes);
                    Assert.AreEqual(meshInitial.NumFaces, meshFinal.NumFaces);
                    Assert.AreEqual(meshInitial.NumEdges, meshFinal.NumEdges);
                    Assert.AreEqual(meshInitial.NumFaceNodes, meshFinal.NumFaceNodes);

                    // Second comparison: all integer arrays must be equal
                    Assert.AreEqual(meshInitial.EdgeFaces, meshFinal.EdgeFaces);
                    Assert.AreEqual(meshInitial.EdgeNodes, meshFinal.EdgeNodes);
                    Assert.AreEqual(meshInitial.FaceEdges, meshFinal.FaceEdges);
                    Assert.AreEqual(meshInitial.FaceNodes, meshFinal.FaceNodes);
                    Assert.AreEqual(meshInitial.NodesPerFace, meshFinal.NodesPerFace);

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
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    // Get data of initial mesh
                    meshInitial = new DisposableMesh2D();
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshInitial));
                    Assert.IsNotNull(meshInitial);

                    {
                        int expectedExitCode = -1;
                        api.GetExitCodeStdLibException(ref expectedExitCode);

                        // Missing projection key-value pair in string
                        var zone = "+lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        int exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.AreEqual(expectedExitCode, exitCode);

                        // Missing projection value pair in string
                        zone = "+proj= +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.AreEqual(expectedExitCode, exitCode);

                        // Use invalid projection value in string
                        zone = "+proj=ratm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=31";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.AreEqual(expectedExitCode, exitCode);

                        // Use invalid zone value in string
                        zone = "+proj=utm +lat_1=0.5 +lat_2=2 +n=0.5 +zone=666";
                        exitCode = api.Mesh2dConvertProjection(id, ProjectionOptions.Spherical, zone);
                        Assert.AreEqual(expectedExitCode, exitCode);
                    }
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int[] hangingEdges = Array.Empty<int>();
                    Assert.AreEqual(0, api.Mesh2dGetHangingEdges(id, out hangingEdges));
                    Assert.AreEqual(0, hangingEdges.Length);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var smallFlowEdgesLengthThreshold = 1000.0; //Large threshold: all edges will be included
                    int numSmallFlowEdges = -1;

                    Assert.AreEqual(0, api.Mesh2dCountSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges));
                    disposableGeometryList = new DisposableGeometryList();
                    disposableGeometryList.XCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.YCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.NumberOfCoordinates = numSmallFlowEdges;

                    // Execute
                    Assert.AreEqual(0, api.Mesh2dGetSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref disposableGeometryList));
                    // Assert
                    Assert.AreEqual(12, disposableGeometryList.XCoordinates.Length);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(24, mesh2D.NumEdges);

                    Assert.AreEqual(0, api.Mesh2dDeleteHangingEdges(id));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(24, mesh2D.NumEdges); // No hanging edges found
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    var smallFlowEdgesLengthThreshold = 10.0;
                    var minFractionalAreaTriangles = 0.01;

                    // Execute
                    Assert.AreEqual(0, api.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(id, smallFlowEdgesLengthThreshold, minFractionalAreaTriangles));
                    // Assert
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(12, mesh2D.NumValidEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int numObtuseTriangles = -1;
                    Assert.AreEqual(0, api.Mesh2dCountObtuseTriangles(id, ref numObtuseTriangles));
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.NumberOfCoordinates = numObtuseTriangles;

                    Assert.AreEqual(0, api.Mesh2dGetObtuseTrianglesMassCenters(id, ref disposableGeometryList));
                    Assert.AreEqual(0, disposableGeometryList.XCoordinates.Length);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
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

                    Assert.AreEqual(0, api.Mesh2dIntersectionsFromPolygon(id,
                                                                          disposableGeometryList,
                                                                          ref edgeNodes,
                                                                          ref edgeIndex,
                                                                          ref edgeDistances,
                                                                          ref segmentDistances,
                                                                          ref segmentIndexes,
                                                                          ref faceIndexes,
                                                                          ref faceNumEdges,
                                                                          ref faceEdgeIndex));

                    Assert.AreEqual(0, segmentIndexes[0]);
                    Assert.AreEqual(0, segmentIndexes[1]);

                    Assert.AreEqual(0.25, segmentDistances[0]);
                    Assert.AreEqual(0.75, segmentDistances[1]);

                    Assert.AreEqual(6, faceIndexes[0]);
                    Assert.AreEqual(3, faceIndexes[1]);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dMakeRectangularMesh(id, makeGridParameters));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.NotNull(mesh2d);
                    Assert.AreEqual(16, mesh2d.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(algorithmErrorExitCode, api.Mesh2dMakeRectangularMeshFromPolygon(id, makeGridParameters, polygon));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dMakeRectangularMeshFromPolygon(id, makeGridParameters, polygon));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.NotNull(mesh2d);
                    Assert.AreEqual(9, mesh2d.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dMakeRectangularMeshOnExtension(id, makeGridParameters));
                    // Assert
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(121, mesh2D.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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
                var griddedSamples = new DisposableGriddedSamples<float>();
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();

                    griddedSamples.NumX = 6;
                    griddedSamples.NumY = 7;

                    double coordinate = -50.0;
                    var dx = 100.0;
                    griddedSamples.CoordinatesX = new double[griddedSamples.NumX];
                    for (var i = 0; i < griddedSamples.NumX; i++)
                    {
                        griddedSamples.CoordinatesX[i] = coordinate + (i * dx);
                    }

                    coordinate = -50.0;
                    var dy = 100.0;
                    griddedSamples.CoordinatesY = new double[griddedSamples.NumY];
                    for (var i = 0; i < griddedSamples.NumY; ++i)
                    {
                        griddedSamples.CoordinatesY[i] = coordinate + (i * dy);
                    }

                    griddedSamples.Values = new float[griddedSamples.NumY * griddedSamples.NumX];
                    for (var i = 0; i < griddedSamples.Values.Length; ++i)
                    {

                        griddedSamples.Values[i] = -0.05f;
                    }
                    // Set the interpolation type to double
                    griddedSamples.ValueType = (int)InterpolationTypes.Float;


                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    // Execute
                    Assert.AreEqual(0, api.Mesh2dRefineBasedOnGriddedSamples(id,
                                                                             griddedSamples,
                                                                             meshRefinementParameters,
                                                                             true));
                    meshOut = new DisposableMesh2D();
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshOut));
                    // Assert
                    Assert.NotNull(meshOut);
                    Assert.AreEqual(16, meshOut.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    meshOut?.Dispose();
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
                    Assert.AreEqual(0, api.GetNodesLocationType(ref locationType));
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2D));
                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new[] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    results.XCoordinates = mesh2D.NodeX;
                    results.YCoordinates = mesh2D.NodeY;
                    results.Values = new double[mesh.NumNodes];
                    results.NumberOfCoordinates = mesh.NumNodes;

                    // Execute
                    Assert.AreEqual(0, api.Mesh2dTriangulationInterpolation(id, samples, locationType, ref results));
                    // Assert
                    Assert.AreEqual(3, results.Values[4]);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    int numHangingEdges = -1;
                    Assert.AreEqual(0, api.Mesh2dCountHangingEdges(id, ref numHangingEdges));
                    Assert.AreEqual(0, numHangingEdges);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));
                    Assert.AreEqual(0, api.Mesh2dDelete(id,
                                                        in polygon,
                                                        deleteMeshInsidePolygonOptions,
                                                        invertSelection));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreNotEqual(expectedNumNodes, mesh.NumNodes);
                    Assert.AreNotEqual(expectedNunEdges, mesh.NumEdges);
                    Assert.AreNotEqual(expectedNumFaces, mesh.NumFaces);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.Mesh2dMakeGlobal(id, 19, 25));

                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(1233, mesh2d.NumEdges);
                    Assert.AreEqual(629, mesh2d.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    // Do
                    Assert.AreEqual(0, api.Mesh2dDeleteNode(id, 0));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(-999.0, mesh2d.NodeX[0]);
                    Assert.AreEqual(numberOfVerticesBefore - 1, mesh2d.NumValidNodes);
                    Assert.AreEqual(0, api.Mesh2dDeleteNode(id, 6));
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(-999.0, mesh2d.NodeX[6]);
                    Assert.AreEqual(numberOfVerticesBefore - 2, mesh2d.NumValidNodes);

                    // Un-do
                    bool undone = false;
                    Assert.AreEqual(0, api.UndoState(ref undone));
                    Assert.AreEqual(true, undone);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(100.0, mesh2d.NodeX[6]);
                    Assert.AreEqual(numberOfVerticesBefore - 1, mesh2d.NumValidNodes);

                    Assert.AreEqual(0, api.UndoState(ref undone));
                    Assert.AreEqual(true, undone);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(0.0, mesh2d.NodeX[0]);
                    Assert.AreEqual(numberOfVerticesBefore, mesh2d.NumValidNodes);

                    // Re-do
                    bool redone = false;

                    Assert.AreEqual(0, api.RedoState(ref redone));
                    Assert.AreEqual(true, redone);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(-999.0, mesh2d.NodeX[0]);
                    Assert.AreEqual(numberOfVerticesBefore - 1, mesh2d.NumValidNodes);

                    Assert.AreEqual(0, api.RedoState(ref redone));
                    Assert.AreEqual(true, redone);
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));
                    Assert.AreEqual(-999.0, mesh2d.NodeX[6]);
                    Assert.AreEqual(numberOfVerticesBefore - 2, mesh2d.NumValidNodes);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetEdgeLocationIndex(id,
                                                   10.0,
                                                   0.0,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.AreEqual(locationIndex, 0);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetFaceLocationIndex(id,
                                                   10.0,
                                                   0.0,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.AreEqual(locationIndex, 0);
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Mesh2dGetNodeLocationIndexThroughApi()
        {
            // Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.Mesh2dGetNodeLocationIndex(id,
                                                   10.0,
                                                   0.0,
                                                   boundingBox,
                                                   ref locationIndex);

                    Assert.AreEqual(locationIndex, 4);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, curvilinearGrid));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.CurvilinearGetFaceLocationIndex(id,
                                                        20.0,
                                                        20.0,
                                                        boundingBox,
                                                        ref locationIndex);

                    Assert.AreEqual(locationIndex, 4);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, curvilinearGrid));

                    BoundingBox boundingBox = BoundingBox.CreateDefault();
                    int locationIndex = -1;
                    api.CurvilinearGetNodeLocationIndex(id, x, y,
                                                        boundingBox,
                                                        ref locationIndex);

                    Assert.That(locationIndex, Is.EqualTo(expectedIndex));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.Mesh2dSet(id, mesh));


                    api.Mesh2dConvertCurvilinear(id, 5, 5);

                    // Get curvilinear grid data after conversion
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.AreEqual((10, 10), (curvilinearGrid.NumM, curvilinearGrid.NumN));

                    // Get mesh data after conversion
                    Assert.AreEqual(0, api.Mesh2dGetData(id, out meshOut));
                    Assert.AreEqual(0, meshOut.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    curvilinearGrid?.Dispose();
                    meshOut?.Dispose();
                }
            }
        }
    }

}
