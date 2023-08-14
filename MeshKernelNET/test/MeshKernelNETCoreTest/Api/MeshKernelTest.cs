using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using GeoAPI.CoordinateSystems;
using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETTests")]
    public class MeshKernelTest
    {
        public static DisposableMesh2D CreateMesh2D(
            int numbOfCellsHorizontal,
            int numbOfCellsVertical,
            double cellWidth,
            double cellHeight,
            double xOffset = 0.0,
            double yOffset = 0.0)
        {
            var result = new DisposableMesh2D();

            int[,] indicesValues = new int[numbOfCellsHorizontal, numbOfCellsVertical];
            result.NodeX = new double[numbOfCellsHorizontal * numbOfCellsVertical];
            result.NodeY = new double[numbOfCellsHorizontal * numbOfCellsVertical];
            result.NumNodes = numbOfCellsHorizontal * numbOfCellsVertical;

            int nodeIndex = 0;
            for (int i = 0; i < numbOfCellsHorizontal; ++i)
            {
                for (int j = 0; j < numbOfCellsVertical; ++j)
                {
                    indicesValues[i, j] = i * numbOfCellsVertical + j;
                    result.NodeX[nodeIndex] = xOffset + i * cellWidth;
                    result.NodeY[nodeIndex] = yOffset + j * cellHeight;
                    nodeIndex++;
                }
            }

            result.NumEdges = (numbOfCellsHorizontal - 1) * numbOfCellsVertical + numbOfCellsHorizontal * (numbOfCellsVertical - 1);
            result.EdgeNodes = new int[result.NumEdges * 2];
            int edgeIndex = 0;
            for (int i = 0; i < numbOfCellsHorizontal - 1; ++i)
            {
                for (int j = 0; j < numbOfCellsVertical; ++j)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[i + 1, j];
                    edgeIndex++;
                }
            }

            for (int i = 0; i < numbOfCellsHorizontal; ++i)
            {
                for (int j = 0; j < numbOfCellsVertical - 1; ++j)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j + 1];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j];
                    edgeIndex++;
                }
            }

            return result;
        }

        public static DisposableCurvilinearGrid CreateCurvilinearGrid(
            int numbOfRows = 3,
            int numbOfColumns = 3,
            double cellWidth = 1.0,
            double cellHeight = 1.0)
        {
            var result = new DisposableCurvilinearGrid(numbOfRows, numbOfColumns);

            int nodeIndex = 0;
            for (int i = 0; i < result.NumM; ++i)
            {
                for (int j = 0; j < result.NumN; ++j)
                {
                    result.NodeX[nodeIndex] =  i * cellWidth;
                    result.NodeY[nodeIndex] =  j * cellHeight;
                    nodeIndex++;
                }
            }

            return result;
        }

        private static void GetTiming(Stopwatch stopwatch, string actionName, Action action)
        {
            stopwatch.Restart();

            action();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed} -- {actionName}");
        }

        [Test]
        [Category("PerformanceTests")]
        public void Mesh2dDeleteNodeThroughApiPerformanceTrace()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(100, 100, 100, 200))
            {
                var stopWatch = new Stopwatch();
                var numberOfVerticesBefore = mesh.NumNodes;
                var id = 0;
                GetTiming(stopWatch, "Create grid state", () =>
                {
                    id = api.AllocateState(0);

                });

                GetTiming(stopWatch, "Set state", () => { Assert.IsTrue(api.Mesh2dSet(id, mesh)); });

                GetTiming(stopWatch, "Delete node", () => { Assert.IsTrue(api.Mesh2dDeleteNode(id, 0)); });

                GetTiming(stopWatch, "Get mesh state", () =>
                {
                    var mesh2d = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2d);
                    Assert.True(success);

                    var count = mesh2d.NodeX.Length;
                    Assert.AreEqual(numberOfVerticesBefore - 1, count);

                    mesh2d.Dispose();
                });

                api.DeallocateState(id);
            }
        }

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
                try
                {
                    var numberOfVerticesBefore = mesh.NumNodes;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dDeleteNode(id, 0));

                    var mesh2d = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2d);
                    Assert.IsTrue(success);

                    var count = mesh2d.NodeX.Length;

                    Assert.AreNotEqual(2, mesh.NumEdges);
                    Assert.AreEqual(numberOfVerticesBefore - 1, count);

                    mesh2d.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    Assert.IsTrue(api.Mesh2dFlipEdges(id, true, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, geometryListIn, landBoundaries));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 9, count);

                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int newEdgeIndex = 0;
                    Assert.IsTrue(api.Mesh2dInsertEdge(id, 4, 1, ref newEdgeIndex));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);

                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);

                    Assert.AreEqual(numberOfEdgesBefore + 1, count);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dMergeTwoNodes(id, 0, 4));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    var count = mesh2D.NumEdges;

                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore - 1, count);

                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryList = new DisposableGeometryList();
                    Assert.IsTrue(api.Mesh2dMergeNodes(id, geometryList, 0.001));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);

                    var count = mesh2D.NumEdges;
                    Assert.AreNotEqual(2, mesh2D.NumEdges);

                    Assert.AreEqual(numberOfEdgesBefore, count);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    Assert.IsTrue(api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries));

                    var mesh2D = new DisposableMesh2D();

                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);


                    Assert.AreNotEqual(2, mesh2D.NumEdges);
                    var count = mesh2D.NumEdges;
                    Assert.AreEqual(numberOfEdgesBefore, count);

                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void CurvilinearMakeUniformThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();
                    var disposableGeometryList = new DisposableGeometryList();

                    makeGridParameters.GridType = 0;
                    makeGridParameters.NumberOfColumns = 3;
                    makeGridParameters.NumberOfRows = 3;
                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = 10.0;
                    makeGridParameters.YGridBlockSize = 10.0;
                    makeGridParameters.UpperRightCornerXCoordinate = 0.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 0.0;

                    Assert.IsTrue(api.CurvilinearMakeUniform(id, ref makeGridParameters, ref disposableGeometryList));

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    Assert.AreEqual(4, curvilinearGrid.NumM);

                    curvilinearGrid.Dispose();
                    disposableGeometryList.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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

                var geometryListOut = new DisposableGeometryList();
                int numberOfPointsBetweenVertices = 20;
                geometryListOut.GeometrySeparator = geometrySeparator;
                geometryListOut.NumberOfCoordinates = 60;
                geometryListOut.XCoordinates = new double[60];
                geometryListOut.YCoordinates = new double[60];
                geometryListOut.Values = new double[60];

                Assert.IsTrue(api.GetSplines(geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices));
            }
        }


        [Test]
        public void CurvilinearComputeTransfiniteFromSplinesThroughAPI()
        {
            // Setup
            using (var mesh = CreateMesh2D(0, 0, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryListIn = new DisposableGeometryList();
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[]
                    {
                            1.340015E+02, 3.642529E+02, 6.927549E+02, geometrySeparator,
                            2.585022E+02, 4.550035E+02, 8.337558E+02, geometrySeparator,
                            1.002513E+02, 4.610035E+02, geometrySeparator,
                            6.522547E+02, 7.197551E+02
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            2.546282E+02, 4.586302E+02, 5.441311E+02, geometrySeparator,
                            6.862631E+01, 2.726284E+02, 3.753794E+02, geometrySeparator,
                            4.068797E+02, 7.912642E+01, geometrySeparator,
                            6.026317E+02, 2.681283E+02
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0, 0.0, 0.0, geometrySeparator,
                            0.0, 0.0, 0.0, geometrySeparator,
                            0.0, 0.0, geometrySeparator,
                            0.0, 0.0
                        };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    var curvilinearParameters = new CurvilinearParameters();
                    curvilinearParameters.MRefinement = 10;
                    curvilinearParameters.NRefinement = 10;
                    curvilinearParameters.SmoothingIterations = 10;
                    curvilinearParameters.SmoothingParameter = 0.5;
                    curvilinearParameters.AttractionParameter = 0.0;
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromSplines(id, ref geometryListIn, ref curvilinearParameters));

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);
                    curvilinearGrid.Dispose();
                    geometryListIn.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void CurvilinearComputeOrthogonalGridFromSplinesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 6;

                    geometryListIn.XCoordinates = new[]
                    {
                            1.175014E+02, 3.755030E+02, 7.730054E+02, geometrySeparator,
                            4.100089E+01, 3.410027E+02
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            2.437587E+01, 3.266289E+02, 4.563802E+02, geometrySeparator,
                            2.388780E+02, 2.137584E+01
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0, 0.0, 0.0, geometrySeparator,
                            0.0, 0.0, geometrySeparator
                        };

                    var curvilinearParameters = new CurvilinearParameters();
                    curvilinearParameters.MRefinement = 40;
                    curvilinearParameters.NRefinement = 10;
                    var splinesToCurvilinearParameters = SplinesToCurvilinearParameters.CreateDefault();
                    splinesToCurvilinearParameters.GrowGridOutside = false;


                    Assert.IsTrue(api.CurvilinearComputeOrthogonalGridFromSplines(id, ref geometryListIn,
                        ref curvilinearParameters, ref splinesToCurvilinearParameters));

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    curvilinearGrid.Dispose();
                    geometryListIn.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Mesh2dMakeMeshFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
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

                    Assert.IsTrue(api.Mesh2dMakeMeshFromPolygon(id, ref geometryListIn));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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

                    var mesh2D = new DisposableMesh2D();
                    
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);

                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);

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

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void PolygonOffsetThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.PolygonCountOffset(id, ref geometryListIn, innerOffsetedPolygon,
                        distance, ref numberOfPolygonVertices));
                    Assert.AreEqual(4, numberOfPolygonVertices);

                    var disposableGeometryListOut = new DisposableGeometryList();
                    bool success = api.PolygonGetOffset(id, ref geometryListIn, innerOffsetedPolygon, distance,
                        ref disposableGeometryListOut);
                    Assert.IsTrue(success);

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void PolygonRefineThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.PolygonCountRefine(id, ref geometryListIn, firstIndex,
                        secondIndex, distance, ref numberOfPolygonVertices));

                    var geometryListOut = new DisposableGeometryList();
                    geometryListOut.GeometrySeparator = geometrySeparator;
                    geometryListOut.NumberOfCoordinates = numberOfPolygonVertices;

                    geometryListOut.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListOut.Values = new double[numberOfPolygonVertices];

                    Assert.IsTrue(api.PolygonRefine(id, ref geometryListIn, firstIndex,
                        secondIndex, distance, ref geometryListOut));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);
                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void Mesh2dRefineBasedOnSamplesThroughAPI()
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

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.Mesh2dRefineBasedOnSamples(id, ref geometryListIn, relativeSearchRadius, minimumNumSamples, meshRefinementParameters));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.True(success);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void Mesh2dRefineBasedOnPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);


                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.Mesh2dRefineBasedOnPolygon(id, ref geometryListIn, meshRefinementParameters));

                    //Assert
                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void CurvilinearComputeTransfiniteFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromPolygon(id, ref geometryListIn, 0, 2, 4, true));

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 3);
                    Assert.AreEqual(curvilinearGrid.NumN, 3);

                    curvilinearGrid.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void CurvilinearComputeTransfiniteFromTriangleThroughAPI()
        {
            //Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
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
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromTriangle(id, ref geometryListIn, 0, 3, 6));

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 4);
                    Assert.AreEqual(curvilinearGrid.NumN, 5);

                    curvilinearGrid.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }


        [Test]
        public void Mesh2dGetClosestNodeThroughAPI()
        {
            //Setup

            using (var mesh = CreateMesh2D(11, 11, 100, 100))
            using (var geometryListIn = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
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
                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);

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
                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);

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
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));
                    var numEdges = mesh.NumEdges;

                    //Call
                    Assert.IsTrue(api.Mesh2dDeleteEdge(id, 50.0, 0.0, 0.0, 0.0, 100.0, 100.0));

                    //Assert
                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
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
                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);
                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);

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
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
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
                    var geometryListIn = new DisposableGeometryList();
                    double projectionFactor = 0.0;
                    Assert.IsTrue(api.ContactsComputeSingle(id, ref onedNodeMaskPinnedAddress, ref geometryListIn, projectionFactor));

                    var contacts = api.ContactsGetData(id);
                    Assert.Greater(contacts.NumContacts, 0);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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
                    Assert.IsTrue(api.ContactsComputeMultiple(id, ref onedNodeMaskPinnedAddress));

                    var contacts = api.ContactsGetData(id);
                    Assert.Greater(contacts.NumContacts, 0);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
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
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
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

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.IsTrue(api.ContactsComputeWithPolygons(id, ref onedNodeMaskPinnedAddress, ref geometryListIn));
                    var contacts = api.ContactsGetData(id);

                    // Only one contact is generated, because only one polygon is present 
                    Assert.AreEqual(contacts.NumContacts, 1);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void ContactsComputeWithPointsThroughAPI()
        {
            //Setup
            using (var mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                var onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
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

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.IsTrue(api.ContactsComputeWithPoints(id, ref onedNodeMaskPinnedAddress, ref geometryListIn));
                    var contacts = api.ContactsGetData(id);

                    // Four contacts are generated, one for each point
                    Assert.AreEqual(contacts.NumContacts, 4);
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void GetSelectedVerticesInPolygonThroughAPI()
        {
            using (var mesh = CreateMesh2D(4, 4, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryListIn = new DisposableGeometryList();

                    geometryListIn.XCoordinates = new[] { 1.5, 1.5, 3.5, 3.5, 1.5 };
                    geometryListIn.YCoordinates = new[] { -1.5, 1.5, 1.5, -1.5, -1.5 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    geometryListIn.GeometrySeparator = api.GetSeparator();

                    var selectedVertices = Array.Empty<int>(); 
                    Assert.IsTrue(api.GetSelectedVerticesInPolygon(id, ref geometryListIn, 1, ref selectedVertices));
                    Assert.AreEqual(selectedVertices.Length, 4);
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void CurvilinearDeleteNodeThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.IsTrue(api.CurvilinearDeleteNode(id, 5.0, 5.0));


                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);
                    Assert.AreEqual(curvilinearGrid.NodeX[24], -999.0);
                    Assert.AreEqual(curvilinearGrid.NodeX[24], -999.0);

                    curvilinearGrid.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void CurvilinearComputeOrthogonalGridFromSplinesIterativeThroughAPI()
        {
            
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var geometryListIn = new DisposableGeometryList();
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 6;

                    geometryListIn.XCoordinates = new[]
                    {
                            1.175014E+02, 3.755030E+02, 7.730054E+02, geometrySeparator,
                            4.100089E+01, 3.410027E+02
                        };

                    geometryListIn.YCoordinates = new[]
                    {
                            2.437587E+01, 3.266289E+02, 4.563802E+02, geometrySeparator,
                            2.388780E+02, 2.137584E+01
                        };

                    geometryListIn.Values = new[]
                    {
                            0.0, 0.0, 0.0, geometrySeparator,
                            0.0, 0.0, geometrySeparator
                        };

                    var curvilinearParameters = new CurvilinearParameters
                    {
                        MRefinement = 40,
                        NRefinement = 10
                    };

                    var splinesToCurvilinearParameters = SplinesToCurvilinearParameters.CreateDefault();
                    splinesToCurvilinearParameters.GrowGridOutside = false;

                    // Execute
                    api.CurvilinearInitializeOrthogonalGridFromSplines(id,
                        geometryListIn,
                        ref curvilinearParameters, 
                        ref splinesToCurvilinearParameters);

                    int numLayers = 3;
                    for (int i = 1; i < numLayers; i++)
                    {
                        Assert.IsTrue(api.CurvilinearIterateOrthogonalGridFromSplines(id, i));
                    }

                    Assert.IsTrue(api.CurvilinearDeleteOrthogonalGridFromSplines(id));
                    
                    Assert.IsTrue(api.CurvilinearComputeOrthogonalGridFromSplines(id,
                                                                                  ref geometryListIn,
                                                                                  ref curvilinearParameters, 
                                                                                  ref splinesToCurvilinearParameters));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);
                    Assert.AreEqual(curvilinearGrid.NumM, 3);
                    Assert.AreEqual(curvilinearGrid.NumN, 7);

                    curvilinearGrid.Dispose();
                    geometryListIn.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void CurvilinearDerefineThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.IsTrue(api.CurvilinearDerefine(id, 10.0, 20.0, 30.0, 20.0));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 4);
                    Assert.AreEqual(curvilinearGrid.NumN, 5);

                    curvilinearGrid.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void CurvilinearLineShiftThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.IsTrue(api.CurvilinearInitializeLineShift(id));
                    Assert.IsTrue(api.CurvilinearSetLineLineShift(id, 0.0, 0.0, 0.0, 30.0));
                    Assert.IsTrue(api.CurvilinearSetBlockLineShift(id, 0.0, 0.0, 30.0, 30.0));
                    Assert.IsTrue(api.CurvilinearMoveNodeLineShift(id, 0.0, 0.0, -10.0, 0.0));
                    Assert.IsTrue(api.CurvilinearFinalizeLineShift(id));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 5);
                    Assert.AreEqual(curvilinearGrid.NumN, 5);

                    curvilinearGrid.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void CurvilinearOrthogonalizeThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    var orthogonalizationParameters = new OrthogonalizationParameters();
                    Assert.IsTrue(api.CurvilinearInitializeOrthogonalize(id, ref orthogonalizationParameters));
                    Assert.IsTrue(api.CurvilinearSetFrozenLinesOrthogonalize(id, 20.0, 0.0, 20.0, 10.0) );
                    Assert.IsTrue(api.CurvilinearFinalizeOrthogonalize(id));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var gridOut = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.NotNull(gridOut);

                    curvilinearGrid.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

       
        [Test]
        public void CurvilinearSmoothingThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.IsTrue(api.CurvilinearSmoothing(id, 10, 10.0, 20.0, 30.0, 20.0));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);

                    curvilinearGrid.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void CurvilinearSmoothingDirectionalThroughAPI()
        {
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.IsTrue(api.CurvilinearSmoothingDirectional(id, 
                                                                      10, 
                                                                      10.0, 
                                                                      0.0, 
                                                                      10.0, 
                                                                      30.0, 
                                                                      10.0, 
                                                                      0.0,  
                                                                      30.0, 
                                                                      0.0));

                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.IsTrue(success);
                    Assert.AreEqual(curvilinearGrid.NumM, 5);

                    curvilinearGrid.Dispose();
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
                Assert.AreEqual(method, 2);   
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
                Assert.AreEqual(method, 5);
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
                Assert.AreEqual(method, 3);
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
                Assert.AreEqual(method, 4);
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
                Assert.AreEqual(method, 6);
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
                Assert.AreEqual(method, 1);
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
                Assert.AreEqual(locationType, 2);
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
                string errorMessage="";
                
                // Execute
                Assert.True(api.GetError(ref errorMessage));
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
                Assert.AreEqual(locationType, 0);
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
                Assert.AreEqual(invalidIndex, 0);
                Assert.AreEqual(type, 3);
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
                Assert.AreEqual(type, 1);
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
                    Assert.AreEqual(projection, 0);
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
                Assert.AreEqual(projection, 0);
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
                Assert.AreEqual(projection, 1);
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
                Assert.AreEqual(projection, 2);
            }
        }

        [Test]
        public void GetVersionThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                // Execute
                string version = "";
                Assert.IsTrue(api.GetVersion(ref version));

                // Assert
                Assert.AreEqual(version.Length, 7);
            }
        }


        [Test]
        public void Mesh1dGetDataThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var id = 1;
                try
                {
                    id = api.AllocateState(0);

                    var inDisposableMesh1D = new DisposableMesh1D(2, 1);

                    inDisposableMesh1D.NodeX = new[] { 0.0, 1.0 };
                    inDisposableMesh1D.NodeY = new[] { 0.0, 1.0 };
                    inDisposableMesh1D.EdgeNodes = new[] { 0, 1 };

                    Assert.True(api.Mesh1dSet(id, inDisposableMesh1D));

                    bool success = api.Mesh1dGetData(id, out var outDisposableMesh1D);
                    Assert.IsTrue(success);
                    
                    Assert.AreEqual(inDisposableMesh1D.NumEdges, outDisposableMesh1D.NumEdges);
                    Assert.AreEqual(inDisposableMesh1D.NumNodes, outDisposableMesh1D.NumNodes);

                    inDisposableMesh1D.Dispose();
                    outDisposableMesh1D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    id = api.AllocateState(0);

                    Assert.True(api.Mesh2dSet(id, disposableMesh2D));

                    var results = new DisposableGeometryList();
                    results.XCoordinates = new double [disposableMesh2D.NumNodes];
                    results.YCoordinates = new double[disposableMesh2D.NumNodes];
                    results.Values = new double[disposableMesh2D.NumNodes];
                    results.NumberOfCoordinates = disposableMesh2D.NumNodes;

                    var samples = new DisposableGeometryList();
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

                    Assert.AreEqual(results.Values[4], 3.0);
                    results.Dispose();
                    samples.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Mesh2dComputeOrthogonalizationThroughApi()
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

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();

                    var polygon = new DisposableGeometryList();
                    var landBoundaries = new DisposableGeometryList(); 
                    
                    Assert.IsTrue(api.Mesh2dComputeOrthogonalization(id, 
                                                                     ProjectToLandBoundaryOptions.ToOriginalNetBoundary, 
                                                                     orthogonalizationParametersList, 
                                                                     ref polygon, 
                                                                     ref landBoundaries));

                    var mesh2D = new DisposableMesh2D();
                    Assert.True(api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(mesh2D.NumNodes, 16);

                    mesh2D.Dispose();
                    polygon.Dispose();
                    landBoundaries.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.IsTrue(api.Mesh2dGetHangingEdges(id, ref hangingEdges));

                    Assert.AreEqual(hangingEdges.Length, 0);
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
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int numEdges = -1;
                    double smallFlowEdgesLengthThreshold = 1000.0; //Large threshold: all edges will be included
                    int numSmallFlowEdges = -1;

                    Assert.IsTrue(api.Mesh2dCountSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges));

                    var disposableGeometryList = new DisposableGeometryList();
                    disposableGeometryList.XCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.YCoordinates = new double[numSmallFlowEdges];
                    disposableGeometryList.NumberOfCoordinates = numSmallFlowEdges;

                    // Execute
                    Assert.IsTrue(api.Mesh2dGetSmallFlowEdgeCenters(id, smallFlowEdgesLengthThreshold, ref disposableGeometryList));

                    // Assert
                    Assert.AreEqual(disposableGeometryList.XCoordinates.Length, 12);
                    disposableGeometryList.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dDeleteHangingEdges(id));

                    var mesh2D = new DisposableMesh2D();
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(mesh2D.NumEdges, 24); // No hanging edges found
                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    double smallFlowEdgesLengthThreshold = 10.0;
                    double minFractionalAreaTriangles = 0.01;

                    // Execute
                    Assert.IsTrue(api.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(id, smallFlowEdgesLengthThreshold, minFractionalAreaTriangles));
                    var mesh2D = new DisposableMesh2D();

                    // Assert
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));
                    Assert.AreEqual(mesh2D.NumEdges, 12);

                    mesh2D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    int numObtuseTriangles = -1;
                    Assert.IsTrue(api.Mesh2dCountObtuseTriangles(id, ref numObtuseTriangles));

                    var disposableGeometryList = new DisposableGeometryList();
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.XCoordinates = new double[numObtuseTriangles];
                    disposableGeometryList.NumberOfCoordinates = numObtuseTriangles;

                    Assert.IsTrue(api.Mesh2dGetObtuseTrianglesMassCenters(id, ref disposableGeometryList));

                    Assert.AreEqual(disposableGeometryList.XCoordinates.Length, 0);
                    disposableGeometryList.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Mesh2dIntersectionsFromPolylineThroughApi()
        {
            using (var mesh = CreateMesh2D(4, 4, 1.0, 1.0))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id,out mesh2D);
                    Assert.IsTrue(success);

                    var disposableGeometryList = new DisposableGeometryList();
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
                                                                      ref disposableGeometryList,
                                                                      ref edgeNodes,
                                                                      ref edgeIndex,
                                                                      ref edgeDistances,
                                                                      ref segmentDistances,
                                                                      ref segmentIndexes,
                                                                      ref faceIndexes,
                                                                      ref faceNumEdges,
                                                                      ref faceEdgeIndex));

                    Assert.AreEqual(segmentIndexes[0], 0);
                    Assert.AreEqual(segmentIndexes[1], 0);

                    Assert.AreEqual(segmentDistances[0], 0.25);
                    Assert.AreEqual(segmentDistances[1], 0.75);

                    Assert.AreEqual(faceIndexes[0], 6);
                    Assert.AreEqual(faceIndexes[1], 3);

                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void Mesh2dMakeUniformThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
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

                    var disposableGeometryList = new DisposableGeometryList();
                    Assert.IsTrue(api.Mesh2dMakeUniform(id, ref makeGridParameters, ref disposableGeometryList));

                    var mesh2d = new DisposableMesh2D();
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2d));

                    Assert.NotNull(mesh2d);
                    Assert.AreEqual(16, mesh2d.NumNodes);

                    mesh2d.Dispose();
                    disposableGeometryList.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }

        [Test]
        public void Mesh2dMakeUniformOnExtensionThroughAPI()
        {
            
            using (var api = new MeshKernelApi())
            {
                var id = 0;
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
                    Assert.IsTrue(api.Mesh2dMakeUniformOnExtension(id, ref makeGridParameters));

                    // Assert
                    var mesh2D = new DisposableMesh2D();
                    Assert.IsTrue(api.Mesh2dGetData(id, out mesh2D));

                    Assert.AreEqual(mesh2D.NumNodes,121);

                    mesh2D.Dispose();

                }
                finally
                {
                    api.DeallocateState(id);
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
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    var meshRefinementParameters = MeshRefinementParameters.CreateDefault();
                    var griddedSamples = new DisposableGriddedSamples();
                    griddedSamples.NumX = 3;
                    griddedSamples.NumY = 3;
                    griddedSamples.OriginX = 0.0;
                    griddedSamples.OriginY = 0.0;
                    griddedSamples.CellSize = 1;
                    griddedSamples.Values = new[] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 } ;

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    // Execute
                    Assert.IsTrue(api.Mesh2dRefineBasedOnGriddedSamples(id, 
                                                                        ref griddedSamples, 
                                                                        ref meshRefinementParameters, 
                                                                        true));
                    var meshOut = new DisposableMesh2D();
                    Assert.IsTrue(api.Mesh2dGetData(id, out meshOut));

                    // Assert
                    Assert.NotNull(meshOut);
                    Assert.AreEqual(meshOut.NumNodes, 16);

                    meshOut.Dispose();
                    griddedSamples.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Mesh2dTriangulationInterpolationThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var mesh = CreateMesh2D(3, 3, 1, 1))
            {
                var id = 0;
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    int locationType = -1;
                    Assert.IsTrue(api.GetNodesLocationType(ref locationType));
                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var mesh2D = new DisposableMesh2D();
                    var success = api.Mesh2dGetData(id, out mesh2D);
                    Assert.IsTrue(success);

                    var samples = new DisposableGeometryList();
                    samples.XCoordinates = new[] { 1.0, 2.0, 3.0, 1.0 };
                    samples.YCoordinates = new[] { 1.0, 3.0, 2.0, 4.0 };
                    samples.Values = new[] { 3.0, 10, 4.0, 5.0 };
                    samples.NumberOfCoordinates = 4;

                    var results = new DisposableGeometryList();
                    results.XCoordinates = mesh2D.NodeX;
                    results.YCoordinates = mesh2D.NodeY;
                    results.Values = new double [mesh.NumNodes];
                    results.NumberOfCoordinates = mesh.NumNodes;

                    // Execute
                    Assert.IsTrue(api.Mesh2dTriangulationInterpolation(id, ref samples, locationType, ref results));

                    // Assert
                    Assert.AreEqual(results.Values[4], 3);

                    mesh2D.Dispose();
                    samples.Dispose();
                    results.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }


        [Test]
        public void Network1dComputeFixedChainagesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var network1d = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    int locationType = -1;
                    Assert.IsTrue(api.GetNodesLocationType(ref locationType));

                    var separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0};
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0};
                    network1d.GeometrySeparator =  separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.IsTrue(api.Network1dSet(id, network1d));

                    double [] fixedChainages ={ 5.0, separator, 5.0 };
                    double minFaceSize = 0.01;
                    double fixedChainagesOffset = 10.0;

                    Assert.IsTrue(api.Network1dComputeFixedChainages(id, ref fixedChainages,  minFaceSize, fixedChainagesOffset));
                    Assert.IsTrue(api.Network1dToMesh1d(id, minFaceSize));

                    var mesh1D = new DisposableMesh1D();
                    var success = api.Mesh1dGetData(id, out mesh1D);
                    Assert.IsTrue(success);

                    Assert.AreEqual(mesh1D.NumNodes, 6);
                    Assert.AreEqual(mesh1D.NumEdges, 4);
                    mesh1D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }



    }

}
