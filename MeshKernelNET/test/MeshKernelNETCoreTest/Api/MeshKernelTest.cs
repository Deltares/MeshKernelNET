namespace MeshKernelNETCoreTest.Api
{
    /*[TestFixture]
    [Category(GridEditorTestCategories.API)]
    public class MeshKernelTest
    {

        [Test]
        [Category(TestCategory.Performance)]
        [TestCase(false)]
        [TestCase(true)]
        public void DeleteNodeThroughApiPerformanceTrace(bool useRemoting)
        {
            var stopWatch = new Stopwatch();

            UnstructuredGrid grid = null;
            GetTiming(stopWatch, "Generate grid", () =>
             {
                 grid = UnstructuredGridTestHelper.GenerateRegularGrid(100, 100, 100, 200, 0, 0);
             });

            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"Number of vertices: {grid.Vertices.Count}");
            Console.WriteLine($"Number of edges: {grid.Edges.Count}");
            Console.WriteLine($"Number of cells: {grid.Cells.Count}");
            Console.WriteLine(new string('-', 100));

            var numberOfVerticesBefore = grid.Vertices.Count;

            var id = 0;
            IMeshKernelApi api = null;

            GetTiming(stopWatch, "Create api", () =>
            {
                api = useRemoting
                    ? (IMeshKernelApi)new MeshKernelApiRemote()
                    : new MeshKernelApi();
            });

            try
            {
                using (var mesh = new DisposableMeshGeometry(grid))
                {
                    GetTiming(stopWatch, "Create grid state", () =>
                    {
                        id = api.AllocateState();
                        Assert.AreEqual(0, id);
                    });

                    GetTiming(stopWatch, "Set state", () =>
                    {
                        Assert.IsTrue(api.SetGridState(id, mesh, false));
                    });

                    GetTiming(stopWatch, "Delete node", () =>
                    {
                        Assert.IsTrue(api.DeleteVertex(id, 0));
                    });

                    GetTiming(stopWatch, "Get mesh state", () =>
                    {
                        var newMeshGeometry = api.GetGridState(id);
                        var count = newMeshGeometry.xNodes.Length;

                        Assert.AreEqual(numberOfVerticesBefore - 1, count);
                        Assert.NotNull(newMeshGeometry);
                    });
                }
            }
            finally
            {
                GetTiming(stopWatch, "Remove state", () =>
                {
                    api.DeallocateState(id);
                });

                if (useRemoting)
                {
                    GetTiming(stopWatch, "Remove instance", () => { RemoteInstanceContainer.RemoveInstance(api); });
                }

                api.Dispose();
            }
        }

        private static void GetTiming(Stopwatch stopwatch, string actionName, Action action)
        {
            stopwatch.Restart();

            action();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed} -- {actionName}");
        }

        [Test]
        public void DeleteNodeThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfVerticesBefore = grid.Vertices.Count;

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



            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    Assert.IsTrue(api.DeleteVertex(id, 0));

                    var newMeshGeometry = api.GetGridState(id);
                    var count = newMeshGeometry.xNodes.Length;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfVerticesBefore - 1, count);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void FlipEdgesThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfEdgesBefore = grid.Edges.Count;

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



            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = -1;
                try

                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));
                    Assert.IsTrue(api.Mesh2dFlipEdges(id, true, ProjectToLandBoundaryOptions.ToOriginalNetBoundary));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 9, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void InsertEdgeThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfEdgesBefore = grid.Edges.Count;

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


            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {


                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    int newEdgeIndex = 0;
                    Assert.IsTrue(api.Mesh2dInsertEdge(id, 4, 1, ref newEdgeIndex));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore + 1, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void MergeTwoVerticesThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfEdgesBefore = grid.Edges.Count;

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


            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    Assert.IsTrue(api.Mesh2dMergeTwoNodes(id, 0, 4));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore - 1, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void MergeVerticesThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfEdgesBefore = grid.Edges.Count;

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


            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    var geometryList = new DisposableGeometryList();
                    Assert.IsTrue(api.Mesh2dMergeNodes(id, geometryList));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void OrthogonalizationThroughApi()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            var numberOfEdgesBefore = grid.Edges.Count;

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


            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    var orthogonalizationParametersList = OrthogonalizationParameters.CreateDefault();
                    var polygon = new DisposableGeometryList();
                    var landBoundaries = new DisposableGeometryList();
                    Assert.IsTrue(api.Mesh2dInitializeOrthogonalization(id, ProjectToLandBoundaryOptions.ToOriginalNetBoundary, orthogonalizationParametersList, polygon, landBoundaries));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void MakeGridThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200);

            var numberOfEdgesBefore = grid.Edges.Count;

            // Before                                          After (2nd grid lies on top of the first)
            // 0 ------- 1 ------- 2 ------- 3                16 --------17 -------18 -------19
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 4 ------- 5 ------- 6 ------- 7                20 --------21 -------22 -------23
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // 8 ------- 9 ------ 10 ------ 11                24 --------25 ------ 26 ------ 27 
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |
            // |         |         |         |                 |         |         |         |   
            // |         |         |         |                 |         |         |         |
            //12 ------ 13 ------ 14 ------ 15                28 ------ 29 ------- 30 ------ 31


            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                    var count = newMeshGeometry.numberOfEdges;

                    Assert.AreNotEqual(2, newMeshGeometry.numberOfEdges);
                    Assert.AreEqual(numberOfEdgesBefore * 2, count);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }



        }


        [Test]
        public void GetSplinesThroughAPI()
        {
            using (var geometryListIn = new DisposableGeometryList())
            using (var api = new MeshKernelApiRemote())
            {
                try
                {
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 3;
                    geometryListIn.XCoordinates = new double[] { 10.0, 20.0, 30.0 };
                    geometryListIn.YCoordinates = new double[] { -5.0, 5.0, -5.0 };
                    geometryListIn.ZCoordinates = new double[] { 0.0, 0.0, 0.0 };

                    var geometryListOut = new DisposableGeometryList();
                    int numberOfPointsBetweenVertices = 20;
                    geometryListOut.GeometrySeparator = geometrySeparator;
                    geometryListOut.NumberOfCoordinates = 60;
                    geometryListOut.XCoordinates = new double[60];
                    geometryListOut.YCoordinates = new double[60];
                    geometryListOut.ZCoordinates = new double[60];

                    Assert.IsTrue(api.GetSplines(geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices));
                }
                finally
                {
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void GenerateCurvilinearGridThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200);


            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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
                    Assert.IsTrue(api.MakeGridFromSplines(id, geometryListIn, curvilinearParameters));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void GenerateOrthogonalCurvilinearGridThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200);

            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void GenerateTriangularGridThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200);

            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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

                    Assert.IsTrue(api.MakeTriangularGridInPolygon(id, ref geometryListIn));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void GenerateTriangularGridFromSamplesThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200);

            using (var mesh = new DisposableMeshGeometry(grid))
            using (var api = new MeshKernelApiRemote())
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    Assert.IsTrue(api.Mesh2dMakeMeshFromSamples(id, ref geometryListIn));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void GetMeshBoundariesThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);


            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    int numberOfPolygonVertices = -1;
                    Assert.IsTrue(api.Mesh2dCountMeshBoundariesAsPolygons(id, ref numberOfPolygonVertices));
                    Assert.AreEqual(13, numberOfPolygonVertices);

                    var geometryListIn = new DisposableGeometryList();
                    geometryListIn.XCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.YCoordinates = new double[numberOfPolygonVertices];
                    geometryListIn.ZCoordinates = new double[numberOfPolygonVertices];
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = numberOfPolygonVertices;

                    Assert.IsTrue(api.Mesh2dGetMeshBoundariesAsPolygons(id, ref geometryListIn));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void OffsetAPolygonThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200, 0, 0);

            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    var geometryListIn = new DisposableGeometryList();
                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;
                    geometryListIn.NumberOfCoordinates = 16;

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

                    geometryListIn.ZCoordinates = new[]
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
                    Assert.AreEqual(16, numberOfPolygonVertices);

                    var disposableGeometryListOut = new DisposableGeometryList();
                    bool success = api.PolygonGetOffset(id, ref geometryListIn, innerOffsetedPolygon, distance,
                        ref disposableGeometryListOut);
                    Assert.IsTrue(success);

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void RefineAPolygonThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200, 0, 0);

            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {


                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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
                    geometryListOut.ZCoordinates = new double[numberOfPolygonVertices];

                    Assert.IsTrue(api.PolygonRefine(id, ref geometryListIn, firstIndex,
                        secondIndex, distance, ref geometryListOut));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void RefineAGridBasedOnSamplesThroughAPI()
        {
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(3, 3, 100, 200, 0, 0);

            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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

                    var interpolationParameters = InterpolationParameters.CreateDefault();
                    var samplesRefineParameters = SamplesRefineParameters.CreateDefault();
                    Assert.IsTrue(api.Mesh2dRefineBasedOnSamples(id, ref geometryListIn, interpolationParameters,
                        samplesRefineParameters));

                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void RefineAGridBasedOnPolygonThroughAPI()
        {
            //Setup
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(10, 10, 100, 100, 0, 0);

            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {
                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
                    {
                            0.0,
                            0.0,
                            0.0,
                            0.0,
                            0.0
                        };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    //Call
                    var interpolationParameters = InterpolationParameters.CreateDefault();
                    Assert.IsTrue(api.Mesh2dRefineBasedOnPolygon(id, ref geometryListIn, interpolationParameters));

                    //Assert
                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void MakeCurvilinearGridFromPolygonThroughAPI()
        {
            // Setup
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200, 0, 0);

            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {


                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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

                    var newMeshGeometry = api.GetGridState(id);

                    // Assert a valid mesh is produced
                    Assert.NotNull(newMeshGeometry);
                    Assert.AreEqual(newMeshGeometry.numberOfEdges, 12);
                    Assert.AreEqual(newMeshGeometry.numberOfNodes, 9);

                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }

        [Test]
        public void MakeCurvilinearGridFromTriangleThroughAPI()
        {
            //Setup
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(0, 0, 100, 200, 0, 0);


            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            {
                var id = 0;
                try
                {

                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

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

                    geometryListIn.ZCoordinates = new[]
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

                    var newMeshGeometry = api.GetGridState(id);

                    // Assert a valid mesh is produced
                    Assert.NotNull(newMeshGeometry);
                    Assert.AreEqual(newMeshGeometry.numberOfEdges, 23);
                    Assert.AreEqual(newMeshGeometry.numberOfNodes, 16);
                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }


        [Test]
        public void GetClosestMeshCoordinateThroughAPI()
        {
            //Setup
            var grid = UnstructuredGridTestHelper.GenerateRegularGrid(10, 10, 100, 100, 0, 0);


            using (var api = new MeshKernelApiRemote())
            using (var mesh = new DisposableMeshGeometry(grid))
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                try
                {


                    id = api.AllocateState();
                    Assert.AreEqual(0, id);

                    Assert.IsTrue(api.SetGridState(id, mesh, false));

                    var geometrySeparator = api.GetSeparator();
                    geometryListIn.GeometrySeparator = geometrySeparator;

                    geometryListIn.XCoordinates = new[] { -5.0 };
                    geometryListIn.YCoordinates = new[] { -5.0 };
                    geometryListIn.ZCoordinates = new[] { 0.0 };

                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    var geometryListOut = new DisposableGeometryList();
                    geometryListOut.XCoordinates = new[] { geometrySeparator };
                    geometryListOut.YCoordinates = new[] { geometrySeparator };
                    geometryListOut.ZCoordinates = new[] { geometrySeparator };
                    geometryListOut.NumberOfCoordinates = 1;

                    //Call
                    Assert.IsTrue(api.GetVertexCoordinates(id, geometryListIn, 10.0, ref geometryListOut));

                    //Assert
                    Assert.LessOrEqual(geometryListOut.XCoordinates[0], 1e-6);
                    Assert.LessOrEqual(geometryListOut.YCoordinates[0], 1e-6);
                    var newMeshGeometry = api.GetGridState(id);
                    Assert.NotNull(newMeshGeometry);
                }
                finally
                {
                    api.DeallocateState(id);
                    RemoteInstanceContainer.RemoveInstance(api);
                }
            }
        }
    }*/
}
