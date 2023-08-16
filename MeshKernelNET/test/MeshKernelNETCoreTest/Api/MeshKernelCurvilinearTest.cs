using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
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

        [Test]
        public void CurvilinearMakeUniformThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var disposableGeometryList = new DisposableGeometryList())
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
                    makeGridParameters.UpperRightCornerXCoordinate = 0.0;
                    makeGridParameters.UpperRightCornerYCoordinate = 0.0;

                    Assert.IsTrue(api.CurvilinearMakeUniform(id, makeGridParameters, disposableGeometryList));

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
        public void CurvilinearComputeTransfiniteFromSplinesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    
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
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    ;
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


                    Assert.IsTrue(api.CurvilinearComputeOrthogonalGridFromSplines(id, geometryListIn,
                         curvilinearParameters, splinesToCurvilinearParameters));

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
        public void CurvilinearComputeTransfiniteFromPolygonThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using(var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
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

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
            using( var api = new MeshKernelApi())
            using( var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
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

                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));
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
                                                                       curvilinearParameters,
                                                                       splinesToCurvilinearParameters);

                    int numLayers = 3;
                    for (int i = 1; i < numLayers; i++)
                    {
                        Assert.IsTrue(api.CurvilinearIterateOrthogonalGridFromSplines(id, i));
                        Assert.IsTrue(api.CurvilinearRefreshOrthogonalGridFromSplines(id));
                    }

                    Assert.IsTrue(api.CurvilinearDeleteOrthogonalGridFromSplines(id));
                    
                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

                    Assert.AreEqual(curvilinearGrid.NumM, 3);
                    Assert.AreEqual(curvilinearGrid.NumN, 5);

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
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
        public void CurvilinearOrthogonalizeWithFrozenLineThroughAPI()
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
                    Assert.IsTrue(api.CurvilinearInitializeOrthogonalize(id, orthogonalizationParameters));
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
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));

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
        public void CurvilinearRefineThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var grid = CreateCurvilinearGrid(4, 4, 10, 10)) 
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    Assert.IsTrue(api.CurvilinearSet(id, grid));
        
                    Assert.IsTrue(api.CurvilinearRefine(id, 10.0, 20.0, 20.0, 20.0, 10));
        
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.IsTrue(api.CurvilinearGridGetData(id, out curvilinearGrid));
        
                    Assert.AreEqual(curvilinearGrid.NumN, 4); 
                    Assert.AreEqual(curvilinearGrid.NumM, 13);
                    
                    curvilinearGrid.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
                }
            }
        }
        
        [Test]
        public void CurvilinearOrthogonalizeOnBlockThroughAPI()
        {
            // Setup
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
                    Assert.IsTrue(api.CurvilinearInitializeOrthogonalize(id, orthogonalizationParameters)); 
                    Assert.IsTrue(api.CurvilinearSetBlockOrthogonalize(id, 0.0, 0.0, 30.0, 30.0));
                    Assert.IsTrue(api.CurvilinearFinalizeOrthogonalize(id));
        
                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));
        
                    Assert.AreEqual(curvilinearGrid.NumN, 5);
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
        public void CurvilinearMoveNodeThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);
                    
                    Assert.IsTrue(api.CurvilinearSet(id, grid));
        
                    // Execute
                    Assert.IsTrue(api.CurvilinearMoveNode(id, 0.0, 0.0, -10.0, 0.0));
        
                    // Assert
                    var curvilinearGrid = new DisposableCurvilinearGrid();
                    Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));
        
                    Assert.AreEqual(curvilinearGrid.NumN, 5);
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
        public void CurvilinearMakeUniformOnExtensionThroughAPI()
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
        
                    Assert.IsTrue(api.CurvilinearMakeUniformOnExtension(id, makeGridParameters));
        
                    var grid = new DisposableCurvilinearGrid();
                    var success = api.CurvilinearGridGetData(id, out grid);
                    Assert.IsTrue(success);
        
                    Assert.AreEqual(6, grid.NumM);
                    Assert.AreEqual(11, grid.NumN);
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
             using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
             {
                 var id = 0;
                 try
                 {
                     id = api.AllocateState(0);
        
                     Assert.IsTrue(api.CurvilinearSet(id, grid));
        
                     // Execute
                     Assert.IsTrue(api.CurvilinearLineMirror(id, 1.2, 0.0, 0.0, 0.0, 50.0));
        
                     // Assert
                     var curvilinearGrid = new DisposableCurvilinearGrid();
                     Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));
        
                     Assert.AreEqual(curvilinearGrid.NumN, 5);
                     Assert.AreEqual(curvilinearGrid.NumM, 6);
        
                     curvilinearGrid.Dispose();
                 }
                 finally
                 {
                     api.DeallocateState(id);
                 }
             }
        }

        [Test]
        public void CurvilinearLineAttractionRepulsionThroughAPI()
        {
             // Setup
             using (var api = new MeshKernelApi())
             using(var grid = CreateCurvilinearGrid(5, 5, 10, 10))
             {
                 var id = 0;
                 try
                 {
                     id = api.AllocateState(0);
                     
                     Assert.IsTrue(api.CurvilinearSet(id, grid));
                     Assert.IsTrue(api.CurvilinearLineAttractionRepulsion(id, 
                                                                          0.5,
                                                                          30.0, 0.0, 30.0, 50.0,
                                                                          10.0, 20.0, 50.0, 20.0));

                     var curvilinearGrid = new DisposableCurvilinearGrid();
                     Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));

                     // Assert
                     Assert.AreEqual(curvilinearGrid.NumN, 5);
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
        public void CurvilinearInsertFaceThroughAPI()
        {
              // Setup
             using (var api = new MeshKernelApi())
             using (var grid = CreateCurvilinearGrid(5, 5, 10, 10))
             {
                 var id = 0;
                 try
                 {
                     id = api.AllocateState(0);
                     Assert.IsTrue(api.CurvilinearSet(id, grid));

                     // Execute
                     Assert.IsTrue(api.CurvilinearInsertFace(id, -5.0, 5.0));

                     // Assert
                     var curvilinearGrid = new DisposableCurvilinearGrid();
                     Assert.True(api.CurvilinearGridGetData(id, out curvilinearGrid));

                     Assert.AreEqual(curvilinearGrid.NumN, 5);
                     Assert.AreEqual(curvilinearGrid.NumM, 6);
                     curvilinearGrid.Dispose();
                 }
                 finally
                 {
                     api.DeallocateState(id);
                 }
             }
        }
    }

}
