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
        public bool Mesh2dSet(int meshKernelId, DisposableMesh2D disposableMesh2D)
        {
            var mesh2D = disposableMesh2D.CreateNativeObject();

            var result = MeshKernelDll.Mesh2dSet(meshKernelId, ref mesh2D);

            return result == 0;
        }

        /// <inheritdoc />
        public DisposableMesh2D Mesh2DGetDimensions(int meshKernelId)
        {
            var newMesh2D = new Mesh2DNative();

            MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);
            var disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes, newMesh2D.num_edges, newMesh2D.num_faces, newMesh2D.num_face_nodes);

            return disposableMesh2D;
        }

        public DisposableMesh2D Mesh2dGetData(int meshKernelId)
        {
            var newMesh2D = new Mesh2DNative();

            MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);
            var disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes, newMesh2D.num_edges, newMesh2D.num_faces, newMesh2D.num_face_nodes);
            newMesh2D = disposableMesh2D.CreateNativeObject();

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
            var geometryListPolygon = selectingPolygon?.CreateNativeObject() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = landBoundaries?.CreateNativeObject() ??
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
        public bool Mesh2dMergeNodes(int meshKernelId, DisposableGeometryList disposableGeometryList, double mergingDistance)
        {
            var geometryList = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMergeNodes(meshKernelId, ref geometryList, mergingDistance) == 0;
        }

        public bool Mesh2dInitializeOrthogonalization(int meshKernelId,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption,
            OrthogonalizationParameters orthogonalizationParameters, DisposableGeometryList geometryListNativePolygon,
            DisposableGeometryList geometryListNativeLandBoundaries)
        {
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;

            var nativeOrthogonalizationParameters = orthogonalizationParameters.ToOrthogonalizationParametersNative();

            var geometryListPolygon = geometryListNativePolygon?.CreateNativeObject() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = geometryListNativeLandBoundaries?.CreateNativeObject() ??
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

        public bool Mesh2dMakeMeshFromPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeMeshFromPolygon(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeMeshFromSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeMeshFromSamples(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices)
        {
            return MeshKernelDll.Mesh2dCountMeshBoundariesAsPolygons(meshKernelId, ref numberOfPolygonVertices) == 0;
        }

        public bool Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref geometryListNative) == 0;
        }
        public bool Mesh2dRefineBasedOnSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, double relativeSearchRadius, int minimumNumSamples, MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, relativeSearchRadius, minimumNumSamples, ref meshRefinementParametersNative) == 0;
        }

        public bool Mesh2dRefineBasedOnPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, InterpolationParameters interpolationParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var interpolationParametersNative = interpolationParameters.ToInterpolationParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref interpolationParametersNative) == 0;
        }

        public bool Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetOrthogonality(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
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
            var geometryListNativeIn = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dDelete(meshKernelId, ref geometryListNativeIn, deletionOption, invertDeletion) == 0;
        }

        public bool Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dMoveNode(meshKernelId, ref xCoordinate, ref yCoordinate, ref vertexIndex) == 0;
        }

        public bool CurvilinearMakeUniform(int meshKernelId, MakeGridParameters makeGridParameters, DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();

            return MeshKernelDll.CurvilinearMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromSplines(int meshKernelId, DisposableGeometryList disposableGeometryListIn,
            CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
            ref CurvilinearParameters curvilinearParameters, ref SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, DisposableGeometryList geometryList, int firstNode,
            int secondNode, int thirdNode, bool useFourthSide)
        {

            var geometryListNative = geometryList.CreateNativeObject();
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
            var geometryListNative = geometryList.CreateNativeObject();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromTriangle(meshKernelId,
                ref geometryListNative,
                firstNode,
                secondNode,
                thirdNode) == 0;
        }


        public bool CurvilinearGetDimensions(int meshKernelId, ref CurvilinearGridNative curvilinearGridNative)
        {
            return MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGridNative) == 0;
        }

        public bool CurvilinearGetData(int meshKernelId, ref CurvilinearGridNative curvilinearGridNative)
        {
            return MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGridNative) == 0;
        }

        public bool CurvilinearConvertToMesh2D(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearConvertToMesh2D(meshKernelId) == 0;
        }

        public DisposableCurvilinearGrid CurvilinearGridGetDimensions(int meshKernelId)
        {
            var curvilinearGrid = new CurvilinearGridNative();

            MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid);
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_nodes, curvilinearGrid.num_edges);
            curvilinearGrid = disposableCurvilinearGrid.CreateNativeObject();

            return CreateDisposableCurvilinearGrid(curvilinearGrid);
        }

        public DisposableCurvilinearGrid CurvilinearGridGetData(int meshKernelId)
        {
            var curvilinearGrid = new CurvilinearGridNative();

            MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid);

            return CreateDisposableCurvilinearGrid(curvilinearGrid);
        }

        public bool GetSplines(DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListOut = disposableGeometryListOut.CreateNativeObject();
            if (MeshKernelDll.GetSplines(ref geometryListIn, ref geometryListOut,
                     numberOfPointsBetweenVertices) != 0)
            {
                return false;
            }
            disposableGeometryListOut.NumberOfCoordinates = geometryListOut.numberOfCoordinates;
            return true;
        }


        public bool PolygonCountOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonGetOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut) == 0;
        }

        public bool PolygonCountRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.PolygonCountRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.PolygonRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut) == 0;
        }

        public int[] GetSelectedVerticesInPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int inside)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int numberOfMeshVertices = -1;
            MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            IntPtr selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref selectedVerticesPtr);
            int[] selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            return selectedVertices;
        }

        public bool GetPointsInPolygon(int meshKernelId, ref DisposableGeometryList inputPolygon, ref DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateNativeObject();
            var geometryListNativeinputPoints = inputPoints.CreateNativeObject();
            var geometryListNativeSelectedPoints = selectedPoints.CreateNativeObject();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeSingle(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeSingle(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeMultiple(int meshKernelId, ref IntPtr oneDNodeMask)
        {
            return MeshKernelDll.ContactsComputeMultiple(meshKernelId, oneDNodeMask) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeWithPolygons(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPolygons(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeWithPoints(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList points)
        {
            var pointsNative = points.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPoints(meshKernelId, oneDNodeMask, ref pointsNative) == 0;
        }

        public DisposableContacts ContactsGetDimensions(int meshKernelId)
        {
            var contacts = new ContactsNative();

            MeshKernelDll.ContactsGetDimensions(meshKernelId, ref contacts);
            var disposableContacts = new DisposableContacts(contacts.num_contacts);

            return disposableContacts;
        }

        public DisposableContacts ContactsGetData(int meshKernelId)
        {
            var contacts = new ContactsNative();

            MeshKernelDll.ContactsGetDimensions(meshKernelId, ref contacts);
            var disposableContacts = new DisposableContacts(contacts.num_contacts);
            contacts = disposableContacts.CreateNativeObject();

            MeshKernelDll.ContactsGetData(meshKernelId, ref contacts);

            return CreateDisposableContacts(contacts);
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

        public bool Mesh1dSet(int meshKernelId, DisposableMesh1D disposableMesh1D)
        {
            var mesh1D = disposableMesh1D.CreateNativeObject();
            return MeshKernelDll.Mesh1dSet(meshKernelId, ref mesh1D) == 0;
        }

        private DisposableMesh2D CreateDisposableMesh2D(Mesh2DNative newMesh2DNative, bool addCellInformation = false)
        {
            var disposableMesh2D = new DisposableMesh2D
            {
                NodeX = newMesh2DNative.node_x.CreateValueArray<double>(newMesh2DNative.num_nodes),
                NodeY = newMesh2DNative.node_y.CreateValueArray<double>(newMesh2DNative.num_nodes),
                EdgeNodes = newMesh2DNative.edge_nodes.CreateValueArray<int>(newMesh2DNative.num_edges * 2).ToArray(),
                NumEdges = newMesh2DNative.num_edges,
                NumNodes = newMesh2DNative.num_nodes
            };

            if (addCellInformation && newMesh2DNative.num_faces > 0)
            {
                disposableMesh2D.NumFaces = newMesh2DNative.num_faces;
                disposableMesh2D.NodesPerFace = newMesh2DNative.nodes_per_face.CreateValueArray<int>(newMesh2DNative.num_faces); ;
                disposableMesh2D.FaceNodes = newMesh2DNative.face_nodes.CreateValueArray<int>(newMesh2DNative.num_faces);
                disposableMesh2D.FaceX = newMesh2DNative.face_x.CreateValueArray<double>(newMesh2DNative.num_faces);
                disposableMesh2D.FaceY = newMesh2DNative.face_y.CreateValueArray<double>(newMesh2DNative.num_faces);
            }

            return disposableMesh2D;
        }

        private DisposableCurvilinearGrid CreateDisposableCurvilinearGrid(CurvilinearGridNative curvilinearGridNative)
        {
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid
            {
                NodeX = curvilinearGridNative.node_x.CreateValueArray<double>(curvilinearGridNative.num_nodes),
                NodeY = curvilinearGridNative.node_y.CreateValueArray<double>(curvilinearGridNative.num_nodes),
                EdgeNodes = curvilinearGridNative.edge_nodes.CreateValueArray<int>(curvilinearGridNative.num_edges * 2).ToArray(),
                NumEdges = curvilinearGridNative.num_edges,
                NumNodes = curvilinearGridNative.num_nodes
            };

            return disposableCurvilinearGrid;
        }

        private DisposableContacts CreateDisposableContacts(ContactsNative contactsNative)
        {
            var disposableContacts = new DisposableContacts
            {
                Mesh1dIndices = contactsNative.mesh1d_indices.CreateValueArray<int>(contactsNative.num_contacts),
                Mesh2dIndices = contactsNative.mesh2d_indices.CreateValueArray<int>(contactsNative.num_contacts),
                NumContacts = contactsNative.num_contacts
            };

            return disposableContacts;
        }

        public void Dispose()
        {
            // Do nothing because no remoting is used
        }
    }
}