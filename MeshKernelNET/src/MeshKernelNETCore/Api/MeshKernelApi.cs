using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Helpers;
using MeshKernelNETCore.Native;

namespace MeshKernelNETCore.Api
{
    [ExcludeFromCodeCoverage]
    // Excluded because it is tested through the MeshKernelApiRemote
    // DotCover on the buildserver does not work correctly with remoting
    public sealed class MeshKernelApi : IMeshKernelApi
    {
        /// <inheritdoc />
        public int AllocateState(int projectionType)
        {
            var meshKernelId = 0;
            MeshKernelDll.AllocateState(projectionType, ref meshKernelId);
            return meshKernelId;
        }

        /// <inheritdoc />
        public bool DeallocateState(int meshKernelId)
        {
            return MeshKernelDll.DeallocateState(meshKernelId) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dSetState(int meshKernelId, DisposableMesh2D disposableMesh2D)
        {
            var mesh2D = disposableMesh2D.CreateMesh2D();

            var result = MeshKernelDll.Mesh2dSetState(meshKernelId, ref mesh2D);

            return result == 0;
        }

        /// <inheritdoc />
        public DisposableMesh2D Mesh2DGetDimensions(int meshKernelId)
        {
            var newMesh2D = new Mesh2D();

            MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);
            var disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes, newMesh2D.num_edges, newMesh2D.num_faces, newMesh2D.num_face_nodes);
            newMesh2D = disposableMesh2D.CreateMesh2D();

            return CreateDisposableMesh2D(newMesh2D);
        }

        public DisposableMesh2D Mesh2dGetData(int meshKernelId)
        {
            var newMesh2D = new Mesh2D();

            MeshKernelDll.Mesh2dGetData(meshKernelId, ref newMesh2D);

            return CreateDisposableMesh2D(newMesh2D, true);
        }

        /// <inheritdoc />
        public bool Mesh2dDeleteNode(int meshKernelId, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dDeleteNode(meshKernelId, vertexIndex) == 0;
        }

        public bool Mesh2dFlipEdges(int meshKernelId, bool isTriangulationRequired, ProjectToLandBoundaryOptions projectToLandBoundaryOption, DisposableGeometryList selectingPolygon, DisposableGeometryList landBoundaries)
        {
            int isTriangulationRequiredInt = isTriangulationRequired ? 1 : 0;
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            var geometryListPolygon = selectingPolygon?.CreateGeometryListNative() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = landBoundaries?.CreateGeometryListNative() ??
                                             new GeometryListNative { numberOfCoordinates = 0 };
            return MeshKernelDll.Mesh2dFlipEdges(meshKernelId, isTriangulationRequiredInt, projectToLandBoundaryOptionInt, ref geometryListPolygon, ref geometryListLandBoundaries) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dInsertEdge(meshKernelId, startVertexIndex, endVertexIndex, ref edgeIndex) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dMergeTwoNodes(int meshKernelId, int startVertexIndex, int endVertexIndex)
        {
            return MeshKernelDll.Mesh2dMergeTwoNodes(meshKernelId, startVertexIndex, endVertexIndex) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dMergeNodes(int meshKernelId, DisposableGeometryList disposableGeometryList)
        {
            var geometryList = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dMergeNodes(meshKernelId, ref geometryList) == 0;
        }

        public bool Mesh2dInitializeOrthogonalization(int meshKernelId,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption,
            OrthogonalizationParameters orthogonalizationParameters, DisposableGeometryList geometryListNativePolygon,
            DisposableGeometryList geometryListNativeLandBoundaries)
        {
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;

            var nativeOrthogonalizationParameters = orthogonalizationParameters.ToOrthogonalizationParametersNative();

            var geometryListPolygon = geometryListNativePolygon?.CreateGeometryListNative() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = geometryListNativeLandBoundaries?.CreateGeometryListNative() ??
                                             new GeometryListNative { numberOfCoordinates = 0 };

            return MeshKernelDll.Mesh2dInitializeOrthogonalization(meshKernelId, projectToLandBoundaryOptionInt,
                       ref nativeOrthogonalizationParameters, ref geometryListPolygon, ref geometryListLandBoundaries) == 0;
        }

        public bool Mesh2dPrepareOuterIterationOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dPrepareOuterIterationOrthogonalization(meshKernelId) == 0;
        }

        public bool Mesh2dComputeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dComputeInnerOrtogonalizationIteration(meshKernelId) == 0;
        }

        public bool Mesh2dFinalizeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dFinalizeInnerOrtogonalizationIteration(meshKernelId) == 0;
        }

        public bool Mesh2dDeleteOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteOrthogonalization(meshKernelId) == 0;
        }

        public bool CurvilinearMakeUniform(int meshKernelId, MakeGridParameters makeGridParameters, DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateGeometryListNative();

            return MeshKernelDll.CurvilinearMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool GetSplines(DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var geometryListIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListOut = disposableGeometryListOut.CreateGeometryListNative();
            if (MeshKernelDll.GetSplines(ref geometryListIn, ref geometryListOut,
                     numberOfPointsBetweenVertices) != 0)
            {
                return false;
            }
            disposableGeometryListOut.NumberOfCoordinates = geometryListOut.numberOfCoordinates;
            return true;
        }

        public bool CurvilinearComputeTransfiniteFromSplines(int meshKernelId, DisposableGeometryList disposableGeometryListIn,
             CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateGeometryListNative();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
             ref CurvilinearParameters curvilinearParameters, ref SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateGeometryListNative();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                        ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool Mesh2dMakeMeshFromPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dMakeMeshFromPolygon(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeMeshFromSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dMakeMeshFromSamples(meshKernelId, ref geometryListNative) == 0;
        }

        public bool CurvilinearGetDimensions(int meshKernelId, ref CurvilinearGrid curvilinearGrid)
        {
            return MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid) == 0;
        }

        public bool CurvilinearGetData(int meshKernelId, ref CurvilinearGrid curvilinearGrid)
        {
            return MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid) == 0;
        }

        public bool Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices)
        {
            return MeshKernelDll.Mesh2dCountMeshBoundariesAsPolygons(meshKernelId, ref numberOfPolygonVertices) == 0;
        }

        public bool Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref geometryListNative) == 0;
        }

        public bool PolygonCountOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonGetOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListNativeOut = disposableGeometryListOut.CreateGeometryListNative();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut) == 0;
        }

        public bool PolygonCountRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            return MeshKernelDll.PolygonCountRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            var geometryListNativeOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.PolygonRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut) == 0;
        }

        public bool Mesh2dRefineBasedOnSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, InterpolationParameters interpolationParameters, SamplesRefineParameters samplesRefineParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            var interpolationParametersNative = interpolationParameters.ToInterpolationParametersNative();
            var samplesRefineParametersNative = samplesRefineParameters.ToSampleRefineParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, ref interpolationParametersNative, ref samplesRefineParametersNative) == 0;
        }

        public bool Mesh2dRefineBasedOnPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, InterpolationParameters interpolationParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateGeometryListNative();
            var interpolationParametersNative = interpolationParameters.ToInterpolationParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref interpolationParametersNative) == 0;
        }

        public int[] GetSelectedVerticesInPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int inside)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateGeometryListNative();
            int numberOfMeshVertices = -1;
            MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            IntPtr selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref selectedVerticesPtr);
            int[] selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            return selectedVertices;
        }

        public bool Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dGetOrthogonality(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dGetSmoothness(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool Mesh2dInsertNode(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dInsertNode(meshKernelId, xCoordinate, yCoordinate, ref vertexIndex) == 0;
        }

        public bool Mesh2dGetNodeIndex(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius, ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dGetNodeIndex(meshKernelId, xCoordinateIn, yCoordinateIn, searchRadius, ref vertexIndex) == 0;
        }
        public bool Mesh2dGetClosestNode(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius, ref double xCoordinateOut, ref double yCoordinateOut)
        {
            return MeshKernelDll.Mesh2dGetClosestNode(meshKernelId, xCoordinateIn, yCoordinateIn, searchRadius, ref xCoordinateOut, ref yCoordinateOut) == 0;
        }

        public bool Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate)
        {
            return MeshKernelDll.Mesh2dDeleteEdge(meshKernelId, ref xCoordinate, ref yCoordinate) == 0;
        }

        public bool Mesh2dGetEdge(int meshKernelId, double xCoordinate, double yCoordinate, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dGetEdge(meshKernelId, ref xCoordinate, ref yCoordinate, ref edgeIndex) == 0;
        }

        public bool Mesh2dDelete(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut, int deletionOption, bool invertDeletion)
        {
            var geometryListNativeIn = disposableGeometryListOut.CreateGeometryListNative();
            return MeshKernelDll.Mesh2dDelete(meshKernelId, ref geometryListNativeIn, deletionOption, invertDeletion) == 0;
        }

        public bool Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dMoveNode(meshKernelId, ref xCoordinate, ref yCoordinate, ref vertexIndex) == 0;
        }

        public bool GetPointsInPolygon(int meshKernelId, ref DisposableGeometryList inputPolygon, ref DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateGeometryListNative();
            var geometryListNativeinputPoints = inputPoints.CreateGeometryListNative();
            var geometryListNativeSelectedPoints = selectedPoints.CreateGeometryListNative();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, DisposableGeometryList geometryList, int firstNode,
            int secondNode, int thirdNode, bool useFourthSide)
        {

            var geometryListNative = geometryList.CreateGeometryListNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromPolygon(meshKernelId,
                                                                 ref geometryListNative,
                                                                 firstNode,
                                                                 secondNode,
                                                                 thirdNode,
                                                                 useFourthSide) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromTriangle(int meshKernelId, DisposableGeometryList geometryList, int firstNode,
            int secondNode, int thirdNode)
        {
            var geometryListNative = geometryList.CreateGeometryListNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromTriangle(meshKernelId,
                                                                  ref geometryListNative,
                                                                  firstNode,
                                                                  secondNode,
                                                                  thirdNode) == 0;
        }

        public bool CurvilinearConvertToMesh2D(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearConvertToMesh2D(meshKernelId) == 0;
        }

        public double GetSeparator()
        {
            // To activate after update of the MeshKernel nuget
            return MeshKernelDll.GetSeparator();
        }

        public double GetInnerOuterSeparator()
        {
            // To activate after update of the MeshKernel nuget
            return MeshKernelDll.GetInnerOuterSeparator();
        }

        public void Dispose()
        {
            // Do nothing because no remoting is used
        }

        /// <inheritdoc />
        public DisposableCurvilinearGrid CurvilinearGridGetDimensions(int meshKernelId)
        {
            var curvilinearGrid = new CurvilinearGrid();

            MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid);
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_nodes, curvilinearGrid.num_edges);
            curvilinearGrid = disposableCurvilinearGrid.CreateCurvilinearGrid();

            return CreateDisposableCurvilinearGrid(curvilinearGrid);
        }

        public DisposableCurvilinearGrid CurvilinearGridGetData(int meshKernelId)
        {
            var curvilinearGrid = new CurvilinearGrid();

            MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid);

            return CreateDisposableCurvilinearGrid(curvilinearGrid);
        }

        private DisposableMesh2D CreateDisposableMesh2D(Mesh2D newMesh2D, bool addCellInformation = false)
        {
            var disposableMesh2D = new DisposableMesh2D
            {
                nodeX = newMesh2D.node_x.CreateValueArray<double>(newMesh2D.num_nodes),
                nodeY = newMesh2D.node_y.CreateValueArray<double>(newMesh2D.num_nodes),
                edgeNodes = newMesh2D.edge_nodes.CreateValueArray<int>(newMesh2D.num_edges * 2).ToArray(),
                numEdges = newMesh2D.num_edges,
                numNodes = newMesh2D.num_nodes
            };

            if (addCellInformation && newMesh2D.num_faces > 0)
            {
                disposableMesh2D.numFaces = newMesh2D.num_faces;
                disposableMesh2D.nodesPerFace = newMesh2D.nodes_per_face.CreateValueArray<int>(newMesh2D.num_faces); ;
                disposableMesh2D.faceNodes = newMesh2D.face_nodes.CreateValueArray<int>(newMesh2D.num_faces);
                disposableMesh2D.faceX = newMesh2D.face_x.CreateValueArray<double>(newMesh2D.num_faces);
                disposableMesh2D.faceY = newMesh2D.face_y.CreateValueArray<double>(newMesh2D.num_faces);
            }

            return disposableMesh2D;
        }

        private DisposableCurvilinearGrid CreateDisposableCurvilinearGrid(CurvilinearGrid curvilinearGrid)
        {
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid
            {
                nodeX = curvilinearGrid.node_x.CreateValueArray<double>(curvilinearGrid.num_nodes),
                nodeY = curvilinearGrid.node_y.CreateValueArray<double>(curvilinearGrid.num_nodes),
                edgeNodes = curvilinearGrid.edge_nodes.CreateValueArray<int>(curvilinearGrid.num_edges * 2).ToArray(),
                numEdges = curvilinearGrid.num_edges,
                numNodes = curvilinearGrid.num_nodes
            };

            return disposableCurvilinearGrid;
        }
    }
}