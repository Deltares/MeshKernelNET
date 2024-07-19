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

                    Assert.AreEqual(0, api.CurvilinearComputeRectangularGrid(id, makeGridParameters));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out fromKernel));
                    Assert.NotNull(fromKernel);
                    Assert.AreEqual((numRows, numCols), (fromKernel.NumN, fromKernel.NumM));

                    using (DisposableCurvilinearGrid subject = CreateCurvilinearGrid(numRows, numCols, cellWidth, cellHeight))
                    {
                        Assert.AreEqual((subject.NumN, subject.NumM), (fromKernel.NumN, fromKernel.NumM));
                        Assert.That(subject.NodeX, Is.EquivalentTo(fromKernel.NodeX));
                        Assert.That(subject.NodeY, Is.EquivalentTo(fromKernel.NodeY));
                    }
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearComputeRectangularGrid(id, makeGridParameters));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual(4, curvilinearGrid.NumM);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(algorithmErrorExitCode, api.CurvilinearComputeRectangularGridFromPolygon(id, makeGridParameters, polygon));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearComputeRectangularGridFromPolygon(id, makeGridParameters, polygon));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((11, 11), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearComputeTransfiniteFromSplines(id, geometryListIn, curvilinearParameters));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearComputeOrthogonalGridFromSplines(id, geometryListIn,
                                                                                       curvilinearParameters, splinesToCurvilinearParameters));

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    geometryListIn.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearComputeTransfiniteFromPolygon(id, geometryListIn, 0, 2, 4, true));
                    Assert.AreEqual(0,
                                    api.CurvilinearGridGetData(id, out curvilinearGrid)); // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((3, 3), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearComputeTransfiniteFromTriangle(id, geometryListIn, 0, 3, 6));
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    // Assert a valid mesh is produced
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((5, 4), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));            // Execute
                    Assert.AreEqual(0, api.CurvilinearDeleteNode(id, 5.0, 5.0)); // Assert
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.AreEqual(-999.0, curvilinearGrid.NodeX[24]);
                    Assert.AreEqual(-999.0, curvilinearGrid.NodeX[24]);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearInitializeOrthogonalGridFromSplines(id,
                                                                                          geometryListIn,
                                                                                          curvilinearParameters,
                                                                                          splinesToCurvilinearParameters));

                    var numLayers = 3;
                    for (var i = 1; i < numLayers; i++)
                    {
                        Assert.AreEqual(0, api.CurvilinearIterateOrthogonalGridFromSplines(id, i));
                        Assert.AreEqual(0, api.CurvilinearRefreshOrthogonalGridFromSplines(id));
                    }

                    Assert.AreEqual(0, api.CurvilinearDeleteOrthogonalGridFromSplines(id));

                    // Assert
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.AreEqual((5, 3), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearDerefine(id, 10.0, 20.0, 30.0, 20.0));
                    // Assert

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((5, 4), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearInitializeLineShift(id));
                    Assert.AreEqual(0, api.CurvilinearSetLineLineShift(id, 0.0, 0.0, 0.0, 30.0));
                    Assert.AreEqual(0, api.CurvilinearSetBlockLineShift(id, 0.0, 0.0, 30.0, 30.0));
                    Assert.AreEqual(0, api.CurvilinearMoveNodeLineShift(id, 0.0, 0.0, -10.0, 0.0));
                    Assert.AreEqual(0, api.CurvilinearFinalizeLineShift(id));
                    // Assert

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.NotNull(curvilinearGrid);
                    Assert.AreEqual((5, 5), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    var orthogonalizationParameters = OrthogonalizationParameters.CreateDefault();
                    Assert.AreEqual(0, api.CurvilinearInitializeOrthogonalize(id, orthogonalizationParameters));
                    Assert.AreEqual(0, api.CurvilinearSetFrozenLinesOrthogonalize(id, 20.0, 0.0, 20.0, 10.0), 0);
                    Assert.AreEqual(0, api.CurvilinearFinalizeOrthogonalize(id));

                    // Assert

                    int gridOut = api.CurvilinearGridGetData(id, out curvilinearGrid);
                    Assert.NotNull(gridOut);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearSmoothing(id, 10, 10.0, 20.0, 30.0, 20.0));

                    // Assert

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearSmoothingDirectional(id,
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

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.AreEqual(5, curvilinearGrid.NumM);
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    Assert.AreEqual(0, api.CurvilinearRefine(id, 10.0, 20.0, 20.0, 20.0, 10));

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    Assert.AreEqual((4, 13), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    var orthogonalizationParameters = OrthogonalizationParameters.CreateDefault();
                    Assert.AreEqual(0, api.CurvilinearInitializeOrthogonalize(id, orthogonalizationParameters));
                    Assert.AreEqual(0, api.CurvilinearSetBlockOrthogonalize(id, 0.0, 0.0, 30.0, 30.0), 0);
                    Assert.AreEqual(0, api.CurvilinearFinalizeOrthogonalize(id));
                    // Assert

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                    Assert.AreEqual((5, 5), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));

                    // Execute
                    Assert.AreEqual(0, api.CurvilinearMoveNode(id, 0.0, 0.0, -10.0, 0.0));
                    // Assert
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    Assert.AreEqual((5, 5), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearComputeRectangularGridOnExtension(id, makeGridParameters));
                    var grid = new DisposableCurvilinearGrid();
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out grid));
                    Assert.AreEqual((11, 6), (grid.NumM, grid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearLineMirror(id, 1.2, 0.0, 0.0, 0.0, 50.0));
                    // Assert
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    Assert.AreEqual((5, 6), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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

                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));

                    Assert.AreEqual(0, api.CurvilinearLineAttractionRepulsion(id,
                                                                              0.5,
                                                                              30.0, 0.0, 30.0, 50.0,
                                                                              10.0, 20.0, 50.0, 20.0));

                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    // Assert
                    Assert.AreEqual((5, 5), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                    Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                    // Execute
                    Assert.AreEqual(0, api.CurvilinearInsertFace(id, -5.0, 5.0));
                    // Assert
                    Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));

                    Assert.AreEqual((5, 6), (curvilinearGrid.NumM, curvilinearGrid.NumN));
                }
                finally
                {
                    api.DeallocateState(id);
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
                        Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                        Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                        var curvature = new double[curvilinearGrid.NumM * curvilinearGrid.NumN];

                        // Execute
                        Assert.AreEqual(0, api.CurvilinearComputeCurvature(id, CurvilinearDirectionOptions.M, ref curvature));

                        // Assert
                        var tolerance = 1e-9;
                        Assert.AreEqual(-999.0, curvature[0], tolerance);
                        Assert.AreEqual(1.0, curvature[1], tolerance);
                        Assert.AreEqual(1.0, curvature[2], tolerance);
                        Assert.AreEqual(1.0, curvature[3], tolerance);
                        Assert.AreEqual(-999.0, curvature[4], tolerance);
                        Assert.AreEqual(-999.0, curvature[5], tolerance);
                        Assert.AreEqual(1.0, curvature[6], tolerance);
                        Assert.AreEqual(1.0, curvature[7], tolerance);
                        Assert.AreEqual(1.0, curvature[8], tolerance);
                        Assert.AreEqual(-999.0, curvature[9], tolerance);
                        Assert.AreEqual(-999.0, curvature[10], tolerance);
                    }
                    finally
                    {
                        api.DeallocateState(id);
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
                        Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                        Assert.AreEqual(0, api.CurvilinearGridGetData(id, out curvilinearGrid));
                        var smoothness = new double[curvilinearGrid.NumM * curvilinearGrid.NumN];

                        // Execute
                        Assert.AreEqual(0, api.CurvilinearComputeSmoothness(id, CurvilinearDirectionOptions.M, ref smoothness));

                        // Assert
                        var tolerance = 1e-9;
                        Assert.AreEqual(-999.0, smoothness[0], tolerance);
                        Assert.AreEqual(1.0, smoothness[1], tolerance);
                        Assert.AreEqual(-999.0, smoothness[5], tolerance);
                        Assert.AreEqual(1.0, smoothness[6], tolerance);
                        Assert.AreEqual(1.0, smoothness[7], tolerance);
                    }
                    finally
                    {
                        api.DeallocateState(id);
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
                Assert.AreEqual((2, 3), (grid.NumM, grid.NumN));
                for (int row = 0; row < grid.NumM; ++row)
                {
                    for (int column = 0; column < grid.NumN; ++column)
                    {
                        int index = row * grid.NumN + column;
                        Assert.AreEqual(grid.GetNodeX(index), grid.NodeX[index]);
                        Assert.AreEqual(grid.GetNodeY(index), grid.NodeY[index]);
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
                Assert.AreEqual((5, 4), (grid.NumN, grid.NumM));
                Assert.AreEqual((firstIndex, secondIndex), (grid.GetFirstNode(edgeIndex), grid.GetLastNode(edgeIndex)));
            }
        }

        [TestCase(5, 3, 5 * 3)]
        [TestCase(15, 42, 15 * 42)]
        public void CurvilinearGrid_GetNodeCount_CountsAllNodes(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.AreEqual((n, m), (grid.NumN, grid.NumM));
                Assert.AreEqual(count, grid.NodeCount());
            }
        }

        [TestCase(5, 3, 4 * 3 + 5 * 2)]
        [TestCase(15, 42, 15 * 41 + 14 * 42)]
        public void CurvilinearGrid_GetEdgeCount_CountsRowOrientedAndColumnOrientedEdges(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.AreEqual((n, m), (grid.NumN, grid.NumM));
                Assert.AreEqual(count, grid.EdgeCount());
            }
        }

        [TestCase(5, 3, (5 - 1) * (3 - 1))]
        [TestCase(15, 42, (15 - 1) * (42 - 1))]
        public void CurvilinearGrid_GetCellCount_CountsAllCells(int m, int n, int count)
        {
            using (var grid = CreateCurvilinearGrid(n, m, 5.0, 2.0))
            {
                Assert.AreEqual((n, m), (grid.NumN, grid.NumM));
                Assert.AreEqual(count, grid.CellCount());
            }
        }

        [TestCaseSource(nameof(EdgeNodesSerialization))]
        public void CurvilinearGrid_EdgeNodesSerialization(int numM, int numN, IList<(int, int)> edgeNodes)
        {
            using (var grid = CreateCurvilinearGrid(numN, numM, 5.0, 2.0))
            {
                Assert.AreEqual((numN, numM), (grid.NumN, grid.NumM));
                for (int i = 0; i < grid.EdgeCount(); ++i)
                {
                    Assert.AreEqual(edgeNodes[i], (grid.GetFirstNode(i), grid.GetLastNode(i)));
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
                        Assert.AreEqual(0, api.CurvilinearSet(id, grid));
                        Assert.AreEqual(0, api.CurvilinearConvertToMesh2D(id));
                        Assert.AreEqual(0, api.Mesh2dGetData(id, out mesh2d));

                        // Assert
                        Assert.AreEqual(25, mesh2d.NumNodes);
                    }
                    finally
                    {
                        api.DeallocateState(id);
                        curvilinearGrid?.Dispose();
                        mesh2d.Dispose();
                    }
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

                    Assert.AreEqual(0, api.CurvilinearComputeRectangularGrid(id, makeGridParameters));
                    Assert.AreEqual(0, api.CurvilinearGetBoundariesAsPolygons(id, 0, 0, 3, 3, out DisposableGeometryList boundaryPolygons));

                    //var expectedPolygonXCoordinates = new []{1,2,3};
                    //var expectedPolygonYCoordinates = new[] { 1, 2, 3 };

                    //Assert.That(boundaryPolygons.XCoordinates, Is.EquivalentTo(expectedPolygonXCoordinates));
                    //Assert.That(boundaryPolygons.YCoordinates, Is.EquivalentTo(expectedPolygonYCoordinates));

                    boundaryPolygons.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
            api.DeallocateState(id);
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
        
    }
}
