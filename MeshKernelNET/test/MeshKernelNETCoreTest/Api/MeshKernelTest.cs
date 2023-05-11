using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETTests")]
    public class MeshKernelTest
    {
        public static DisposableMesh2D GenerateRegularGrid(
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
            using (var mesh = GenerateRegularGrid(100, 100, 100, 200))
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
                    var mesh2d = api.Mesh2dGetData(id);
                    var count = mesh2d.NodeX.Length;

                    Assert.AreEqual(numberOfVerticesBefore - 1, count);
                    Assert.NotNull(mesh2d);
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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    var numberOfVerticesBefore = mesh.NumNodes;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dDeleteNode(id, 0));

                    var mesh2d = api.Mesh2dGetData(id);
                    var count = mesh2d.NodeX.Length;

                    Assert.AreNotEqual(2, mesh.NumEdges);
                    Assert.AreEqual(numberOfVerticesBefore - 1, count);
                    Assert.NotNull(mesh2d);

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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
                    var count = mesh2d.NumEdges;

                    Assert.AreNotEqual(2, mesh2d.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 9, count);

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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
                    var count = mesh2d.NumEdges;

                    Assert.AreNotEqual(2, mesh2d.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 1, count);

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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    Assert.IsTrue(api.Mesh2dMergeTwoNodes(id, 0, 4));

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
                    var count = mesh2d.NumEdges;

                    Assert.AreNotEqual(2, mesh2d.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore - 1, count);

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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
                    var count = mesh2d.NumEdges;

                    Assert.AreNotEqual(2, mesh2d.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore, count);

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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
            using (var polygon = new DisposableGeometryList())
            using (var landBoundaries = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    var numberOfEdgesBefore = mesh.NumEdges;
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    Assert.IsTrue(api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries));

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
                    var count = mesh2d.NumEdges;

                    Assert.AreNotEqual(2, mesh2d.NumEdges);
                    Assert.AreEqual(numberOfEdgesBefore, count);

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
                    makeGridParameters.GridBlockSize = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.OriginZCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = 0.0;
                    makeGridParameters.YGridBlockSize = 0.0;

                    Assert.IsTrue(api.CurvilinearMakeUniform(id, makeGridParameters, disposableGeometryList));

                    var curvilinearGrid = api.CurvilinearGridGetData(id);
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(4, curvilinearGrid.NumM);

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
            using (var mesh = GenerateRegularGrid(0, 0, 100, 200))
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
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromSplines(id, geometryListIn, curvilinearParameters));

                    var curvilinearGrid = api.CurvilinearGridGetData(id);
                    Assert.NotNull(curvilinearGrid);

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
                    splinesToCurvilinearParameters.GrowGridOutside = 0;


                    Assert.IsTrue(api.CurvilinearComputeOrthogonalGridFromSplines(id, ref geometryListIn,
                        ref curvilinearParameters, ref splinesToCurvilinearParameters));

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);

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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);

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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
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
            using (var mesh = GenerateRegularGrid(4, 4, 100, 200))
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

                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);

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
            using (var mesh = GenerateRegularGrid(11, 11, 100, 100))
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
                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);

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
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromPolygon(id, geometryListIn, 0, 2, 4, true));

                    var curvilinearGrid = api.CurvilinearGridGetData(id);

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 3);
                    Assert.AreEqual(curvilinearGrid.NumN, 3);
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
                    Assert.IsTrue(api.CurvilinearComputeTransfiniteFromTriangle(id, geometryListIn, 0, 3, 6));

                    var curvilinearGrid = api.CurvilinearGridGetData(id);

                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(curvilinearGrid.NumM, 4);
                    Assert.AreEqual(curvilinearGrid.NumN, 5);
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

            using (var mesh = GenerateRegularGrid(11, 11, 100, 100))
            using (var geometryListIn = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { -5.0 };
                    geometryListIn.YCoordinates = new[] { -5.0 };
                    geometryListIn.Values = new[] { 0.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var geometryListOut = new DisposableGeometryList();
                    geometryListOut.XCoordinates = new[] { geometrySeparator };
                    geometryListOut.YCoordinates = new[] { geometrySeparator };
                    geometryListOut.Values = new[] { geometrySeparator };
                    geometryListOut.NumberOfCoordinates = 1;

                    //Call
                    double xCoordinateOut = 0.0;
                    double yCoordinateOut = 0.0;
                    Assert.IsTrue(api.Mesh2dGetClosestNode(id, -5.0, -5.0, 10.0, ref xCoordinateOut, ref yCoordinateOut));

                    //Assert
                    Assert.LessOrEqual(xCoordinateOut, 1e-6);
                    Assert.LessOrEqual(yCoordinateOut, 1e-6);
                    var mesh2d = api.Mesh2dGetData(id);
                    Assert.NotNull(mesh2d);
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
            using (var mesh = GenerateRegularGrid(4, 4, 10, 10))
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
                    Assert.IsTrue(api.ContactsComputeSingle(id, ref onedNodeMaskPinnedAddress, ref geometryListIn));

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
            using (var mesh = GenerateRegularGrid(4, 4, 10, 10))
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
            using (var mesh = GenerateRegularGrid(4, 4, 10, 10))
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
            using (var mesh = GenerateRegularGrid(4, 4, 10, 10))
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
            using (var mesh = GenerateRegularGrid(4, 4, 1, 1))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    Assert.IsTrue(api.Mesh2dSet(id, mesh));

                    var geometryListIn = new DisposableGeometryList();

                    geometryListIn.XCoordinates = new[] {  1.5, 1.5, 3.5,  3.5,  1.5 };
                    geometryListIn.YCoordinates = new[] { -1.5, 1.5, 1.5, -1.5, -1.5 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    geometryListIn.GeometrySeparator = api.GetSeparator();


                    int inside = 1;
                    var selectedVertices = api.GetSelectedVerticesInPolygon(id, ref geometryListIn, inside);
                    Assert.AreEqual(selectedVertices.Length, 4);
                }
                finally
                {
                    api.DeallocateState(id);

                }
            }
        }
    }
}
