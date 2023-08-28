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

        public bool ContactsComputeBoundary(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double searchRadius)
        {
            var polygonsNative = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeBoundary(meshKernelId, oneDNodeMask, ref polygonsNative, searchRadius) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeMultiple(int meshKernelId, in IntPtr oneDNodeMask)
        {
            return MeshKernelDll.ContactsComputeMultiple(meshKernelId, oneDNodeMask) == 0;
        }
        /// <inheritdoc />
        public bool ContactsComputeSingle(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double projectionFactor)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeSingle(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon, projectionFactor) == 0;
        }
        /// <inheritdoc />
        public bool ContactsComputeWithPoints(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList points)
        {
            var pointsNative = points.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPoints(meshKernelId, oneDNodeMask, ref pointsNative) == 0;
        }

        /// <inheritdoc />
        public bool ContactsComputeWithPolygons(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPolygons(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon) == 0;
        }

        public bool ContactsGetData(int meshKernelId, out DisposableContacts disposableContacts)
        {
            disposableContacts = new DisposableContacts();
            var contacts = disposableContacts.CreateNativeObject();
            if (MeshKernelDll.ContactsGetDimensions(meshKernelId, ref contacts) != 0)
            {
                
                return false;
            }

            disposableContacts = new DisposableContacts(contacts.num_contacts);
            contacts = disposableContacts.CreateNativeObject();
            if (MeshKernelDll.ContactsGetData(meshKernelId, ref contacts) != 0)
            {
                return false;
            }

            disposableContacts = CreateDisposableContacts(contacts);

            return true;
        }
        
        public bool CurvilinearComputeTransfiniteFromSplines(int meshKernelId, 
                                                             in DisposableGeometryList disposableGeometryListIn,
                                                             in CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId, 
                                                                in DisposableGeometryList disposableGeometryListIn,
                                                                in CurvilinearParameters curvilinearParameters, 
                                                                in SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative) == 0;
        }

        public bool CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, 
                                                             in DisposableGeometryList geometryList, 
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
                                                              in DisposableGeometryList geometryList, 
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

        public bool CurvilinearGridGetData(int meshKernelId, out DisposableCurvilinearGrid disposableCurvilinearGrid)
        {
            var curvilinearGrid = new CurvilinearGridNative();
            
            bool success = MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid)==0;
            if (!success)
            {
                disposableCurvilinearGrid = new DisposableCurvilinearGrid();
                return false;
            }

            disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_m, curvilinearGrid.num_n);
            curvilinearGrid = disposableCurvilinearGrid.CreateNativeObject();

            success = MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid)==0;
            if (!success)
            {
                disposableCurvilinearGrid = new DisposableCurvilinearGrid();
                return false;
            }

            disposableCurvilinearGrid =  CreateDisposableCurvilinearGrid(curvilinearGrid);
            return true;
        }

        public bool CurvilinearInitializeLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearInitializeLineShift(meshKernelId) == 0;
        }

        public bool CurvilinearInitializeOrthogonalGridFromSplines(int meshKernelId, 
                                                                   in DisposableGeometryList disposableGeometryListIn,
                                                                   in CurvilinearParameters curvilinearParameters,
                                                                   in SplinesToCurvilinearParameters splinesToCurvilinearParameters)
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
                                                       in OrthogonalizationParameters orthogonalizationParameters)
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
                                           in MakeGridParameters makeGridParameters, 
                                           in DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative =
                makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();

            return MeshKernelDll.CurvilinearMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool CurvilinearMakeUniformOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters)
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

        public bool CurvilinearSet(int meshKernelId, in DisposableCurvilinearGrid grid)
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

        public bool CurvilinearSetBlockOrthogonalize(int meshKernelId,
            double xLowerLeftCorner,
            double yLowerLeftCorner,
            double xUpperRightCorner,
            double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetBlockOrthogonalize(meshKernelId,
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

        public bool GetError(out string errorMessage)
        {
            var bufferSize = 512;
            var errorMessagePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            if (MeshKernelDll.GetError(errorMessagePtr) != 0)
            {
                errorMessage = "";
                return false;
            }
            errorMessage = Marshal.PtrToStringAnsi(errorMessagePtr);
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

        public bool GetSplines(in DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var nativeGeometryListIn = disposableGeometryListIn.CreateNativeObject();
            var nativeGeometryListOut = disposableGeometryListOut.CreateNativeObject();
            if (MeshKernelDll.GetSplines(ref nativeGeometryListIn, ref nativeGeometryListOut, numberOfPointsBetweenVertices) != 0)
            {
                return false;
            }
            return true;
        }

        public bool GetVersion(out string version)
        {
            var bufferSize = 64;
            version ="";
            var versionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            if (MeshKernelDll.GetVersion(versionPtr) != 0)
            {
                return false;
            }
            version = Marshal.PtrToStringAnsi(versionPtr);
            Marshal.FreeCoTaskMem(versionPtr);

            return true;
        }

        public bool Mesh1dGetData(int meshKernelId, out DisposableMesh1D disposableMesh1D)
        {
            var newMesh1D = new Mesh1DNative();

            var success = MeshKernelDll.Mesh1dGetDimensions(meshKernelId, ref newMesh1D) == 0;
            
            if (!success)
            {
                disposableMesh1D = new DisposableMesh1D();
                return false;
            }

            disposableMesh1D = new DisposableMesh1D(newMesh1D.num_nodes, newMesh1D.num_edges);

            
            newMesh1D = disposableMesh1D.CreateNativeObject();

            success = MeshKernelDll.Mesh1dGetData(meshKernelId, ref newMesh1D) == 0;
            disposableMesh1D = CreateDisposableMesh1d(newMesh1D);

            return success;
            
        }

        public bool Mesh1dSet(int meshKernelId, in DisposableMesh1D disposableMesh1D)
        {
            var mesh1D = disposableMesh1D.CreateNativeObject();
            return MeshKernelDll.Mesh1dSet(meshKernelId, ref mesh1D) == 0;
        }

        public bool Mesh2dAveragingInterpolation(int meshKernelId, 
                                                 in DisposableGeometryList samples,
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
                                                   ProjectToLandBoundaryOptions projectToLandBoundaryOption,
                                                   OrthogonalizationParameters orthogonalizationParameters,
                                                   in DisposableGeometryList geometryListPolygon,
                                                   in DisposableGeometryList geometryListLandBoundaries)
        {
            var orthogonalizationParametersNative = orthogonalizationParameters.ToOrthogonalizationParametersNative();

            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            var geometryListPolygonNative = geometryListPolygon?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundariesNative = geometryListLandBoundaries?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            

            return MeshKernelDll.Mesh2dComputeOrthogonalization(meshKernelId, 
                                                                projectToLandBoundaryOptionInt,
                                                                ref orthogonalizationParametersNative,
                                                                ref geometryListPolygonNative,
                                                                ref geometryListLandBoundariesNative) == 0;
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

        public bool Mesh2dFlipEdges(int meshKernelId, bool isTriangulationRequired, ProjectToLandBoundaryOptions projectToLandBoundaryOption, in DisposableGeometryList selectingPolygon, in DisposableGeometryList landBoundaries)
        {
            int isTriangulationRequiredInt = isTriangulationRequired ? 1 : 0;
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            var geometryListPolygon = selectingPolygon?.CreateNativeObject() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = landBoundaries?.CreateNativeObject() ??
                                             new GeometryListNative { numberOfCoordinates = 0 };
            return MeshKernelDll.Mesh2dFlipEdges(meshKernelId, isTriangulationRequiredInt, projectToLandBoundaryOptionInt, ref geometryListPolygon, ref geometryListLandBoundaries) == 0;
        }

        public bool Mesh2dGetClosestNode(int meshKernelId, 
                                         double xCoordinateIn, 
                                         double yCoordinateIn, 
                                         double searchRadius, 
                                         double xLowerLeftBoundingBox, 
                                         double yLowerLeftBoundingBox, 
                                         double xUpperRightBoundingBox, 
                                         double yUpperRightBoundingBox, 
                                         ref double xCoordinateOut, 
                                         ref double yCoordinateOut)
        {
            return MeshKernelDll.Mesh2dGetClosestNode(meshKernelId, 
                                                      xCoordinateIn, 
                                                      yCoordinateIn, 
                                                      searchRadius,
                                                      xLowerLeftBoundingBox, 
                                                      yLowerLeftBoundingBox, 
                                                      xUpperRightBoundingBox, 
                                                      yUpperRightBoundingBox, 
                                                      ref xCoordinateOut, 
                                                      ref yCoordinateOut) == 0;
        }
        public bool Mesh2dGetData(int meshKernelId, out DisposableMesh2D disposableMesh2D)
        {
           
            var newMesh2D = new Mesh2DNative();

            var success = MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D) == 0;

            if (!success)
            {
                disposableMesh2D = new DisposableMesh2D();
                return false;
            }
            disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes,
                                                    newMesh2D.num_edges,
                                                    newMesh2D.num_faces,
                                                    newMesh2D.num_face_nodes);

            newMesh2D = disposableMesh2D.CreateNativeObject();

            success = MeshKernelDll.Mesh2dGetData(meshKernelId, ref newMesh2D) == 0;
            disposableMesh2D = CreateDisposableMesh2D(newMesh2D, true);

            return success;
        }

        public bool Mesh2dGetEdge(int meshKernelId, 
                                  double xCoordinate, 
                                  double yCoordinate, 
                                  double xLowerLeftBoundingBox, 
                                  double yLowerLeftBoundingBox, 
                                  double xUpperRightBoundingBox, 
                                  double yUpperRightBoundingBox, 
                                  ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dGetEdge(meshKernelId,
                                               xCoordinate, 
                                               yCoordinate, 
                                               xLowerLeftBoundingBox, 
                                               yLowerLeftBoundingBox, 
                                               xUpperRightBoundingBox, 
                                               yUpperRightBoundingBox, 
                                               ref edgeIndex) == 0;
        }

        public bool Mesh2dGetHangingEdges(int meshKernelId, out int[] hangingEdges)
        {
            int numberOfHangingEdges = -1;
            hangingEdges = new int[]{};
            if (MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numberOfHangingEdges) != 0)
            {
                return false;
            }

            if (numberOfHangingEdges <= 0)
            {
                return true;
            }

            IntPtr hangingVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfHangingEdges);
            if (MeshKernelDll.Mesh2dGetHangingEdges(meshKernelId, hangingVerticesPtr) != 0)
            {
                return false;
            }

            hangingEdges = new int[numberOfHangingEdges];

            Marshal.Copy(hangingVerticesPtr, hangingEdges, 0, numberOfHangingEdges);

            Marshal.FreeCoTaskMem(hangingVerticesPtr);

            return true;
        }

        public bool Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dGetNodeIndex(int meshKernelId, 
                                       double xCoordinateIn, 
                                       double yCoordinateIn, 
                                       double searchRadius, 
                                       double xLowerLeftBoundingBox,
                                       double yLowerLeftBoundingBox, 
                                       double xUpperRightBoundingBox, 
                                       double yUpperRightBoundingBox, 
                                       ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dGetNodeIndex(meshKernelId, 
                                                    xCoordinateIn,
                                                    yCoordinateIn,
                                                    searchRadius,
                                                    xLowerLeftBoundingBox, 
                                                    yLowerLeftBoundingBox, 
                                                    xUpperRightBoundingBox, 
                                                    yUpperRightBoundingBox, 
                                                    ref vertexIndex) == 0;
        }

        public bool GetSelectedVerticesInPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int inside, ref int[] selectedVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };

            int numberOfMeshVertices = -1;
            if (MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices)!=0)
            {
                return false;
            }

            var selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            if (MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, selectedVerticesPtr) != 0)
            {
                Marshal.FreeCoTaskMem(selectedVerticesPtr);
                return false;
            }

            selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            Marshal.FreeCoTaskMem(selectedVerticesPtr);
            return true;
        }

        public bool Mesh2dGetObtuseTrianglesMassCenters(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var disposableGeometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetObtuseTrianglesMassCenters(meshKernelId, ref disposableGeometryListNative) == 0;
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
            var geometryListNativeIn = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesThreshold, ref geometryListNativeIn) == 0;
        }

        public bool Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmoothness(meshKernelId, ref geometryListNativeInOut) == 0;
        }

        public bool Mesh2dInitializeOrthogonalization(int meshKernelId,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption,
            in OrthogonalizationParameters orthogonalizationParameters,
            in DisposableGeometryList geometryListNativePolygon,
            in DisposableGeometryList geometryListNativeLandBoundaries)
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
                                                    in DisposableGeometryList boundaryPolyLine,
                                                    ref int[] edgeNodes, 
                                                    ref int[] edgeIndex,
                                                    ref double[] edgeDistances,
                                                    ref double[] segmentDistances,
                                                    ref int[] segmentIndexes,
                                                    ref int[] faceIndexes,
                                                    ref int[] faceNumEdges,
                                                    ref int[] faceEdgeIndex)
        {
            if (boundaryPolyLine.NumberOfCoordinates<=0)
            {
                return true;
            }

            var newMesh2D = new Mesh2DNative();

            MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);

            var edgeNodesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges * 2);
            var edgeIndexPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges);
            var edgeDistancesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * newMesh2D.num_edges);
            var segmentDistancesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * newMesh2D.num_edges);
            var segmentIndexesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges);

            var faceIndexesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);
            var faceNumEdgesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);
            var faceEdgeIndexPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);
            

            var boundaryPolyLineNative = boundaryPolyLine.CreateNativeObject();
            bool successful =  MeshKernelDll.Mesh2dIntersectionsFromPolyline(meshKernelId, 
                                                                             ref boundaryPolyLineNative,
                                                                             edgeNodesPtr,
                                                                             edgeIndexPtr,
                                                                             edgeDistancesPtr,
                                                                             segmentDistancesPtr,
                                                                             segmentIndexesPtr,
                                                                             faceIndexesPtr,
                                                                             faceNumEdgesPtr,
                                                                             faceEdgeIndexPtr) == 0;

            edgeNodes = new int[newMesh2D.num_edges * 2];
            Marshal.Copy(edgeNodesPtr, edgeNodes, 0, edgeNodes.Length);

            edgeIndex = new int[newMesh2D.num_edges];
            Marshal.Copy(edgeIndexPtr, edgeIndex, 0, edgeIndex.Length);

            edgeDistances = new double[newMesh2D.num_edges];
            Marshal.Copy(edgeDistancesPtr, edgeDistances, 0, edgeDistances.Length);

            segmentDistances = new double[newMesh2D.num_edges];
            Marshal.Copy(segmentDistancesPtr, segmentDistances, 0, segmentDistances.Length);

            segmentIndexes = new int[newMesh2D.num_edges];
            Marshal.Copy(segmentIndexesPtr, segmentIndexes, 0, segmentIndexes.Length);

            faceIndexes = new int[newMesh2D.num_faces];
            Marshal.Copy(faceIndexesPtr, faceIndexes, 0, faceIndexes.Length);

            faceNumEdges = new int[newMesh2D.num_faces];
            Marshal.Copy(faceNumEdgesPtr, faceNumEdges, 0, faceNumEdges.Length);

            faceEdgeIndex = new int[newMesh2D.num_faces];
            Marshal.Copy(faceEdgeIndexPtr, faceEdgeIndex, 0, faceEdgeIndex.Length);
            
            
            Marshal.FreeCoTaskMem(edgeNodesPtr);
            Marshal.FreeCoTaskMem(edgeIndexPtr);
            Marshal.FreeCoTaskMem(edgeDistancesPtr);
            Marshal.FreeCoTaskMem(segmentDistancesPtr);
            Marshal.FreeCoTaskMem(segmentIndexesPtr);
            Marshal.FreeCoTaskMem(faceIndexesPtr);
            Marshal.FreeCoTaskMem(faceNumEdgesPtr);
            Marshal.FreeCoTaskMem(faceEdgeIndexPtr);

            return successful;
        }

        public bool Mesh2dMakeMeshFromPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeMeshFromPolygon(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeMeshFromSamples(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeMeshFromSamples(meshKernelId, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeUniform(int meshKernelId, in MakeGridParameters makeGridParameters, in DisposableGeometryList geometryList)
        {
            var geometryListNative = geometryList.CreateNativeObject();
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeUniform(meshKernelId, ref makeGridParametersNative, ref geometryListNative) == 0;
        }

        public bool Mesh2dMakeUniformOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeUniformOnExtension(meshKernelId, ref makeGridParametersNative) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dMergeNodes(int meshKernelId, in DisposableGeometryList disposableGeometryList, double mergingDistance)
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
                                                      in DisposableGriddedSamples griddedSamples, 
                                                      in MeshRefinementParameters meshRefinementParameters, 
                                                      bool useNodalRefinement)
        {
            var griddedSamplesNative = griddedSamples.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();

            return MeshKernelDll.Mesh2dRefineBasedOnGriddedSamples(meshKernelId, 
                                                                   ref griddedSamplesNative,
                                                                   ref meshRefinementParametersNative, 
                                                                   useNodalRefinement) == 0;
        }

        public bool Mesh2dRefineBasedOnPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, in MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref meshRefinementParametersNative) == 0;
        }

        public bool Mesh2dRefineBasedOnSamples(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, double relativeSearchRadius, int minimumNumSamples, in MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, relativeSearchRadius, minimumNumSamples, ref meshRefinementParametersNative) == 0;
        }

        /// <inheritdoc />
        public bool Mesh2dSet(int meshKernelId, in DisposableMesh2D disposableMesh2D)
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

        public bool Mesh2dTriangulationInterpolation(int meshKernelId, in DisposableGeometryList samples, int locationType, ref DisposableGeometryList results)
        {
            if (samples.NumberOfCoordinates <= 0)
            {
                return true;
            }
            var samplesNative = samples.CreateNativeObject();
            var resultsNative = results.CreateNativeObject();
            return MeshKernelDll.Mesh2dTriangulationInterpolation(meshKernelId, ref samplesNative, locationType, ref  resultsNative) == 0;
        }

        public bool Network1dComputeFixedChainages(int meshKernelId, in double[] fixedChainages, double minFaceSize, double fixedChainagesOffset)
        {

            int numFixedChainages = fixedChainages.Length;
            IntPtr fixedChainagesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * numFixedChainages);
            Marshal.Copy(fixedChainages, 0, fixedChainagesPtr, numFixedChainages);
            bool succcess =  MeshKernelDll.Network1dComputeFixedChainages(meshKernelId, fixedChainagesPtr, numFixedChainages, minFaceSize, fixedChainagesOffset)==0;
            Marshal.FreeCoTaskMem(fixedChainagesPtr);
            return succcess;
        }

        public bool Network1dComputeOffsettedChainages(int meshKernelId, double offset)
        {
            return MeshKernelDll.Network1dComputeOffsettedChainages(meshKernelId, offset) == 0;
        }

        public bool Network1dSet(int meshKernelId, in DisposableGeometryList offset)
        {
            var offsetNative = offset.CreateNativeObject();
            return MeshKernelDll.Network1dSet(meshKernelId, ref offsetNative) == 0;
        }

        public bool Network1dToMesh1d(int meshKernelId, double minFaceSize)
        {
            return MeshKernelDll.Network1dToMesh1d(meshKernelId, minFaceSize) == 0;
        }
        
        public bool PolygonCountOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool PolygonCountRefine(int meshKernelId, in DisposableGeometryList disposableGeometryList, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.PolygonCountRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices) == 0;
        }

        public bool GetPointsInPolygon(int meshKernelId, in DisposableGeometryList inputPolygon, in DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateNativeObject();
            var geometryListNativeinputPoints = inputPoints.CreateNativeObject();
            var geometryListNativeSelectedPoints = selectedPoints.CreateNativeObject();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints) == 0;
        }

        public bool PolygonGetOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut) == 0;
        }

        public bool PolygonRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject(); // Create an instance for the out parameter
            
            bool success = MeshKernelDll.PolygonRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut) == 0;

            return success;
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

        private DisposableMesh1D CreateDisposableMesh1d(Mesh1DNative newMesh1DNative) 
        {
            var disposableMesh1D = new DisposableMesh1D
            {
                EdgeNodes = newMesh1DNative.edge_nodes.CreateValueArray<int>(newMesh1DNative.num_edges),
                NodeX = newMesh1DNative.node_x.CreateValueArray<double>(newMesh1DNative.num_nodes),
                NodeY = newMesh1DNative.node_y.CreateValueArray<double>(newMesh1DNative.num_nodes),
                NumNodes = newMesh1DNative.num_nodes,
                NumEdges = newMesh1DNative.num_edges,
            };
            return disposableMesh1D;
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