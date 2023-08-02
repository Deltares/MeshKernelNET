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

        public bool ContactsComputeBoundary(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons, double searchRadius)
        {
            var polygonsNative = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeBoundary(meshKernelId, oneDNodeMask, ref polygonsNative, searchRadius) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeMultiple(int meshKernelId, ref IntPtr oneDNodeMask)
        {
            return MeshKernelDll.ContactsComputeMultiple(meshKernelId, oneDNodeMask) == 0;
        }
        /// <inheritdoc />
        public bool ContactsComputeSingle(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons, double projectionFactor)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeSingle(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon, projectionFactor) == 0;
        }
        /// <inheritdoc />
        public bool ContactsComputeWithPoints(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList points)
        {
            var pointsNative = points.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPoints(meshKernelId, oneDNodeMask, ref pointsNative) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeWithPolygons(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPolygons(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon) == 0;
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
        
        public bool CurvilinearComputeTransfiniteFromSplines(int meshKernelId, 
                                                             DisposableGeometryList disposableGeometryListIn,
                                                             CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId, 
                                                                ref DisposableGeometryList disposableGeometryListIn,
                                                                ref CurvilinearParameters curvilinearParameters, 
                                                                ref SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, 
                                                             ref DisposableGeometryList geometryList, 
                                                             int firstNode,
                                                             int secondNode, 
                                                             int thirdNode, 
                                                             bool useFourthSide)
        {

            var geometryListNative = geometryList.CreateNativeObject();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromPolygon(meshKernelId,
                ref geometryListNative,
                firstNode,
                secondNode,
                thirdNode,
                useFourthSide) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromTriangle(int meshKernelId, 
                                                              ref DisposableGeometryList geometryList, 
                                                              int firstNode,
                                                              int secondNode, 
                                                              int thirdNode)
        {
            var geometryListNative = geometryList.CreateNativeObject();
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

        public bool CurvilinearDeleteNode(int meshKernelId, double xPointCoordinate, double yPointCoordinate)
        {
            return MeshKernelDll.CurvilinearDeleteNode(meshKernelId, xPointCoordinate, yPointCoordinate) == 0;
        }

        public bool CurvilinearDeleteOrthogonalGridFromSplines(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearDeleteOrthogonalGridFromSplines(meshKernelId) == 0;
        }

        public bool CurvilinearDerefine(int meshKernelId,
                                        double xLowerLeftCorner,
                                        double yLowerLeftCorner,
                                        double xUpperRightCorner,
                                        double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearDerefine(meshKernelId,
                                                     xLowerLeftCorner,
                                                     yLowerLeftCorner,
                                                     xUpperRightCorner,
                                                     yUpperRightCorner) == 0;
        }

        public bool CurvilinearFinalizeLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearFinalizeLineShift(meshKernelId) == 0;
        }

        public bool CurvilinearFinalizeOrthogonalize(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearFinalizeOrthogonalize(meshKernelId) == 0;
        }

        public DisposableCurvilinearGrid CurvilinearGridGetData(int meshKernelId)
        {
            var curvilinearGrid = new CurvilinearGridNative();
            MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid);
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_m, curvilinearGrid.num_n);
            curvilinearGrid = disposableCurvilinearGrid.CreateNativeObject();

            MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid);

            return CreateDisposableCurvilinearGrid(curvilinearGrid);
        }

        public bool CurvilinearInitializeLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearInitializeLineShift(meshKernelId) == 0;
        }

        public bool CurvilinearInitializeOrthogonalGridFromSplines(int meshKernelId, 
                                                                   ref DisposableGeometryList disposableGeometryListIn,
                                                                   ref CurvilinearParameters curvilinearParameters,
                                                                   ref SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative =  curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearInitializeOrthogonalGridFromSplines( meshKernelId, 
                ref geometryListNativeIn, 
                ref curvilinearParametersNative, 
                ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool CurvilinearInitializeOrthogonalize(int meshKernelId,
                                                       ref OrthogonalizationParameters orthogonalizationParameters)
        {
            var orthogonalizationParametersNative = orthogonalizationParameters.ToOrthogonalizationParametersNative();
            return MeshKernelDll.CurvilinearInitializeOrthogonalize(meshKernelId, ref orthogonalizationParametersNative) == 0;
        }

        public bool CurvilinearInsertFace(int meshKernelId, double xCoordinate, double yCoordinate)
        {
            return MeshKernelDll.CurvilinearInsertFace(meshKernelId, xCoordinate, yCoordinate) == 0;
        }

        public bool CurvilinearIterateOrthogonalGridFromSplines(int meshKernelId, int layer)
        {
            return MeshKernelDll.CurvilinearIterateOrthogonalGridFromSplines(meshKernelId, layer) == 0;
        }

        public bool CurvilinearLineAttractionRepulsion(int meshKernelId, 
                                                       double repulsionParameter,
                                                       double xFirstNodeOnTheLine,
                                                       double yFirstNodeOnTheLine,
                                                       double xSecondNodeOnTheLine,
                                                       double ySecondNodeOnTheLine,
                                                       double xLowerLeftCorner,
                                                       double yLowerLeftCorner,
                                                       double xUpperRightCorner,
                                                       double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearLineAttractionRepulsion(meshKernelId, 
                                                                    repulsionParameter,
                                                                    xFirstNodeOnTheLine,
                                                                    yFirstNodeOnTheLine,
                                                                    xSecondNodeOnTheLine,
                                                                    ySecondNodeOnTheLine,
                                                                    xLowerLeftCorner,
                                                                    yLowerLeftCorner,
                                                                    xUpperRightCorner,
                                                                    yUpperRightCorner) == 0;
        }

        public bool CurvilinearLineMirror(int meshKernelId,
                                          double mirroringFactor,
                                          double xFirstGridLineNode,
                                          double yFirstGridLineNode,
                                          double xSecondGridLineNode,
                                          double ySecondGridLineNode)
        {
            return MeshKernelDll.CurvilinearLineMirror(meshKernelId,
                                                       mirroringFactor,
                                                       xFirstGridLineNode,
                                                       yFirstGridLineNode,
                                                       xSecondGridLineNode,
                                                       ySecondGridLineNode) == 0;
        }

        public bool CurvilinearLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearLineShift(meshKernelId) == 0;
        }

        public bool CurvilinearMakeUniform(int meshKernelId, 
                                           MakeGridParameters makeGridParameters, 
                                           DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();

            return MeshKernelDll.CurvilinearMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool CurvilinearMakeUniformOnExtension(int meshKernelId, MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();

            return MeshKernelDll.CurvilinearMakeUniformOnExtension(meshKernelId, ref makeGridParametersNative) == 0;
        }

        public bool CurvilinearMoveNode(int meshKernelId, double xFromPoint, double yFromPoint, double xToPoint, double yToPoint)
        {
            return MeshKernelDll.CurvilinearMoveNode(meshKernelId, xFromPoint, yFromPoint, xToPoint, yToPoint) == 0;
        }

        public bool CurvilinearMoveNodeLineShift(int meshKernelId, 
                                                 double xFromCoordinate,
                                                 double yFromCoordinate,
                                                 double xToCoordinate,
                                                 double yToCoordinate)
        {
            return MeshKernelDll.CurvilinearMoveNodeLineShift(meshKernelId, xFromCoordinate, yFromCoordinate, xToCoordinate, yToCoordinate) == 0;
        }

        public bool CurvilinearOrthogonalize(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearOrthogonalize(meshKernelId) == 0;
        }

        public bool CurvilinearRefine(int meshKernelId,
                                      double xLowerLeftCorner,
                                      double yLowerLeftCorner,
                                      double xUpperRightCorner,
                                      double yUpperRightCorner,
                                      int refinement)
        {
            return MeshKernelDll.CurvilinearRefine(meshKernelId, xLowerLeftCorner, yLowerLeftCorner, xUpperRightCorner, yUpperRightCorner, refinement) == 0;
        }

        public bool CurvilinearRefreshOrthogonalGridFromSplines(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearRefreshOrthogonalGridFromSplines(meshKernelId) == 0;
        }

        public bool CurvilinearSet(int meshKernelId, DisposableCurvilinearGrid grid)
        {
            var gridNative = grid.CreateNativeObject();
            return MeshKernelDll.CurvilinearSet(meshKernelId, ref gridNative) == 0;
        }

        public bool CurvilinearSetBlockLineShift(int meshKernelId,
                                                 double xLowerLeftCorner,
                                                 double yLowerLeftCorner,
                                                 double xUpperRightCorner,
                                                 double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetBlockLineShift(meshKernelId,
                xLowerLeftCorner,
                yLowerLeftCorner,
                xUpperRightCorner,
                yUpperRightCorner) == 0;
        }

        public bool CurvilinearSetFrozenLinesOrthogonalize(int meshKernelId,
                                                           double xFirstGridLineNode,
                                                           double yFirstGridLineNode,
                                                           double xSecondGridLineNode,
                                                           double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetFrozenLinesOrthogonalize(meshKernelId,
                                                                        xFirstGridLineNode,
                                                                        yFirstGridLineNode,
                                                                        xSecondGridLineNode, 
                                                                        yUpperRightCorner) == 0;
        }

        public bool CurvilinearSetLineLineShift(int meshKernelId,
                                                double xFirstGridLineNode,
                                                double yFirstGridLineNode,
                                                double xSecondGridLineNode,
                                                double ySecondGridLineNode)
        {
            return MeshKernelDll.CurvilinearSetLineLineShift(meshKernelId,
                                                             xFirstGridLineNode,
                                                             yFirstGridLineNode,
                                                             xSecondGridLineNode,
                                                             ySecondGridLineNode) == 0;
        }

        public bool CurvilinearSmoothing(int meshKernelId,
                                         int smoothingIterations,
                                         double xLowerLeftCorner,
                                         double yLowerLeftCorner,
                                         double xUpperRightCorner,
                                         double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSmoothing(meshKernelId,
                                                      smoothingIterations,
                                                      xLowerLeftCorner,
                                                      yLowerLeftCorner,
                                                      xUpperRightCorner,
                                                      yUpperRightCorner) == 0;
        }

        public bool CurvilinearSmoothingDirectional(int meshKernelId,
                                                    int smoothingIterations,
                                                    double xFirstGridlineNode,
                                                    double yFirstGridlineNode,
                                                    double xSecondGridLineNode,
                                                    double ySecondGridLineNode,
                                                    double xLowerLeftCornerSmoothingArea,
                                                    double yLowerLeftCornerSmoothingArea,
                                                    double xUpperRightCornerSmoothingArea,
                                                    double yUpperRightCornerSmoothingArea)
        {
            return MeshKernelDll.CurvilinearSmoothingDirectional(meshKernelId,
                                                                 smoothingIterations,
                                                                 xFirstGridlineNode,
                                                                 yFirstGridlineNode,
                                                                 xSecondGridLineNode,
                                                                 ySecondGridLineNode,
                                                                 xLowerLeftCornerSmoothingArea,
                                                                 yLowerLeftCornerSmoothingArea,
                                                                 xUpperRightCornerSmoothingArea,
                                                                 yUpperRightCornerSmoothingArea) == 0;
        }

        public bool DeallocateState(int meshKernelId)
        {
            return MeshKernelDll.DeallocateState(meshKernelId) == 0;
        }

        public bool GetAveragingMethodClosestPoint(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodClosestPoint(ref method) == 0;
        }

        public bool GetAveragingMethodInverseDistanceWeighting(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodInverseDistanceWeighting(ref method) == 0;
        }

        public bool GetAveragingMethodMax(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMax(ref method) == 0;
        }

        public bool GetAveragingMethodMin(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMin(ref method) == 0;
        }

        public bool GetAveragingMethodMinAbsoluteValue(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMinAbsoluteValue(ref method) == 0;
        }

        public bool GetAveragingMethodSimpleAveraging(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodSimpleAveraging(ref method) == 0;
        }

        public bool GetEdgesLocationType(ref int type)
        {
            return MeshKernelDll.GetEdgesLocationType(ref type) == 0;
        }

        public bool GetError(ref string errorMessage)
        {
            int bufferSize = 512;
            var errorMessagePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * bufferSize);
            if (MeshKernelDll.GetError(errorMessagePtr) != 0)
            {
                return false;
            }

            var messageBuffer = new char[bufferSize];
            Marshal.Copy(errorMessagePtr, messageBuffer, 0, bufferSize);
            errorMessage = new string(messageBuffer);
            Marshal.FreeCoTaskMem(errorMessagePtr);

            return true;
        }

        public bool GetFacesLocationType(ref int type)
        {
            return MeshKernelDll.GetFacesLocationType(ref type)==0;
        }

        public bool GetGeometryError(ref int invalidIndex, ref int type)
        {
            return MeshKernelDll.GetGeometryError(ref invalidIndex, ref type) == 0;
        }

        public bool GetNodesLocationType(ref int type)
        {
            return MeshKernelDll.GetNodesLocationType(ref type) == 0;
        }

        public bool GetProjection(int meshKernelId, ref int projection)
        {
            return MeshKernelDll.GetProjection(meshKernelId, ref projection) == 0;
        }

        public bool GetProjectionCartesian(ref int projection)
        {
            return MeshKernelDll.GetProjectionCartesian(ref projection) == 0;
        }

        public bool GetProjectionSpherical(ref int projection)
        {
            return MeshKernelDll.GetProjectionSpherical(ref projection) == 0;
        }

        public bool GetProjectionSphericalAccurate(ref int projection)
        {
            return MeshKernelDll.GetProjectionSphericalAccurate(ref projection) == 0;
        }

        public bool GetSplines(DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListOut = disposableGeometryListOut.CreateNativeObject();
            if (MeshKernelDll.GetSplines(ref geometryListIn, ref geometryListOut, numberOfPointsBetweenVertices) != 0)
            {
                return false;
            }
            disposableGeometryListOut.NumberOfCoordinates = geometryListOut.numberOfCoordinates;
            return true;
        }

        public bool GetVersion(ref string version)
        {
            int bufferSize = 64;
            var versionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * bufferSize);
            if (MeshKernelDll.GetVersion(versionPtr) != 0)
            {
                return false;
            }

            var versionBuffer = new char[bufferSize];
            Marshal.Copy(versionPtr, versionBuffer, 0, bufferSize);
            version = new string(versionBuffer);
            Marshal.FreeCoTaskMem(versionPtr);

            return true;
        }

        public DisposableMesh1D Mesh1dGetData(int meshKernelId)
        {
            var newMesh1D = new Mesh1DNative();

            MeshKernelDll.Mesh1dGetDimensions(meshKernelId, ref newMesh1D);
            var disposableMesh1D = new DisposableMesh1D(newMesh1D.num_nodes, newMesh1D.num_edges);
            var mesh1d = disposableMesh1D.CreateNativeObject();

            MeshKernelDll.Mesh1dGetData(meshKernelId, ref mesh1d);

            return disposableMesh1D;
        }

        public bool Mesh1dSet(int meshKernelId, DisposableMesh1D disposableMesh1D)
        {
            var mesh1D = disposableMesh1D.CreateNativeObject();
            return MeshKernelDll.Mesh1dSet(meshKernelId, ref mesh1D) == 0;
        }

        public bool Mesh2dAveragingInterpolation(int meshKernelId,
                                                 ref DisposableGeometryList samples,
                                                 int locationType,
                                                 int averagingMethodType,
                                                 double relativeSearchSize,
                                                 int minNumSamples,
                                                 ref DisposableGeometryList results)
        {
            var samplesNative = samples.CreateNativeObject();
            var resultsNative = results.CreateNativeObject();
            
            return MeshKernelDll.Mesh2dAveragingInterpolation(meshKernelId, 
                                                              ref samplesNative,
                                                              locationType,
                                                              averagingMethodType,
                                                              relativeSearchSize,
                                                              minNumSamples, 
                                                              ref resultsNative) == 0;
        }

        public bool Mesh2dComputeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dComputeInnerOrtogonalizationIteration(meshKernelId) == 0;
        }

        public bool Mesh2dComputeOrthogonalization(int meshKernelId,
                                                   int projectToLandBoundaryOption,
                                                   ref OrthogonalizationParameters orthogonalizationParameters,
                                                   ref DisposableGeometryList geometryListPolygon,
                                                   ref DisposableGeometryList geometryListLandBoundaries)
        {
            var orthogonalizationParametersNative = orthogonalizationParameters.ToOrthogonalizationParametersNative();
            var geometryListNativePolygon= geometryListPolygon.CreateNativeObject();
            var geometryListNativeLandBoundaries= geometryListLandBoundaries.CreateNativeObject();

            return MeshKernelDll.Mesh2dComputeOrthogonalization(meshKernelId,
                                                                projectToLandBoundaryOption,
                                                                ref orthogonalizationParametersNative,
                                                                ref geometryListNativePolygon,
                                                                ref geometryListNativeLandBoundaries) == 0;
        }

        public bool Mesh2dCountHangingEdges(int meshKernelId, ref int numEdges)
        {
            return MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numEdges) == 0;
        }

        public bool Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices)
        {
            return MeshKernelDll.Mesh2dCountMeshBoundariesAsPolygons(meshKernelId, ref numberOfPolygonVertices) == 0;
        }

        public bool Mesh2dCountSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesLengthThreshold, ref int numSmallFlowEdges)
        {
            return MeshKernelDll.Mesh2dCountSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges) == 0;
        }
        public bool Mesh2dDelete(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut, int deletionOption, bool invertDeletion)
        {
            var geometryListNativeIn = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dDelete(meshKernelId, ref geometryListNativeIn, deletionOption, invertDeletion) == 0;
        }
        public bool Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox)
        {
            return MeshKernelDll.Mesh2dDeleteEdge(meshKernelId, xCoordinate, yCoordinate, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox) == 0;
        }

        public bool Mesh2dDeleteHangingEdges(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteHangingEdges(meshKernelId) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dDeleteNode(int meshKernelId, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dDeleteNode(meshKernelId, vertexIndex) == 0;
        }
        public bool Mesh2dDeleteOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteOrthogonalization(meshKernelId) == 0;
        }

        public bool Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(int meshKernelId, 
                                                                double smallFlowEdgesThreshold, 
                                                                double minFractionalAreaTriangles)
        {
            return MeshKernelDll.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(meshKernelId, smallFlowEdgesThreshold, minFractionalAreaTriangles) == 0;
        }
        public bool Mesh2dFinalizeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dFinalizeInnerOrtogonalizationIteration(meshKernelId) == 0;
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

        public bool Mesh2dGetClosestNode(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox, ref double xCoordinateOut, ref double yCoordinateOut)
        {
            return MeshKernelDll.Mesh2dGetClosestNode(meshKernelId, xCoordinateIn, yCoordinateIn, searchRadius, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox, ref xCoordinateOut, ref yCoordinateOut) == 0;
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

        public bool Mesh2dGetEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dGetEdge(meshKernelId, xCoordinate, yCoordinate, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox, ref edgeIndex) == 0;
        }

        public int[] Mesh2dGetHangingEdges(int meshKernelId)
        {
            int numberOfHangingEdges = -1;
            MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numberOfHangingEdges);
            IntPtr hangingVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfHangingEdges);
            MeshKernelDll.Mesh2dGetHangingEdges(meshKernelId, hangingVerticesPtr);
            int[] hangingEdges = new int[numberOfHangingEdges];
            Marshal.Copy(hangingVerticesPtr, hangingEdges, 0, numberOfHangingEdges);
            Marshal.FreeCoTaskMem(hangingVerticesPtr);
            return hangingEdges;
        }

        public bool Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dGetNodeIndex(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox, ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dGetNodeIndex(meshKernelId, xCoordinateIn, yCoordinateIn, searchRadius, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox, ref vertexIndex) == 0;
        }

        public int[] GetSelectedVerticesInPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int inside)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int numberOfMeshVertices = -1;
            MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            IntPtr selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, selectedVerticesPtr);
            int[] selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            Marshal.FreeCoTaskMem(selectedVerticesPtr);
            return selectedVertices;
        }

        public bool Mesh2dGetObtuseTrianglesMassCenters(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetObtuseTrianglesMassCenters(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetOrthogonality(meshKernelId, ref geometryListNativeIn) == 0;
        }

        public bool Mesh2dCountObtuseTriangles(int meshKernelId, ref int numObtuseTriangles)
        {
            return MeshKernelDll.Mesh2dCountObtuseTriangles(meshKernelId, ref numObtuseTriangles) == 0;
        }

        public bool Mesh2dGetSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesThreshold, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesThreshold, ref geometryListNative) == 0;
        }

        public bool Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmoothness(meshKernelId, ref geometryListNativeInOut) == 0;
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

        /// <inheritdoc />
        public bool Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dInsertEdge(meshKernelId, startVertexIndex, endVertexIndex, ref edgeIndex) == 0;
        }

        public bool Mesh2dInsertNode(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dInsertNode(meshKernelId, xCoordinate, yCoordinate, ref vertexIndex) == 0;
        }
        
        public bool Mesh2dIntersectionsFromPolyline(int meshKernelId,
                                                    ref DisposableGeometryList boundaryPolyLine,
                                                    ref IntPtr edgeNodes,
                                                    ref IntPtr edgeIndex,
                                                    ref IntPtr edgeDistances,
                                                    ref IntPtr segmentDistances,
                                                    ref IntPtr segmentIndexes,
                                                    ref IntPtr faceIndexes,
                                                    ref IntPtr faceNumEdges,
                                                    ref IntPtr faceEdgeIndex)
        {
            var boundaryPolyLineNative = boundaryPolyLine.CreateNativeObject();
            return MeshKernelDll.Mesh2dIntersectionsFromPolyline(meshKernelId, 
                                                                 ref boundaryPolyLineNative,
                                                                 edgeNodes,
                                                                 edgeIndex,
                                                                 edgeDistances,
                                                                 segmentDistances,
                                                                 segmentIndexes,
                                                                 faceIndexes,
                                                                 faceNumEdges,
                                                                 faceEdgeIndex) == 0;
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

        public bool Mesh2dMakeUniform(int meshKernelId, ref MakeGridParameters makeGridParameters, ref DisposableGeometryList geometryList)
        {
            var geometryListNative = geometryList.CreateNativeObject();
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeUniformOnExtension(int meshKernelId, ref MakeGridParameters makeGridParameters, ref DisposableGeometryList geometryList)
        {
            var geometryListNative = geometryList.CreateNativeObject();
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeUniformOnExtension(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dMergeNodes(int meshKernelId, DisposableGeometryList disposableGeometryList, double mergingDistance)
        {
            var geometryList = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMergeNodes(meshKernelId, ref geometryList, mergingDistance) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dMergeTwoNodes(int meshKernelId, int startVertexIndex, int endVertexIndex)
        {
            return MeshKernelDll.Mesh2dMergeTwoNodes(meshKernelId, startVertexIndex, endVertexIndex) == 0;
        }

        public bool Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dMoveNode(meshKernelId, xCoordinate, yCoordinate, vertexIndex) == 0;
        }

        public bool Mesh2dPrepareOuterIterationOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dPrepareOuterIterationOrthogonalization(meshKernelId) == 0;
        }

        public bool Mesh2dRefineBasedOnGriddedSamples(int meshKernelId, 
            ref DisposableGriddedSamples griddedSamples, 
            ref MeshRefinementParameters meshRefinementParameters, 
            bool useNodalRefinement)
        {
            var griddedSamplesNative = griddedSamples.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();

            return MeshKernelDll.Mesh2dRefineBasedOnGriddedSamples(meshKernelId, 
                                                                   ref griddedSamplesNative,
                                                                   ref meshRefinementParametersNative, 
                                                                   useNodalRefinement) == 0;
        }

        public bool Mesh2dRefineBasedOnPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref meshRefinementParametersNative) == 0;
        }

        public bool Mesh2dRefineBasedOnSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, double relativeSearchRadius, int minimumNumSamples, MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, relativeSearchRadius, minimumNumSamples, ref meshRefinementParametersNative) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dSet(int meshKernelId, DisposableMesh2D disposableMesh2D)
        {
            var mesh2D = disposableMesh2D.CreateNativeObject();

            var result = MeshKernelDll.Mesh2dSet(meshKernelId, ref mesh2D);

            return result == 0;
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

        public double Mesh2dTriangulationInterpolation(int meshKernelId, ref DisposableGeometryList samples, int locationType, ref DisposableGeometryList results)
        {
            var samplesNative = samples.CreateNativeObject();
            var resultsNative = results.CreateNativeObject();
            return MeshKernelDll.Mesh2dTriangulationInterpolation(meshKernelId, ref samplesNative, locationType, ref  resultsNative);
        }

        public bool Network1dComputeFixedChainages(int meshKernelId, ref double[] fixedChainages, double minFaceSize, double fixedChainagesOffset)
        {

            int numFixedChainages = fixedChainages.Length;
            IntPtr fixedChainagesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * numFixedChainages);
            bool succcess =  MeshKernelDll.Network1dComputeFixedChainages(meshKernelId, ref fixedChainagesPtr, numFixedChainages, minFaceSize, fixedChainagesOffset)==0;
            Marshal.FreeCoTaskMem(fixedChainagesPtr);
            return succcess;
        }

        public bool Network1dComputeOffsettedChainages(int meshKernelId, double offset)
        {
            return MeshKernelDll.Network1dComputeOffsettedChainages(meshKernelId, offset) == 0;
        }

        public bool Network1dSet(int meshKernelId, ref DisposableGeometryList offset)
        {
            var offsetNative = offset.CreateNativeObject();
            return MeshKernelDll.Network1dSet(meshKernelId, ref offsetNative) == 0;
        }

        public bool PolygonCountOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonCountRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.PolygonCountRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool GetPointsInPolygon(int meshKernelId, ref DisposableGeometryList inputPolygon, ref DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateNativeObject();
            var geometryListNativeinputPoints = inputPoints.CreateNativeObject();
            var geometryListNativeSelectedPoints = selectedPoints.CreateNativeObject();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints) == 0;
        }

        public bool PolygonGetOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut) == 0;
        }

        public bool PolygonRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.PolygonRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut) == 0;
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
                disposableMesh2D.NodesPerFace = newMesh2DNative.nodes_per_face.CreateValueArray<int>(newMesh2DNative.num_faces);
                int numFaceNodes = disposableMesh2D.NodesPerFace.Sum();
                disposableMesh2D.FaceNodes = newMesh2DNative.face_nodes.CreateValueArray<int>(numFaceNodes);
                disposableMesh2D.FaceX = newMesh2DNative.face_x.CreateValueArray<double>(newMesh2DNative.num_faces);
                disposableMesh2D.FaceY = newMesh2DNative.face_y.CreateValueArray<double>(newMesh2DNative.num_faces);
            }

            return disposableMesh2D;
        }

        private DisposableCurvilinearGrid CreateDisposableCurvilinearGrid(CurvilinearGridNative curvilinearGridNative)
        {
            var disposableCurvilinearGrid = new DisposableCurvilinearGrid
            {
                NumM = curvilinearGridNative.num_m,
                NumN = curvilinearGridNative.num_n,
                NodeX = curvilinearGridNative.node_x.CreateValueArray<double>(curvilinearGridNative.num_m * curvilinearGridNative.num_n),
                NodeY = curvilinearGridNative.node_y.CreateValueArray<double>(curvilinearGridNative.num_m * curvilinearGridNative.num_n),
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