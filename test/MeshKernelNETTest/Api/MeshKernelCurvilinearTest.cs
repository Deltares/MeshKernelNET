using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using MeshKernelNET.Api;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETCurvilinearTests")]
    public class MeshKernelCurvilinearTest
    {
        public static DisposableCurvilinearGrid CreateCurvilinearGrid(
            int numbOfRows = 3,
            int numbOfColumns = 3,
            double cellWidth = 1.0,
            double cellHeight = 1.0)
        {
            var result = new DisposableCurvilinearGrid(numbOfRows, numbOfColumns);

            var nodeIndex = 0;
            for (var i = 0; i < result.NumM; ++i)
            {
                for (var j = 0; j < result.NumN; ++j)
                {
                    result.NodeX[nodeIndex] = i * cellWidth;
                    result.NodeY[nodeIndex] = j * cellHeight;
                    nodeIndex++;
                }
            }

            return result;
        }

        [TestCase(3, 2, 2.0, 1.0)]
        [TestCase(2, 3, 2.0, 1.5)]
        public void TestUtility_CreateCurvilinearGrid_EqualToMakeGridParameters(int numRows, int numCols, double cellWidth, double cellHeight)
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid fromKernel = null;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridType = 0;
                    makeGridParameters.NumberOfColumns = numCols - 1;
                    makeGridParameters.NumberOfRows = numRows - 1;
                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = cellWidth;
                    makeGridParameters.YGridBlockSize = cellHeight;
                    makeGridParameters.UpperRightCornerXCoordinate = 0.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 0.0;

                    Assert.That(api.CurvilinearComputeRectangularGrid(id, makeGridParameters), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out fromKernel), Is.EqualTo(0));
                    Assert.That(fromKernel, Is.Not.Null);
                    Assert.That((fromKernel.NumN, fromKernel.NumM), Is.EqualTo((numRows, numCols)));

                    using (DisposableCurvilinearGrid subject = CreateCurvilinearGrid(numRows, numCols, cellWidth, cellHeight))
                    {
                        Assert.That((fromKernel.NumN, fromKernel.NumM), Is.EqualTo((subject.NumN, subject.NumM)));
                        Assert.That(subject.NodeX, Is.EquivalentTo(fromKernel.NodeX));
                        Assert.That(subject.NodeY, Is.EquivalentTo(fromKernel.NodeY));
                    }
                }
                finally
                {
                    api.ClearState();
                    fromKernel?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeRectangularGridThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
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
                    makeGridParameters.UpperRightCornerXCoordinate = 0.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 0.0;

                    Assert.That(api.CurvilinearComputeRectangularGrid(id, makeGridParameters), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That(curvilinearGrid.NumM, Is.EqualTo(4));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeRectangularGridFromPolygonThroughAPIFails()
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
                    makeGridParameters.XGridBlockSize = 1;
                    makeGridParameters.YGridBlockSize = 1;

                    // geometry list is empty, expect an algorithm error to be thrown in the backend
                    int algorithmErrorExitCode = -1;
                    api.GetExitCodeAlgorithmError(ref algorithmErrorExitCode);
                    Assert.That(api.CurvilinearComputeRectangularGridFromPolygon(id, makeGridParameters, polygon), Is.EqualTo(algorithmErrorExitCode));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void CurvilinearComputeRectangularGridFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var polygon = new DisposableGeometryList())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    var makeGridParameters = MakeGridParameters.CreateDefault();

                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.XGridBlockSize = 1;
                    makeGridParameters.YGridBlockSize = 1;

                    polygon.XCoordinates = new[] { 0.5, 2.5, 5.5, 3.5, 0.5 };
                    polygon.YCoordinates = new[] { 2.5, 0.5, 3.0, 5.0, 2.5 };
                    polygon.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
                    polygon.NumberOfCoordinates = 5;

                    Assert.That(api.CurvilinearComputeRectangularGridFromPolygon(id, makeGridParameters, polygon), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((11, 11)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeTransfiniteFromSplinesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { 1.340015E+02, 3.642529E+02, 6.927549E+02, geometrySeparator, 2.585022E+02, 4.550035E+02, 8.337558E+02, geometrySeparator, 1.002513E+02, 4.610035E+02, geometrySeparator, 6.522547E+02, 7.197551E+02 };

                    geometryListIn.YCoordinates = new[] { 2.546282E+02, 4.586302E+02, 5.441311E+02, geometrySeparator, 6.862631E+01, 2.726284E+02, 3.753794E+02, geometrySeparator, 4.068797E+02, 7.912642E+01, geometrySeparator, 6.026317E+02, 2.681283E+02 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, geometrySeparator, 0.0, 0.0, 0.0, geometrySeparator, 0.0, 0.0, geometrySeparator, 0.0, 0.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;
                    var curvilinearParameters = new CurvilinearParameters();
                    curvilinearParameters.MRefinement = 10;
                    curvilinearParameters.NRefinement = 10;
                    curvilinearParameters.SmoothingIterations = 10;
                    curvilinearParameters.SmoothingParameter = 0.5;
                    curvilinearParameters.AttractionParameter = 0.0;
                    Assert.That(api.CurvilinearComputeTransfiniteFromSplines(id, geometryListIn, curvilinearParameters), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeOrthogonalGridFromSplinesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    ;
                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 6;

                    geometryListIn.XCoordinates = new[] { 1.175014E+02, 3.755030E+02, 7.730054E+02, geometrySeparator, 4.100089E+01, 3.410027E+02 };

                    geometryListIn.YCoordinates = new[] { 2.437587E+01, 3.266289E+02, 4.563802E+02, geometrySeparator, 2.388780E+02, 2.137584E+01 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, geometrySeparator, 0.0, 0.0, geometrySeparator };

                    var curvilinearParameters = CurvilinearParameters.CreateDefault();
                    curvilinearParameters.MRefinement = 40;
                    curvilinearParameters.NRefinement = 10;
                    var splinesToCurvilinearParameters = SplinesToCurvilinearParameters.CreateDefault();
                    splinesToCurvilinearParameters.GrowGridOutside = false;

                    Assert.That(api.CurvilinearComputeOrthogonalGridFromSplines(id, geometryListIn,
                                                                                       curvilinearParameters, splinesToCurvilinearParameters), Is.EqualTo(0));

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    geometryListIn.Dispose();
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeGridFromSplinesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    

                    geometryListIn.XCoordinates = new[] {
                        -1.0, 11.0,
                        geometrySeparator,
                        10.0, 10.0,
                        geometrySeparator,
                        11.0, -1.0,
                        geometrySeparator,
                        0.0, 0.0};
                    geometryListIn.YCoordinates = new[] {
                        0.0, 0.0,
                        geometrySeparator,
                        11.0, -1.0,
                        geometrySeparator,
                        10.0, 10.0,
                        geometrySeparator,
                        11.0, -1.0};

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var curvilinearParameters = CurvilinearParameters.CreateDefault();
                    curvilinearParameters.MRefinement = 5;
                    curvilinearParameters.NRefinement = 5;

                    Assert.That(api.CurvilinearComputeGridFromSplines(id, geometryListIn, curvilinearParameters), Is.EqualTo(0));

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    geometryListIn.Dispose();
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
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
                DisposableCurvilinearGrid curvilinearGrid = null;
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
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid),
                                    Is.EqualTo(0)); // Assert a valid mesh is produced
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((3, 3)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
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
                DisposableCurvilinearGrid curvilinearGrid = null;
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
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearDeleteNodeThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5))
            using (var api = new MeshKernelApi())
            {
                DisposableCurvilinearGrid curvilinearGrid = null;
                var id = 0;
                try
                {
                    // Setup
                    id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));            // Execute
                    Assert.That(api.CurvilinearDeleteNode(id, 5.0, 5.0), Is.EqualTo(0)); // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid.NodeX[24], Is.EqualTo(-999.0));
                    Assert.That(curvilinearGrid.NodeX[24], Is.EqualTo(-999.0));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeOrthogonalGridFromSplinesIterativeThroughAPI()
        {
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var curvilinearGrid = new DisposableCurvilinearGrid();
                var geometryListIn = new DisposableGeometryList();
                try
                {
                    // Setup
                    id = api.AllocateState(0);

                    double geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 6;

                    geometryListIn.XCoordinates = new[] { 1.175014E+02, 3.755030E+02, 7.730054E+02, geometrySeparator, 4.100089E+01, 3.410027E+02 };

                    geometryListIn.YCoordinates = new[] { 2.437587E+01, 3.266289E+02, 4.563802E+02, geometrySeparator, 2.388780E+02, 2.137584E+01 };

                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, geometrySeparator, 0.0, 0.0, geometrySeparator };

                    var curvilinearParameters = CurvilinearParameters.CreateDefault();
                    curvilinearParameters.MRefinement = 40;
                    curvilinearParameters.NRefinement = 10;

                    var splinesToCurvilinearParameters = SplinesToCurvilinearParameters.CreateDefault();
                    splinesToCurvilinearParameters.GrowGridOutside = false;

                    // Execute
                    Assert.That(api.CurvilinearInitializeOrthogonalGridFromSplines(id,
                                                                                          geometryListIn,
                                                                                          curvilinearParameters,
                                                                                          splinesToCurvilinearParameters), Is.EqualTo(0));

                    var numLayers = 3;
                    for (var i = 1; i < numLayers; i++)
                    {
                        Assert.That(api.CurvilinearIterateOrthogonalGridFromSplines(id, i), Is.EqualTo(0));
                        Assert.That(api.CurvilinearRefreshOrthogonalGridFromSplines(id), Is.EqualTo(0));
                    }

                    Assert.That(api.CurvilinearDeleteOrthogonalGridFromSplines(id), Is.EqualTo(0));

                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 3)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                    geometryListIn?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearDerefineThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.CurvilinearDerefine(id, 10.0, 20.0, 30.0, 20.0), Is.EqualTo(0));
                    // Assert

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 4)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearLineShiftThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                DisposableCurvilinearGrid curvilinearGrid = null;
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.CurvilinearInitializeLineShift(id), Is.EqualTo(0));
                    Assert.That(api.CurvilinearSetLineLineShift(id, 0.0, 0.0, 0.0, 30.0), Is.EqualTo(0));
                    Assert.That(api.CurvilinearSetBlockLineShift(id, 0.0, 0.0, 30.0, 30.0), Is.EqualTo(0));
                    Assert.That(api.CurvilinearMoveNodeLineShift(id, 0.0, 0.0, -10.0, 0.0), Is.EqualTo(0));
                    Assert.That(api.CurvilinearFinalizeLineShift(id), Is.EqualTo(0));
                    // Assert

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid, Is.Not.Null);
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 5)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearOrthogonalizeWithFrozenLineThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                DisposableCurvilinearGrid curvilinearGrid = null;
                var id = 0;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    var orthogonalizationParameters = OrthogonalizationParameters.CreateDefault();
                    int frozenLineId = -1;
                    Assert.That(api.CurvilinearFrozenLineAdd(id,
                                                                      10.0,
                                                                      0.0,
                                                                      10.0,
                                                                      10.0,
                                                                     ref frozenLineId), Is.EqualTo(0));
                    Assert.That(frozenLineId, Is.EqualTo(0));
                    Assert.That(api.CurvilinearOrthogonalize(id,
                                                                    ref orthogonalizationParameters,
                                                                    20.0,
                                                                    0.0,
                                                                    20.0,
                                                                    10.0), Is.EqualTo(0));

                    // Assert
                    int gridOut = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.That(gridOut, Is.EqualTo(0));
                    Assert.That(curvilinearGrid, Is.Not.Null);
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearSmoothingThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));

                    // Execute
                    int frozenLineId = -1;
                    Assert.That(api.CurvilinearFrozenLineAdd(id, 10.0, 0.0, 10.0, 10.0, ref frozenLineId), Is.EqualTo(0));
                    Assert.That(frozenLineId, Is.EqualTo(0));
                    Assert.That(api.CurvilinearSmoothing(id, 10, 10.0, 20.0, 30.0, 20.0), Is.EqualTo(0));

                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearSmoothingDirectionalThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.CurvilinearSmoothingDirectional(id,
                                                                           10,
                                                                           10.0,
                                                                           0.0,
                                                                           10.0,
                                                                           30.0,
                                                                           10.0,
                                                                           0.0,
                                                                           30.0,
                                                                           0.0), Is.EqualTo(0));

                    // Assert

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid.NumM, Is.EqualTo(5));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearSnapToLandBoundaryThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (DisposableGeometryList land = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    // Prepare
                    var id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    land.XCoordinates = new[]{0.0, 10.0, 20.0, 30.0, 40.0 };
                    land.YCoordinates = new[]{-10.0, -10.0, -10.0, -10.0, -10.0 };
                    land.Values = new[] {0.0, 0.0, 0.0, 0.0, 0.0 };
                    land.NumberOfCoordinates = land.XCoordinates.Length;

                    // Execute
                    Assert.That(api.CurvilinearSnapToLandBoundary(id,
                                                                         land,
                                                                         0.0,
                                                                         0.0,
                                                                         30.0,
                                                                         0.0,
                                                                         20.0,
                                                                         20.0), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid.NumM, Is.EqualTo(5));
                    Assert.That(curvilinearGrid.NodeX[0], Is.EqualTo(0.0));
                    Assert.That(curvilinearGrid.NodeY[0], Is.EqualTo(-10.0));
                    Assert.That(curvilinearGrid.NodeY[15], Is.EqualTo(-10.0));
                    Assert.That(curvilinearGrid.NodeY[20], Is.EqualTo(0.0));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearSnapToSplineThroughAPI()
        {
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (DisposableGeometryList spline = new DisposableGeometryList())
            using (var api = new MeshKernelApi())
            {
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    // Prepare
                    var id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    spline.XCoordinates = new[] { 0.0, 10.0, 20.0, 30.0, 40.0 };
                    spline.YCoordinates = new[] { -10.0, -10.0, -10.0, -10.0, -10.0 };
                    spline.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
                    spline.NumberOfCoordinates = spline.XCoordinates.Length;

                    // Execute
                    Assert.That(api.CurvilinearSnapToSpline(id,
                                                                   spline,
                                                                   0.0,
                                                                   0.0,
                                                                   30.0,
                                                                   0.0,
                                                                   20.0,
                                                                   20.0), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That(curvilinearGrid.NumM, Is.EqualTo(5));
                    Assert.That(curvilinearGrid.NodeX[0], Is.EqualTo(0.0));
                    Assert.That(curvilinearGrid.NodeY[0], Is.EqualTo(-10.0));
                    Assert.That(curvilinearGrid.NodeY[15], Is.EqualTo(-10.0));
                    Assert.That(curvilinearGrid.NodeY[20], Is.EqualTo(0.0));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearRefineThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(4, 4, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    Assert.That(api.CurvilinearRefine(id, 10.0, 20.0, 20.0, 20.0, 10), Is.EqualTo(0));

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((4, 13)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearOrthogonalizeOnBlockThroughAPI()
        {
            // Setup
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));

                    // Execute
                    var orthogonalizationParameters = OrthogonalizationParameters.CreateDefault();
                    Assert.That(api.CurvilinearOrthogonalize(id, ref orthogonalizationParameters, 0.0, 0.0, 30.0, 30.0), Is.EqualTo(0).Within(0));

                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 5)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearMoveNodeThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));

                    // Execute
                    Assert.That(api.CurvilinearMoveNode(id, 0.0, 0.0, -10.0, 0.0), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 5)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearComputeRectangularGridOnExtensionThroughAPI()
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
                    makeGridParameters.GridAngle = 0.0;
                    makeGridParameters.OriginXCoordinate = 0.0;
                    makeGridParameters.OriginYCoordinate = 0.0;
                    makeGridParameters.XGridBlockSize = 1.0;
                    makeGridParameters.YGridBlockSize = 2.0;
                    makeGridParameters.UpperRightCornerXCoordinate = 10.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 10.0;

                    Assert.That(api.CurvilinearComputeRectangularGridOnExtension(id, makeGridParameters), Is.EqualTo(0));
                    var grid = new DisposableCurvilinearGrid();
                    Assert.That(api.CurvilinearGridGetData(id, out grid), Is.EqualTo(0));
                    Assert.That((grid.NumM, grid.NumN), Is.EqualTo((11, 6)));
                }
                finally
                {
                    api.ClearState();
                }
            }
        }

        [Test]
        public void CurvilinearLineMirrorThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.CurvilinearLineMirror(id, 1.2, 0.0, 0.0, 0.0, 50.0), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 6)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearLineAttractionRepulsionThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));

                    Assert.That(api.CurvilinearLineAttractionRepulsion(id,
                                                                              0.5,
                                                                              30.0, 0.0, 30.0, 50.0,
                                                                              10.0, 20.0, 50.0, 20.0), Is.EqualTo(0));

                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    // Assert
                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 5)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearInsertFaceThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                try
                {
                    id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    // Execute
                    Assert.That(api.CurvilinearInsertFace(id, -5.0, 5.0), Is.EqualTo(0));
                    // Assert
                    Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));

                    Assert.That((curvilinearGrid.NumM, curvilinearGrid.NumN), Is.EqualTo((5, 6)));
                }
                finally
                {
                    api.ClearState();
                    curvilinearGrid?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearGetCurvatureThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                {
                    try
                    {
                        // Prepare
                        id = api.AllocateState(0);
                        Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                        Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                        var curvature = new double[curvilinearGrid.NumM * curvilinearGrid.NumN];

                        // Execute
                        Assert.That(api.CurvilinearComputeCurvature(id, CurvilinearDirectionOptions.M, ref curvature), Is.EqualTo(0));

                        // Assert
                        var tolerance = 1e-9;
                        Assert.That(curvature[0], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(curvature[1], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[2], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[3], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[4], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(curvature[5], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(curvature[6], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[7], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[8], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(curvature[9], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(curvature[10], Is.EqualTo(-999.0).Within(tolerance));
                    }
                    finally
                    {
                        api.ClearState();
                        curvilinearGrid?.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CurvilinearGetSmoothnessThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                {
                    try
                    {
                        // Prepare
                        id = api.AllocateState(0);
                        Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                        Assert.That(api.CurvilinearGridGetData(id, out curvilinearGrid), Is.EqualTo(0));
                        var smoothness = new double[curvilinearGrid.NumM * curvilinearGrid.NumN];

                        // Execute
                        Assert.That(api.CurvilinearComputeSmoothness(id, CurvilinearDirectionOptions.M, ref smoothness), Is.EqualTo(0));

                        // Assert
                        var tolerance = 1e-9;
                        Assert.That(smoothness[0], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(smoothness[1], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(smoothness[5], Is.EqualTo(-999.0).Within(tolerance));
                        Assert.That(smoothness[6], Is.EqualTo(1.0).Within(tolerance));
                        Assert.That(smoothness[7], Is.EqualTo(1.0).Within(tolerance));
                    }
                    finally
                    {
                        api.ClearState();
                        curvilinearGrid?.Dispose();
                    }
                }
            }
        }

        [Test]
        public void CurvilinearGrid_GetNodeXY_AccessesNodeXY()
        {
            // Setup
            using (var grid = CreateCurvilinearGrid(3, 2, 5.0, 2.0))
            {
                Assert.That((grid.NumM, grid.NumN), Is.EqualTo((2, 3)));
                for (int row = 0; row < grid.NumM; ++row)
                {
                    for (int column = 0; column < grid.NumN; ++column)
                    {
                        int index = row * grid.NumN + column;
                        Assert.That(grid.NodeX[index], Is.EqualTo(grid.GetNodeX(index)));
                        Assert.That(grid.NodeY[index], Is.EqualTo(grid.GetNodeY(index)));
                    }
                }
            }
        }

        [TestCase(0, 0, 4)]    // first column-oriented edge
        [TestCase(15, 15, 19)] // last column-oriented edge
        [TestCase(16, 0, 1)]   // first row-oriented edge
        [TestCase(30, 18, 19)] // last row-oriented edge
        public void CurvilinearGrid_GetEdgeNodeIndexMethods_FollowMeshKernelEdgeOrderingConvention(int edgeIndex, int firstIndex, int secondIndex)
        {
            // first all column-oriented edges in row-major order, then all row-oriented edges in row-major order
            using (var grid = CreateCurvilinearGrid(5, 4, 5.0, 2.0))
            {
                Assert.That((grid.NumN, grid.NumM), Is.EqualTo((5, 4)));
                Assert.That((grid.GetFirstNode(edgeIndex), grid.GetLastNode(edgeIndex)), Is.EqualTo((firstIndex, secondIndex)));
            }
        }

        [TestCase(5, 3, 5 * 3)]
        [TestCase(15, 42, 15 * 42)]
        public void CurvilinearGrid_GetNodeCount_CountsAllNodes(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.That((grid.NumN, grid.NumM), Is.EqualTo((n, m)));
                Assert.That(grid.NodeCount(), Is.EqualTo(count));
            }
        }

        [TestCase(5, 3, 4 * 3 + 5 * 2)]
        [TestCase(15, 42, 15 * 41 + 14 * 42)]
        public void CurvilinearGrid_GetEdgeCount_CountsRowOrientedAndColumnOrientedEdges(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.That((grid.NumN, grid.NumM), Is.EqualTo((n, m)));
                Assert.That(grid.EdgeCount(), Is.EqualTo(count));
            }
        }

        [TestCase(5, 3, (5 - 1) * (3 - 1))]
        [TestCase(15, 42, (15 - 1) * (42 - 1))]
        public void CurvilinearGrid_GetCellCount_CountsAllCells(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.That((grid.NumN, grid.NumM), Is.EqualTo((n, m)));
                Assert.That(grid.CellCount(), Is.EqualTo(count));
            }
        }

        [TestCaseSource(nameof(EdgeNodesSerialization))]
        public void CurvilinearGrid_EdgeNodesSerialization(int numM, int numN, IList<(int, int)> edgeNodes)
        {
            using (var grid = CreateCurvilinearGrid(numN, numM, 5.0, 2.0))
            {
                Assert.That((grid.NumN, grid.NumM), Is.EqualTo((numN, numM)));
                for (int i = 0; i < grid.EdgeCount(); ++i)
                {
                    Assert.That((grid.GetFirstNode(i), grid.GetLastNode(i)), Is.EqualTo(edgeNodes[i]));
                }
            }
        }

        public static object[] EdgeNodesSerialization = { new object[] { 2, 3, new[] { (0, 2), (1, 3), (2, 4), (3, 5), (0, 1), (2, 3), (4, 5) } }, new object[] { 3, 2, new[] { (0, 3), (1, 4), (2, 5), (0, 1), (1, 2), (3, 4), (4, 5) } } };

        [Test]
        public void CurvilinearSetAndCovertThroughApi()
        {

            // Setup
            using (var api = new MeshKernelApi())
            using (DisposableCurvilinearGrid grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                DisposableCurvilinearGrid curvilinearGrid = null;
                var mesh2d = new DisposableMesh2D();
                {
                    try
                    {
                        // Prepare
                        id = api.AllocateState(0);
                        Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                        Assert.That(api.CurvilinearConvertToMesh2D(id), Is.EqualTo(0));
                        Assert.That(api.Mesh2dGetData(id, out mesh2d), Is.EqualTo(0));

                        // Assert
                        Assert.That(mesh2d.NumNodes, Is.EqualTo(25));
                    }
                    finally
                    {
                        api.ClearState();
                        curvilinearGrid?.Dispose();
                        mesh2d.Dispose();
                    }
                }
            }
        }
        
        [Test]
        [TestCase("CurvilinearDeleteExterior", 0.0, -999.0)]
        [TestCase("CurvilinearDeleteInterior", 0.0,   0.0)]
        public void CurvilinearDeleteThroughApi(string methodName, double expectedBefore, double expectedAfter)
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                var disposableCurvilinearGridBefore = new DisposableCurvilinearGrid();
                var disposableCurvilinearGridAfter = new DisposableCurvilinearGrid();
                try
                {
                    // Prepare
                    id = api.AllocateState(0);
                    Assert.That(api.CurvilinearSet(id, grid), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGridGetData(id, out disposableCurvilinearGridBefore), Is.EqualTo(0));

                    var box = new BoundingBox() 
                    {
                        xLowerLeft = 0.0,
                        yLowerLeft = 0.0,
                        xUpperRight = 20.0,
                        yUpperRight = 20.0
                    };

                    // Call the method dynamically based on the parameter
                    if (methodName == "CurvilinearDeleteExterior")
                    {
                        Assert.That(api.CurvilinearDeleteExterior(id, box), Is.EqualTo(0));
                    }
                    else if (methodName == "CurvilinearDeleteInterior")
                    {
                        Assert.That(api.CurvilinearDeleteInterior(id, box), Is.EqualTo(0));
                    }
                    else
                    {
                        Assert.Fail("Invalid method name provided");
                    }

                    Assert.That(api.CurvilinearGridGetData(id, out disposableCurvilinearGridAfter), Is.EqualTo(0));

                    // Assert
                    var nodeXBefore = disposableCurvilinearGridBefore.NodeX[3];
                    var nodeXAfter = disposableCurvilinearGridAfter.NodeX[3];
                    Assert.That(nodeXBefore, Is.EqualTo(expectedBefore));
                    Assert.That(nodeXAfter, Is.EqualTo(expectedAfter));
                }
                finally
                {
                    api.ClearState();
                    disposableCurvilinearGridBefore?.Dispose();
                    disposableCurvilinearGridAfter?.Dispose();
                }
            }
        }

        [Test]
        public void CurvilinearGetBoundariesAsPolygons()
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
                    makeGridParameters.XGridBlockSize = 1.0;
                    makeGridParameters.YGridBlockSize = 1.0;
                    makeGridParameters.UpperRightCornerXCoordinate = 0.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 0.0;

                    Assert.That(api.CurvilinearComputeRectangularGrid(id, makeGridParameters), Is.EqualTo(0));
                    Assert.That(api.CurvilinearGetBoundariesAsPolygons(id, 0, 0, 3, 3, out DisposableGeometryList boundaryPolygons), Is.EqualTo(0));

                    var expectedPolygonXCoordinates = new[] { 0, 1, 2, 3, 3, 3, 3, 2, 1, 0, 0, 0, 0 };
                    var expectedPolygonYCoordinates = new[] { 0, 0, 0, 0, 1, 2, 3, 3, 3, 3, 2, 1, 0 };

                    Assert.That(boundaryPolygons.GeometrySeparator,Is.EqualTo(api.GetSeparator()));
                    Assert.That(boundaryPolygons.InnerOuterSeparator,Is.EqualTo(api.GetInnerOuterSeparator()));
                    Assert.That(boundaryPolygons.XCoordinates, Is.EquivalentTo(expectedPolygonXCoordinates));
                    Assert.That(boundaryPolygons.YCoordinates, Is.EquivalentTo(expectedPolygonYCoordinates));

                    boundaryPolygons.Dispose();
                }
                finally
                {
                    api.ClearState();
                }
            }
        }
    }
    
    [TestFixture]
    class CurvilinearGetNodeLocationIndexTest
    {
        private int id;
        private MeshKernelApi api;
        private (double x, double y)[] vertices;

        [SetUp]
        public void SetUp()
        {
            // Setup
            api = new MeshKernelApi();
            id = api.AllocateState(0);
        }

        [TearDown]
        public void TearDown()
        {
            api.ClearState();
            api.Dispose();
            api = null;
        }
        
        private void CreateGrid(int nx, int ny, double x0, double y0, double dx, double dy)
        {
            MakeGridParameters makeGridParameters = new MakeGridParameters()
            {
                NumberOfColumns = ny - 1,
                NumberOfRows = nx - 1,
                OriginXCoordinate = x0,
                XGridBlockSize = dx,
                OriginYCoordinate = y0,
                YGridBlockSize = dy
            };
            api.CurvilinearComputeRectangularGrid(id, makeGridParameters);
            api.CurvilinearGridGetData(id, out DisposableCurvilinearGrid grid);
            Assert.Multiple(() =>
            {
                Assert.That(grid.NodeCount(), Is.EqualTo(nx*ny));
                Assert.That(grid.NumM, Is.EqualTo(ny));
                Assert.That(grid.NumN, Is.EqualTo(nx));
            });
            vertices = new (double x, double y)[nx*ny];
            for (int i = 0; i < nx*ny; ++i)
                vertices[i] = (grid.GetNodeX(i), grid.GetNodeY(i));
        }

        [Test]
        public void CreateGridSucceeds()
        {
            CreateGrid(5,4,1.0,1.0,1.0,1.0);

            (double x, double y)[] expectedVertices_5x4 = { 
                (1, 1), (2, 1), (3, 1), (4, 1),  // 0 - 3
                (1, 2), (2, 2), (3, 2), (4, 2),  // 4 - 7
                (1, 3), (2, 3), (3, 3), (4, 3),  // 8 - 11
                (1, 4), (2, 4), (3, 4), (4, 4),  // 12 - 15
                (1, 5), (2, 5), (3, 5), (4, 5) }; // 16 - 19

            Assert.That(vertices,Is.EquivalentTo(expectedVertices_5x4));
        }

        private void AssertThatPointIsCloseToExpectedVertex(double x, double y, int expectedIndex)
        {
            // Check that point is closest to expected vertex
            Coordinate vertex = new Coordinate(vertices[expectedIndex].x, vertices[expectedIndex].y);
            Coordinate point = new Coordinate(x, y);
            Assert.That(vertex.Distance(point),Is.LessThan(Math.Sqrt(0.5*0.5 + 0.5*0.5)));
        }
        
        [TestCase(1.0,1.0, 0)]
        [TestCase(.99, .95, 0)]  // slightly below and left of vertex (outside mesh perimeter)
        [TestCase(1.01,1.01, 0)] // slightly above and right of vertex
        
        [TestCase(2.0,2.0, 5)]    // on vertex
        [TestCase(1.97,1.999, 5)] // slightly below and left of vertex
        [TestCase(2.01,2.01, 5)]  // slightly above and right of vertex
        
        [TestCase(4.0,4.0, 15)]
        [TestCase(4.01,3.99, 15)] // slightly below and right of vertex (outside mesh perimeter)
        [TestCase(3.99,4.04, 15)] // slightly above and right of vertex
        
        [TestCase(4.0,1.0,3)]
        [TestCase(3.99,0.99,3)] // slightly below and left of vertex (outside mesh perimeter)
        [TestCase(4.01,1.01,3)] // slightly above and right of vertex (outside mesh perimeter)

        [TestCase(1.0,5.0,16)]
        [TestCase(1.01,5.01,16)] // slightly above and right of vertex (outside mesh perimeter)
        [TestCase(0.99,4.99,16)] // slightly below and left of vertex (outside mesh perimeter)
        
        [TestCase(4.0,5.0,19)]
        [TestCase(4.01,5.01,19)] // slightly above and right of vertex (outside mesh perimeter)
        [TestCase(3.99,4.99,19)] // slightly below and left of vertex (outside mesh perimeter)
        public void CurvilinearGetNodeLocationIndex_AtOrCloseToVertexPositionFindsNearest(double x, double y, int expectedIndex)
        {
            // Setup
            CreateGrid(5,4,1.0,1.0,1.0,1.0);
            AssertThatPointIsCloseToExpectedVertex(x,y,expectedIndex);
            
            // Call
            var box = new BoundingBox() // box around grid with 1.0 margin
            {
                xLowerLeft = 0,
                yLowerLeft = 0,
                xUpperRight = 5,
                yUpperRight = 6
            };
            int actualIndex = -1;
            int returnCode = api.CurvilinearGetNodeLocationIndex(id,x,y,box, ref actualIndex);

            // Assert
            Assert.That(actualIndex,Is.EqualTo(expectedIndex));
        }

        // on vertex
        [TestCase(3.0,3.0)]
        
        // inside bounding box not on vertex
        [TestCase(2.9,3.1)]
        [TestCase(3.2,2.9)]
        [TestCase(2.8,2.7)]
        [TestCase(3.3,3.2)]
        
        // edges of bounding box
        [TestCase(2.6,3.0)]
        [TestCase(3.4,3.0)]
        [TestCase(3.0,2.6)]
        [TestCase(3.0,3.4)]
        public void CurvilinearGetNodeLocationIndex_OnOrInBoundingBox_FindsNearest(double x, double y)
        {
            const int expectedIndex = 10;
            CreateGrid(5,4,1.0,1.0,1.0,1.0);
            AssertThatPointIsCloseToExpectedVertex(x, y, expectedIndex);
            
            // Call
            var box = new BoundingBox() // box (w=0.8,h=0.8) around point (3,3) index 10
            {
                xLowerLeft = 2.6,
                yLowerLeft = 2.6,
                xUpperRight = 3.4,
                yUpperRight = 3.4
            };
            int actualIndex = -1;
            int returnCode = api.CurvilinearGetNodeLocationIndex(id,x,y,box, ref actualIndex);

            // Assert
            Assert.That(actualIndex,Is.EqualTo(expectedIndex));
        }

        // close to 10
        [TestCase(3.4,3.0,10,10)]
        [TestCase(3.0,3.4,10,10)]
        [TestCase(2.6,3.0,10,10)]
        [TestCase(3.0,2.6,10,10)]

        // not close to 10
        [TestCase(3.9,3.0,11,10)]
        [TestCase(3.0,3.9,14,10)]
        [TestCase(2.1,3.0,9,10)]
        [TestCase(3.0,2.1,6,10)]
        public void CurvilinearGetNodeLocationIndex_DoesNotFindNearest_OutsideBoundingBox(double x, double y, int closeToIndex, int expectedIndex)
        {
            CreateGrid(5,4,1.0,1.0,1.0,1.0);
            AssertThatPointIsCloseToExpectedVertex(x, y, closeToIndex);

            // Call
            var box = new BoundingBox() // box (w=0.4,h=0.4) around point (3,3) index 10
            {
                xLowerLeft = 2.8,
                yLowerLeft = 2.8,
                xUpperRight = 3.2,
                yUpperRight = 3.2
            };
            int actualIndex = -1;
            int returnCode = api.CurvilinearGetNodeLocationIndex(id,x,y,box, ref actualIndex);

            // Assert
            Assert.That(actualIndex,Is.EqualTo(expectedIndex));
        }

        [Test]
        public void CurvilinearSetAndDeleteFrozenLines_ShouldSetAndDeleteFrozenLines()
        {
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int forzenLineId = -1;

            // Set and delete
            int returnCode = api.CurvilinearFrozenLineAdd(id, 
                                                           0.0,
                                                           0.0,
                                                           0.0,
                                                           2.0, 
                                                           ref forzenLineId);
            Assert.That(forzenLineId, Is.EqualTo(0));

            returnCode = api.CurvilinearFrozenLineDelete(id, forzenLineId);
            Assert.That(returnCode, Is.EqualTo(0));

            returnCode = api.CurvilinearFrozenLineAdd(id,
                                                       0.0,
                                                       0.0,
                                                       0.0,
                                                       2.0,
                                                       ref forzenLineId);
            Assert.That(returnCode, Is.EqualTo(0));

            // Id is always increasing
            Assert.That(forzenLineId, Is.EqualTo(1));

        }

        [Test]
        public void CurvilinearFrozenLineAdd_ShouldReturnValidId()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;

            // Act
            int returnCode = api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(frozenLineId, Is.EqualTo(0));
        }

        [Test]
        public void CurvilinearFrozenLinesGetCount_ShouldReturnCorrectCount()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;
            api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Act
            int numFrozenLines = 0;
            int returnCode = api.CurvilinearFrozenLinesGetCount(id, ref numFrozenLines);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(numFrozenLines, Is.EqualTo(1));
        }

        [Test]
        public void CurvilinearFrozenLinesGetIds_ShouldReturnCorrectIds()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;
            api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Act
            int numFrozenLines = 1;
            int[] frozenLinesIds;
            int returnCode = api.CurvilinearFrozenLinesGetIds(id, out frozenLinesIds);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(frozenLinesIds[0], Is.EqualTo(frozenLineId));
        }

        [Test]
        public void CurvilinearFrozenLineValid_ShouldReturnTrueForValidLine()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;
            api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Act
            bool isValid = false;
            int returnCode = api.CurvilinearFrozenLineIsValid(id, frozenLineId, ref isValid);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void CurvilinearFrozenLineGet_ShouldReturnCorrectCoordinates()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;
            api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Act
            double xFirstGridLineNode = 0.0;
            double yFirstGridLineNode = 0.0;
            double xSecondGridLineNode = 0.0;
            double ySecondGridLineNode = 0.0;
            int returnCode = api.CurvilinearFrozenLineGet(id, frozenLineId, ref xFirstGridLineNode, ref yFirstGridLineNode, ref xSecondGridLineNode, ref ySecondGridLineNode);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(xFirstGridLineNode, Is.EqualTo(0.0).Within(1e-9));
            Assert.That(yFirstGridLineNode, Is.EqualTo(0.0).Within(1e-9));
            Assert.That(xSecondGridLineNode, Is.EqualTo(0.0).Within(1e-9));
            Assert.That(ySecondGridLineNode, Is.EqualTo(2.0).Within(1e-9));
        }

        [Test]
        public void CurvilinearFrozenLineAdd_ShouldIncrementId()
        {
            // Arrange
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;
            api.CurvilinearFrozenLineAdd(id, 0.0, 0.0, 0.0, 2.0, ref frozenLineId);

            // Act
            int newFrozenLineId = -1;
            int returnCode = api.CurvilinearFrozenLineAdd(id, 1.0, 1.0, 1.0, 3.0, ref newFrozenLineId);

            // Assert
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(newFrozenLineId, Is.GreaterThan(frozenLineId));
        }

        [Test]
        public void CurvilinearSetAndDeleteFrozenLinesTwice_ShouldAddAndDeleteFrozenLineAndReturnErrorCode()
        {
            CreateGrid(5, 4, 1.0, 1.0, 1.0, 1.0);
            int frozenLineId = -1;

            // Set and delete
            int returnCode = api.CurvilinearFrozenLineAdd(id,
                                                          0.0,
                                                          0.0,
                                                          0.0,
                                                          2.0,
                                                          ref frozenLineId);
            Assert.That(frozenLineId, Is.EqualTo(0));

            returnCode = api.CurvilinearFrozenLineDelete(id, frozenLineId);
            Assert.That(returnCode, Is.EqualTo(0));
            returnCode = api.CurvilinearFrozenLineDelete(id, frozenLineId);
            Assert.That(returnCode, Is.EqualTo(1));
        }

    }
}
