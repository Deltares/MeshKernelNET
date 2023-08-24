﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Api;
using NUnit.Framework;
using static MeshKernelNETCoreTest.Api.TestUtilityFunctions;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETTests")]
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2d = new DisposableMesh2D();
                try
                {
                    var numberOfVerticesBefore = mesh.NumNodes;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dDeleteNode(id, 0));

                    
                    var success = api.Mesh2dGetData(id, out mesh2d);
                    Assert.IsTrue(success);
                    var count = mesh2d.NodeX.Length;

                    Assert.AreNotEqual(2, mesh.NumEdges);
                    Assert.AreEqual(numberOfVerticesBefore - 1, count);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var geometryListIn = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    Assert.IsTrue(api.Mesh2dFlipEdges(id, true, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, geometryListIn, landBoundaries));

                    
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 9, count);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int newEdgeIndex = 0;
                    Assert.IsTrue(api.Mesh2dInsertEdge(id, 4, 1, ref newEdgeIndex));

                    
                    var success = api.Mesh2dGetData(id, out mesh2D);

                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 1, count);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dMergeTwoNodes(id, 0, 4));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;

                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore - 1, count);
                    
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
            // Merges vertices within a distance of 0.001 m, effectively removing small edges.
            // In this case no small edges are present, so no edges shall be removed.

            // Setup
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryList = new DisposableGeometryList();
                    Assert.IsTrue(api.Mesh2dMergeNodes(id, geometryList, 0.001));

                    
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);

                    Assert.AreEqual(numberOfEdgesBefore, count);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    Assert.IsTrue(api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries));
                    
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);

                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    var count = mesh2D.NumEdges;
                    Assert.AreEqual(numberOfEdgesBefore, count);
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
                var geometrySeparator = api.GetSeparator();
                geometryListIn.GeometrySeparator = geometrySeparator;
                geometryListIn.NumberOfCoordinates = 3;
                geometryListIn.XCoordinates = new[] { 10.0, 20.0, 30.0 };
                geometryListIn.YCoordinates = new[] { -5.0, 5.0, -5.0 };
                geometryListIn.Values = new[] { 0.0, 0.0, 0.0 };

                int numberOfPointsBetweenVertices = 20;
                var geometryListOut = new DisposableGeometryList();
                geometryListOut.GeometrySeparator = geometrySeparator;
                geometryListOut.NumberOfCoordinates = 60;
                geometryListOut.XCoordinates = new double[60];
                geometryListOut.YCoordinates = new double[60];
                geometryListOut.Values = new double[60];

                Assert.IsTrue(api.GetSplines(geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices));
                geometryListOut.Dispose();
            }
        }

        [Test]
        public void Mesh2dMakeMeshFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {

                    id = api.AllocateState(0);

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 17;

                    geometryListIn.XCoordinates = new[]
                    {
                            415.319672,
                            390.271973,
                            382.330048,
                            392.715668,
                            418.374268,
                            453.807556,
                            495.960968,
                            532.005188,
                            565.605774,
                            590.653442,
                            598.595398,
                            593.708008,
                            564.994812,
                            514.899475,
                            461.138611,
                            422.039764,
                            415.319672
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            490.293762,
                            464.024139,
                            438.365448,
                            411.484894,
                            386.437103,
                            366.276703,
                            363.222107,
                            370.553162,
                            386.437103,
                            412.095825,
                            445.085571,
                            481.129944,
                            497.624817,
                            504.955872,
                            501.290344,
                            493.348358,
                            490.293762
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    Assert.IsTrue(api.Mesh2dMakeMeshFromPolygon(id, geometryListIn));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeMeshFromSamplesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 5;

                    geometryListIn.XCoordinates = new[]
                    {
                            0.0,
                            10.0,
                            10.0,
                            0.0,
                            0.0
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            0.0,
                            0.0,
                            10.0,
                            10.0,
                            0.0
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    Assert.IsTrue(api.Mesh2dMakeMeshFromSamples(id, ref geometryListIn));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int numberOfPolygonVertices = -1;
                    Assert.IsTrue(api.Mesh2dCountMeshBoundariesAsPolygons(id, ref numberOfPolygonVertices));
                    Assert.AreEqual(13, numberOfPolygonVertices);

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.Values = new double[numberOfPolygonVertices];
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = numberOfPolygonVertices;

                    Assert.IsTrue(api.Mesh2dGetMeshBoundariesAsPolygons(id, ref geometryListIn));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
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
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var disposableGeometryListOut = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);
                    
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 4;

                    geometryListIn.XCoordinates = new[]
                    {
                            0.0,
                            1.0,
                            1.0,
                            0.0
                    };

                    geometryListIn.YCoordinates = new[]
                    {
                            0.0,
                            0.0,
                            1.0,
                            1.0
                    };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0
                    };

                    double distance = 10.0;
                    int numberOfPolygonVertices = -1;
                    bool innerOffsetedPolygon = false;
                    Assert.IsTrue(api.PolygonCountOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref numberOfPolygonVertices));
                    Assert.AreEqual(4, numberOfPolygonVertices);

                    bool success = api.PolygonGetOffset(id, geometryListIn, innerOffsetedPolygon, distance, ref disposableGeometryListOut);
                    Assert.IsTrue(success);
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
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                var geometryListOut = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 3;

                    geometryListIn.XCoordinates = new[]
                    {
                            76.251099,
                            498.503723,
                            505.253784
                    };

                    geometryListIn.YCoordinates = new[]
                    {
                            92.626556,
                            91.126541,
                            490.130554
                    };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0
                    };

                    double distance = 40.0;
                    int firstIndex = 0;
                    int secondIndex = 2;
                    int numberOfPolygonVertices = -1;
                    Assert.IsTrue(api.PolygonCountRefine(id,
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

                    Assert.IsTrue(api.PolygonRefine(id, 
                                                    geometryListIn, 
                                                    firstIndex,
                                                    secondIndex, 
                                                    distance, 
                                                    ref geometryListOut));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[]
                    {
                            50.0,
                            150.0,
                            250.0,
                            50.0,
                            150.0,
                            250.0,
                            50.0,
                            150.0,
                            250.0
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            50.0,
                            50.0,
                            50.0,
                            150.0,
                            150.0,
                            150.0,
                            250.0,
                            250.0,
                            250.0
                        };

                    geometryListIn.Values = new[]
                    {
                            2.0,
                            2.0,
                            2.0,
                            3.0,
                            3.0,
                            3.0,
                            4.0,
                            4.0,
                            4.0
                        };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    double relativeSearchRadius = 1.0;
                    int minimumNumSamples = 1;
                    Assert.IsTrue(api.Mesh2dRefineBasedOnSamples(id, geometryListIn, relativeSearchRadius, minimumNumSamples, meshRefinementParameters));

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);
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
            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);


                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[]
                    {
                            250.0,
                            750.0,
                            750.0,
                            250.0,
                            250.0
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            250.0,
                            250.0,
                            750.0,
                            750.0,
                            250.0
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    //Call
                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    meshRefinementParameters.MaxNumRefinementIterations = 2;
                    meshRefinementParameters.RefineIntersected = false;
                    Assert.IsTrue(api.Mesh2dRefineBasedOnPolygon(id, geometryListIn, meshRefinementParameters));

                    //Assert
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
                    
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }


        [Test]
        public void CurvilinearComputeTransfiniteFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                try
                {
                    id = api.AllocateState(0);

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 9;

                    geometryListIn.XCoordinates = new[]
                    {
                            273.502319,
                            274.252319,
                            275.002350,
                            458.003479,
                            719.005127,
                            741.505249,
                            710.755066,
                            507.503784,
                            305.002533
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            478.880432,
                            325.128906,
                            172.127350,
                            157.127213,
                            157.127213,
                            328.128937,
                            490.880554,
                            494.630615,
                            493.130615
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    // Call
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromPolygon(id, geometryListIn, 0, 2, 4, true));

                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(3, curvilinearGrid.NumM);
                    Assert.AreEqual(3, curvilinearGrid.NumN);
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
            using( var api = new MeshKernelApi())
            using( var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                try
                {

                    id = api.AllocateState(0);

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 10;

                    geometryListIn.XCoordinates = new[]
                    {
                            444.504791,
                            427.731781,
                            405.640503,
                            381.094666,
                            451.050354,
                            528.778931,
                            593.416260,
                            558.643005,
                            526.733398,
                            444.095703
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            437.155945,
                            382.745758,
                            317.699005,
                            262.470612,
                            262.879700,
                            263.288788,
                            266.561584,
                            324.653687,
                            377.836578,
                            436.746857
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    // Call
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromTriangle(id, geometryListIn, 0, 3, 6));
                    
                    Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(4, curvilinearGrid.NumM);
                    Assert.AreEqual(5, curvilinearGrid.NumN);
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

            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    //Call
                    double xCoordinateOut = 0.0;
                    double yCoordinateOut = 0.0;
                    Assert.IsTrue(api.Mesh2dGetClosestNode(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref xCoordinateOut, ref yCoordinateOut));

                    //Assert
                    Assert.LessOrEqual(xCoordinateOut, 1e-6);
                    Assert.LessOrEqual(yCoordinateOut, 1e-6);
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
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

            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    //Execute
                    int nodeIndex = -1;
                    Assert.IsTrue(api.Mesh2dGetNodeIndex(id, -5.0, -5.0, 10.0, 0.0, 0.0, 1000.0, 1000.0, ref nodeIndex));

                    //Assert
                    Assert.AreEqual(nodeIndex, 0);
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
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

            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    var numEdges = mesh.NumEdges;

                    //Call
                    Assert.IsTrue(api.Mesh2dDeleteEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0));

                    //Assert
                    
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(mesh2D.NumEdges, numEdges - 1);
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

            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    var numEdges = mesh.NumEdges;

                    //Call
                    int edgeIndex = -1;
                    Assert.IsTrue(api.Mesh2dGetEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0, ref edgeIndex));

                    //Assert
                    Assert.AreEqual(edgeIndex, 0);
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
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
            using (var mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    mesh1d.NodeX = new[]
                    {
                        1.73493900000000,
                        2.35659313023165,
                        5.38347452702839,
                        14.2980910429074,
                        22.9324017677239,
                        25.3723169493137,
                        25.8072280000000
                    };
                    mesh1d.NodeY = new[]
                    {
                        -7.6626510000000,
                        1.67281447902331,
                        10.3513746546384,
                        12.4797224193970,
                        15.3007317677239,
                        24.1623588554512,
                        33.5111870000000
                    };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.IsTrue(api.Mesh1dSet(id, mesh1d));

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    double projectionFactor = 0.0;
                    Assert.IsTrue(api.ContactsComputeSingle(id, onedNodeMaskPinnedAddress, geometryListIn, projectionFactor));
                    
                    Assert.IsTrue(api.ContactsGetData(id, out contacts));
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
            using (var mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    mesh1d.NodeX = new[]
                    {
                        1.73493900000000,
                        2.35659313023165,
                        5.38347452702839,
                        14.2980910429074,
                        22.9324017677239,
                        25.3723169493137,
                        25.8072280000000
                    };
                    mesh1d.NodeY = new[]
                    {
                        -7.6626510000000,
                        1.67281447902331,
                        10.3513746546384,
                        12.4797224193970,
                        15.3007317677239,
                        24.1623588554512,
                        33.5111870000000
                    };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.IsTrue(api.Mesh1dSet(id, mesh1d));

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.IsTrue(api.ContactsComputeMultiple(id, onedNodeMaskPinnedAddress));
                    
                    Assert.IsTrue(api.ContactsGetData(id, out contacts));
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
            using (var mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    mesh1d.NodeX = new[]
                    {
                        1.73493900000000,
                        2.35659313023165,
                        5.38347452702839,
                        14.2980910429074,
                        22.9324017677239,
                        25.3723169493137,
                        25.8072280000000
                    };
                    mesh1d.NodeY = new[]
                    {
                        -7.6626510000000,
                        1.67281447902331,
                        10.3513746546384,
                        12.4797224193970,
                        15.3007317677239,
                        24.1623588554512,
                        33.5111870000000
                    };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.IsTrue(api.Mesh1dSet(id, mesh1d));

                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.IsTrue(api.ContactsComputeWithPolygons(id, onedNodeMaskPinnedAddress, geometryListIn));
                    Assert.IsTrue(api.ContactsGetData(id, out contacts));

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
            using(var mesh = CreateMesh2D(4, 4, 10, 10))
            using(var mesh1d = new DisposableMesh1D())
            using(var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                var contacts = new DisposableContacts();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    mesh1d.NodeX = new[]
                    {
                        1.73493900000000,
                        2.35659313023165,
                        5.38347452702839,
                        14.2980910429074,
                        22.9324017677239,
                        25.3723169493137,
                        25.8072280000000
                    };
                    mesh1d.NodeY = new[]
                    {
                        -7.6626510000000,
                        1.67281447902331,
                        10.3513746546384,
                        12.4797224193970,
                        15.3007317677239,
                        24.1623588554512,
                        33.5111870000000
                    };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.IsTrue(api.Mesh1dSet(id, mesh1d));

                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.IsTrue(api.ContactsComputeWithPoints(id, onedNodeMaskPinnedAddress, geometryListIn));
                    Assert.IsTrue(api.ContactsGetData(id, out contacts));

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
            using (var mesh = CreateMesh2D(4, 4, 1, 1))
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    geometryListIn.XCoordinates = new[] { 1.5, 1.5, 3.5, 3.5, 1.5 };
                    geometryListIn.YCoordinates = new[] { -1.5, 1.5, 1.5, -1.5, -1.5 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    geometryListIn.GeometrySeparator = api.GetSeparator();

                    var selectedVertices = Array.Empty<int>(); 
                    Assert.IsTrue(api.GetSelectedVerticesInPolygon(id, geometryListIn, 1, ref selectedVertices));
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
                Assert.IsTrue(api.GetAveragingMethodClosestPoint(ref method));

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
                Assert.IsTrue(api.GetAveragingMethodInverseDistanceWeighting(ref method));

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
                Assert.IsTrue(api.GetAveragingMethodMax(ref method));

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
                Assert.IsTrue(api.GetAveragingMethodMin(ref method));

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
                Assert.IsTrue(api.GetAveragingMethodMinAbsoluteValue(ref method));

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
                Assert.IsTrue(api.GetAveragingMethodSimpleAveraging(ref method));

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
                Assert.IsTrue(api.GetEdgesLocationType(ref locationType));

                // Assert
                Assert.AreEqual(2, locationType);
            }
        }

        [Test]
        public void GetError()
        {
            using (var api = new MeshKernelApi())
            {
                var disposableMesh1D = new DisposableMesh1D();

                bool success  = api.Mesh1dGetData(0, out disposableMesh1D);

                Assert.False(success);

                // Execute
                string errorMessage;
                Assert.True(api.GetError(out errorMessage));
                Assert.True(errorMessage.Length > 0);

                disposableMesh1D.Dispose();
            }
        }

        [Test]
        public void GetFacesLocationTypeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                int locationType = -1;
                Assert.IsTrue(api.GetFacesLocationType(ref locationType));

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
                Assert.IsTrue(api.GetGeometryError(ref invalidIndex, ref type));

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
                Assert.IsTrue(api.GetNodesLocationType(ref type));

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
                    Assert.IsTrue(api.GetProjection(id, ref projection));

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
                Assert.IsTrue(api.GetProjectionCartesian(ref projection));

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
                Assert.IsTrue(api.GetProjectionSpherical(ref projection));

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
                Assert.IsTrue(api.GetProjectionSphericalAccurate(ref projection));

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
                Assert.IsTrue(api.GetVersion(out version));

                // Assert
                Assert.AreEqual(7, version.Length);
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

                    Assert.True(api.Mesh1dSet(id, inDisposableMesh1D));

                    Assert.IsTrue(api.Mesh1dGetData(id, out  outDisposableMesh1D));

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
            using (var disposableMesh2D = CreateMesh2D(2, 3, 1, 1))
            {
                var id = 1;
                var results = new DisposableGeometryList();
                var samples = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.True(api.Mesh2dSet(id, disposableMesh2D));

                    results.XCoordinates = new double [disposableMesh2D.NumNodes];
                    results.YCoordinates = new double[disposableMesh2D.NumNodes];
                    results.Values = new double[disposableMesh2D.NumNodes];
                    results.NumberOfCoordinates = disposableMesh2D.NumNodes;

                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new [] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    int locationType = 1;
                    int averagingMethodType = 1;
                    double relativeSearchSize = 1.01;
                    int minNumSamples = 0;

                    Assert.True(api.Mesh2dAveragingInterpolation(id, 
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
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    
                    Assert.IsTrue(api.Mesh2dComputeOrthogonalization(id,
                                                                     ProjectToLandBoundaryOptions.ToOriginalNetBoundary,
                                                                     orthogonalizationParametersList,
                                                                     polygon,
                                                                     landBoundaries));
                    
                    Assert.True(api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(16, mesh2D.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dGetHangingEdgesThroughApi()
        {
            // Setup
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var hangingEdges = Array.Empty<int>();
                    Assert.IsTrue(api.Mesh2dGetHangingEdges(id, out hangingEdges));

                    Assert.AreEqual(0, hangingEdges.Length);
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void Mesh2dGetSmallFlowEdgeCentersThroughApi()
        {
            // Setup
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var disposableGeometryList = new DisposableGeometryList();
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int numEdges = -1;
                    double smallFlowEdgesLengthThreshold = 1000.0; //Large threshold: all edges will be included
                    int numSmallFlowEdges = -1;

                    Assert.IsTrue(api.Mesh2dCountSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges));

                    disposableGeometryList.XCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.YCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.NumberOfCoordinates = numSmallFlowEdges;

                    // Execute
                    Assert.IsTrue(api.Mesh2dGetSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref disposableGeometryList));

                    // Assert
                    Assert.AreEqual(12, disposableGeometryList.XCoordinates.Length);
                }
                finally
                {
                    api.DeallocateState(id);
                    disposableGeometryList.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dDeleteHangingEdgesThroughApi()
        {
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(24, mesh2D.NumEdges);

                    Assert.IsTrue(api.Mesh2dDeleteHangingEdges(id));

                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(24, mesh2D.NumEdges); // No hanging edges found
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dDeleteSmallFlowEdgesAndSmallTrianglesThroughApi()
        {
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    double smallFlowEdgesLengthThreshold = 10.0;
                    double minFractionalAreaTriangles = 0.01;

                    // Execute
                    Assert.IsTrue(api.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(id, smallFlowEdgesLengthThreshold, minFractionalAreaTriangles));
                    
                    // Assert
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(12, mesh2D.NumEdges);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dGetObtuseTrianglesMassCentersThroughApi()
        {
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var disposableGeometryList = new DisposableGeometryList();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int numObtuseTriangles = -1;
                    Assert.IsTrue(api.Mesh2dCountObtuseTriangles(id, ref numObtuseTriangles));

                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.NumberOfCoordinates = numObtuseTriangles;

                    Assert.IsTrue(api.Mesh2dGetObtuseTrianglesMassCenters(id, ref disposableGeometryList));

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
        public void Mesh2dIntersectionsFromPolylineThroughApi()
        {
            using (var mesh = CreateMesh2D(4, 4, 1.0, 1.0))
            using (var api = new MeshKernelApi())
            using (var disposableGeometryList = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dGetData(id,out mesh2D));

                    disposableGeometryList.XCoordinates = new[] { 0.6, 0.6 };
                    disposableGeometryList.XCoordinates = new[] { 2.5, 0.5 };
                    disposableGeometryList.NumberOfCoordinates = 2;

                    var edgeNodes = Array.Empty<int>();
                    var edgeIndex = Array.Empty<int>();
                    var edgeDistances = Array.Empty<double>();
                    var segmentDistances = Array.Empty<double>();
                    var segmentIndexes = Array.Empty<int>();
                    var faceIndexes = Array.Empty<int>();
                    var faceNumEdges = Array.Empty<int>();
                    var faceEdgeIndex = Array.Empty<int>();

                    Assert.IsTrue(api.Mesh2dIntersectionsFromPolyline(id,
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
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeUniformThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using(var disposableGeometryList = new DisposableGeometryList())
            {
                var id = 0;
                var mesh2d = new DisposableMesh2D();
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

                    Assert.IsTrue(api.Mesh2dMakeUniform(id, makeGridParameters, disposableGeometryList));

                    
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2d));

                    Assert.NotNull(mesh2d);
                    Assert.AreEqual(16, mesh2d.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2d.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dMakeUniformOnExtensionThroughAPI()
        {
            
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var mesh2D = new DisposableMesh2D();
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
                    Assert.IsTrue(api.Mesh2dMakeUniformOnExtension(id, makeGridParameters));

                    // Assert
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(121, mesh2D.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dRefineBasedOnGriddedSamplesThroughAPI()
        {
            
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(4, 4, 100, 200))
            {
                var id = 0;
                var meshOut = new DisposableMesh2D();
                var griddedSamples = new DisposableGriddedSamples();
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    
                    griddedSamples.NumX = 6;
                    griddedSamples.NumY = 7;

                    double coordinate = -50.0;
                    double dx = 100.0;
                    griddedSamples.CoordinatesX = new double [griddedSamples.NumX];
                    for (var i = 0; i < griddedSamples.NumX; i++)
                    {
                        griddedSamples.CoordinatesX[i] = coordinate + i * dx;
                    }

                    coordinate = -50.0;
                    double dy = 100.0;
                    griddedSamples.CoordinatesY = new double[griddedSamples.NumY];
                    for (var i = 0; i < griddedSamples.NumY; ++i)
                    {
                        griddedSamples.CoordinatesY[i] = coordinate + i * dy;
                    }

                    griddedSamples.Values = new double[griddedSamples.NumY * griddedSamples.NumX];
                    for (var i = 0; i < griddedSamples.Values.Length; ++i)
                    {
                        griddedSamples.Values[i] = -0.05;
                    }
                    
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    // Execute
                    Assert.IsTrue(api.Mesh2dRefineBasedOnGriddedSamples(id, 
                                                                        griddedSamples, 
                                                                        meshRefinementParameters, 
                                                                        true));
                    Assert.IsTrue(api.Mesh2dGetData(id, out meshOut));

                    // Assert
                    Assert.NotNull(meshOut);
                    Assert.AreEqual(16, meshOut.NumNodes);
                }
                finally
                {
                    api.DeallocateState(id);
                    meshOut.Dispose();
                    griddedSamples.Dispose();
                }
            }
        }


        [Test]
        public void Mesh2dTriangulationInterpolationThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(3, 3, 1, 1))
            using (var samples = new DisposableGeometryList())
            {
                var id = 0;
                var results = new DisposableGeometryList();
                var mesh2D = new DisposableMesh2D();
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    int locationType = -1;
                    Assert.IsTrue(api.GetNodesLocationType(ref locationType));
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new[] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    results.XCoordinates = mesh2D.NodeX;
                    results.YCoordinates = mesh2D.NodeY;
                    results.Values = new double [mesh.NumNodes];
                    results.NumberOfCoordinates = mesh.NumNodes;

                    // Execute
                    Assert.IsTrue(api.Mesh2dTriangulationInterpolation(id, samples, locationType, ref results));

                    // Assert
                    Assert.AreEqual(3, results.Values[4]);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh2D.Dispose();
                    results.Dispose();
                }
            }
        }

        [Test]
        public void Mesh2dCountHangingEdgesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(3, 3, 1, 1))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
        
                    int numHangingEdges = -1;
                    Assert.IsTrue(api.Mesh2dCountHangingEdges(id, ref numHangingEdges));
        
                    Assert.AreEqual(0, numHangingEdges);
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

    }

}
