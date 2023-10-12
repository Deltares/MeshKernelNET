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
    // DotCover on the build server does not work correctly with remoting
    public sealed class MeshKernelApi : IMeshKernelApi
    {
        /// <inheritdoc />
        public int AllocateState(int projectionType)
        {
            var meshKernelId = 0;
            MeshKernelDll.AllocateState(projectionType, ref meshKernelId);
            return meshKernelId;
        }

        public int ContactsComputeBoundary(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double searchRadius)
        {
            var polygonsNative = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeBoundary(meshKernelId, oneDNodeMask, ref polygonsNative, searchRadius);
        }

        /// <inheritdoc />
        public int ContactsComputeMultiple(int meshKernelId, in IntPtr oneDNodeMask)
        {
            return MeshKernelDll.ContactsComputeMultiple(meshKernelId, oneDNodeMask);
        }
        /// <inheritdoc />
        public int ContactsComputeSingle(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double projectionFactor)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeSingle(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon, projectionFactor);
        }
        /// <inheritdoc />
        public int ContactsComputeWithPoints(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList points)
        {
            var pointsNative = points.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPoints(meshKernelId, oneDNodeMask, ref pointsNative);
        }

        /// <inheritdoc />
        public int ContactsComputeWithPolygons(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons)
        {
            var geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPolygons(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon);
        }

        public int ContactsGetData(int meshKernelId, out DisposableContacts disposableContacts)
        {
            disposableContacts = new DisposableContacts();
            var contacts = disposableContacts.CreateNativeObject();
            var exitCode = MeshKernelDll.ContactsGetDimensions(meshKernelId, ref contacts);
            if (exitCode != 0)
            {
                return exitCode;
            }

            disposableContacts = new DisposableContacts(contacts.num_contacts);
            contacts = disposableContacts.CreateNativeObject();
            exitCode = MeshKernelDll.ContactsGetData(meshKernelId, ref contacts);
            if (exitCode != 0)
            {
                return exitCode;
            }

            disposableContacts = CreateDisposableContacts(contacts);

            return exitCode;
        }

        public int CurvilinearComputeTransfiniteFromSplines(int meshKernelId,
                                                             in DisposableGeometryList disposableGeometryListIn,
                                                             in CurvilinearParameters curvilinearParameters)
        {
            var geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative);
        }

        public int CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId,
                                                                in DisposableGeometryList disposableGeometryListIn,
                                                                in CurvilinearParameters curvilinearParameters,
                                                                in SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative);
        }

        public int CurvilinearComputeTransfiniteFromPolygon(int meshKernelId,
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
                useFourthSide);
        }

        public int CurvilinearComputeTransfiniteFromTriangle(int meshKernelId,
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
                thirdNode);
        }

        public int CurvilinearConvertToMesh2D(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearConvertToMesh2D(meshKernelId);
        }

        public int CurvilinearDeleteNode(int meshKernelId, double xPointCoordinate, double yPointCoordinate)
        {
            return MeshKernelDll.CurvilinearDeleteNode(meshKernelId, xPointCoordinate, yPointCoordinate);
        }

        public int CurvilinearDeleteOrthogonalGridFromSplines(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearDeleteOrthogonalGridFromSplines(meshKernelId);
        }

        public int CurvilinearDerefine(int meshKernelId,
                                        double xLowerLeftCorner,
                                        double yLowerLeftCorner,
                                        double xUpperRightCorner,
                                        double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearDerefine(meshKernelId,
                                                     xLowerLeftCorner,
                                                     yLowerLeftCorner,
                                                     xUpperRightCorner,
                                                     yUpperRightCorner);
        }

        public int CurvilinearFinalizeLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearFinalizeLineShift(meshKernelId);
        }

        public int CurvilinearFinalizeOrthogonalize(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearFinalizeOrthogonalize(meshKernelId);
        }

        public int CurvilinearGridGetData(int meshKernelId, out DisposableCurvilinearGrid disposableCurvilinearGrid)
        {
            var curvilinearGrid = new CurvilinearGridNative();

            var exitCode = MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid);
            if (exitCode != 0)
            {
                disposableCurvilinearGrid = new DisposableCurvilinearGrid();
                return exitCode;
            }

            disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_m, curvilinearGrid.num_n);
            curvilinearGrid = disposableCurvilinearGrid.CreateNativeObject();

            exitCode = MeshKernelDll.CurvilinearGetData(meshKernelId, ref curvilinearGrid);
            if (exitCode != 0)
            {
                disposableCurvilinearGrid = new DisposableCurvilinearGrid();
                return exitCode;
            }

            disposableCurvilinearGrid = CreateDisposableCurvilinearGrid(curvilinearGrid);
            return exitCode;
        }

        public int CurvilinearInitializeLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearInitializeLineShift(meshKernelId);
        }

        public int CurvilinearInitializeOrthogonalGridFromSplines(int meshKernelId,
                                                                   in DisposableGeometryList disposableGeometryListIn,
                                                                   in CurvilinearParameters curvilinearParameters,
                                                                   in SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearInitializeOrthogonalGridFromSplines(meshKernelId,
                ref geometryListNativeIn,
                ref curvilinearParametersNative,
                ref splinesToCurvilinearParametersNative);
        }

        public int CurvilinearInitializeOrthogonalize(int meshKernelId,
                                                       in OrthogonalizationParameters orthogonalizationParameters)
        {
            var orthogonalizationParametersNative = orthogonalizationParameters.ToOrthogonalizationParametersNative();
            return MeshKernelDll.CurvilinearInitializeOrthogonalize(meshKernelId, ref orthogonalizationParametersNative);
        }

        public int CurvilinearInsertFace(int meshKernelId, double xCoordinate, double yCoordinate)
        {
            return MeshKernelDll.CurvilinearInsertFace(meshKernelId, xCoordinate, yCoordinate);
        }

        public int CurvilinearIterateOrthogonalGridFromSplines(int meshKernelId, int layer)
        {
            return MeshKernelDll.CurvilinearIterateOrthogonalGridFromSplines(meshKernelId, layer);
        }

        public int CurvilinearLineAttractionRepulsion(int meshKernelId,
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
                                                                    yUpperRightCorner);
        }

        public int CurvilinearLineMirror(int meshKernelId,
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
                                                       ySecondGridLineNode);
        }

        public int CurvilinearLineShift(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearLineShift(meshKernelId);
        }

        public int CurvilinearComputeRectangularGrid(int meshKernelId,
                                           in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.CurvilinearComputeRectangularGrid(meshKernelId, ref makeGridParametersNative);
        }

        public int CurvilinearComputeRectangularGridFromPolygon(int meshKernelId,
            in MakeGridParameters makeGridParameters,
            in DisposableGeometryList disposableGeometryListIn)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            var geometryListNative = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.CurvilinearComputeRectangularGridFromPolygon(meshKernelId, 
                                                                    ref makeGridParametersNative,
                                                                    ref geometryListNative);
        }

        public int CurvilinearComputeRectangularGridOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.CurvilinearComputeRectangularGridOnExtension(meshKernelId, ref makeGridParametersNative);
        }

        public int CurvilinearMoveNode(int meshKernelId, double xFromPoint, double yFromPoint, double xToPoint, double yToPoint)
        {
            return MeshKernelDll.CurvilinearMoveNode(meshKernelId, xFromPoint, yFromPoint, xToPoint, yToPoint);
        }

        public int CurvilinearMoveNodeLineShift(int meshKernelId,
                                                 double xFromCoordinate,
                                                 double yFromCoordinate,
                                                 double xToCoordinate,
                                                 double yToCoordinate)
        {
            return MeshKernelDll.CurvilinearMoveNodeLineShift(meshKernelId, xFromCoordinate, yFromCoordinate, xToCoordinate, yToCoordinate);
        }

        public int CurvilinearOrthogonalize(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearOrthogonalize(meshKernelId);
        }

        public int CurvilinearRefine(int meshKernelId,
                                      double xLowerLeftCorner,
                                      double yLowerLeftCorner,
                                      double xUpperRightCorner,
                                      double yUpperRightCorner,
                                      int refinement)
        {
            return MeshKernelDll.CurvilinearRefine(meshKernelId, xLowerLeftCorner, yLowerLeftCorner, xUpperRightCorner, yUpperRightCorner, refinement);
        }

        public int CurvilinearRefreshOrthogonalGridFromSplines(int meshKernelId)
        {
            return MeshKernelDll.CurvilinearRefreshOrthogonalGridFromSplines(meshKernelId);
        }

        public int CurvilinearSet(int meshKernelId, in DisposableCurvilinearGrid grid)
        {
            var gridNative = grid.CreateNativeObject();
            return MeshKernelDll.CurvilinearSet(meshKernelId, ref gridNative);
        }

        public int CurvilinearSetBlockLineShift(int meshKernelId,
                                                 double xLowerLeftCorner,
                                                 double yLowerLeftCorner,
                                                 double xUpperRightCorner,
                                                 double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetBlockLineShift(meshKernelId,
                xLowerLeftCorner,
                yLowerLeftCorner,
                xUpperRightCorner,
                yUpperRightCorner);
        }

        public int CurvilinearSetBlockOrthogonalize(int meshKernelId,
            double xLowerLeftCorner,
            double yLowerLeftCorner,
            double xUpperRightCorner,
            double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetBlockOrthogonalize(meshKernelId,
                xLowerLeftCorner,
                yLowerLeftCorner,
                xUpperRightCorner,
                yUpperRightCorner);
        }

        public int CurvilinearSetFrozenLinesOrthogonalize(int meshKernelId,
                                                           double xFirstGridLineNode,
                                                           double yFirstGridLineNode,
                                                           double xSecondGridLineNode,
                                                           double yUpperRightCorner)
        {
            return MeshKernelDll.CurvilinearSetFrozenLinesOrthogonalize(meshKernelId,
                                                                        xFirstGridLineNode,
                                                                        yFirstGridLineNode,
                                                                        xSecondGridLineNode,
                                                                        yUpperRightCorner);
        }

        public int CurvilinearSetLineLineShift(int meshKernelId,
                                                double xFirstGridLineNode,
                                                double yFirstGridLineNode,
                                                double xSecondGridLineNode,
                                                double ySecondGridLineNode)
        {
            return MeshKernelDll.CurvilinearSetLineLineShift(meshKernelId,
                                                             xFirstGridLineNode,
                                                             yFirstGridLineNode,
                                                             xSecondGridLineNode,
                                                             ySecondGridLineNode);
        }

        public int CurvilinearSmoothing(int meshKernelId,
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
                                                      yUpperRightCorner);
        }

        public int CurvilinearSmoothingDirectional(int meshKernelId,
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
                                                                 yUpperRightCornerSmoothingArea);
        }

        public int DeallocateState(int meshKernelId)
        {
            return MeshKernelDll.DeallocateState(meshKernelId);
        }

        public int GetAveragingMethodClosestPoint(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodClosestPoint(ref method);
        }

        public int GetAveragingMethodInverseDistanceWeighting(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodInverseDistanceWeighting(ref method);
        }

        public int GetAveragingMethodMax(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMax(ref method);
        }

        public int GetAveragingMethodMin(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMin(ref method);
        }

        public int GetAveragingMethodMinAbsoluteValue(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodMinAbsoluteValue(ref method);
        }

        public int GetAveragingMethodSimpleAveraging(ref int method)
        {
            return MeshKernelDll.GetAveragingMethodSimpleAveraging(ref method);
        }

        public int GetEdgesLocationType(ref int type)
        {
            return MeshKernelDll.GetEdgesLocationType(ref type);
        }

        public int GetError(out string errorMessage)
        {
            var bufferSize = 512;
            var errorMessagePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            var exitCode = MeshKernelDll.GetError(errorMessagePtr);
            if (exitCode != 0)
            {
                errorMessage = "";
                return exitCode;
            }
            errorMessage = Marshal.PtrToStringAnsi(errorMessagePtr);
            Marshal.FreeCoTaskMem(errorMessagePtr);

            return exitCode;
        }

        public int GetExitCodeSuccess(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeSuccess(ref exitCode);
        }

        public int GetExitCodeMeshKernelError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeMeshKernelError(ref exitCode);
        }

        public int GetExitCodeNotImplementedError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeNotImplementedError(ref exitCode);
        }

        public int GetExitCodeAlgorithmError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeAlgorithmError(ref exitCode);
        }

        public int GetExitCodeConstraintError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeConstraintError(ref exitCode);
        }

        public int GetExitCodeMeshGeometryError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeMeshGeometryError(ref exitCode);
        }

        public int GetExitCodeLinearAlgebraError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeLinearAlgebraError(ref exitCode);
        }

        public int GetExitCodeRangeError(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeRangeError(ref exitCode);
        }

        public int GetExitCodeStdLibException(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeStdLibException(ref exitCode);
        }

        public int GetExitCodeUnknownException(ref int exitCode)
        {
            return MeshKernelDll.GetExitCodeUnknownException(ref exitCode);
        }

        public int GetFacesLocationType(ref int type)
        {
            return MeshKernelDll.GetFacesLocationType(ref type);
        }

        public int GetGeometryError(ref int invalidIndex, ref int type)
        {
            return MeshKernelDll.GetGeometryError(ref invalidIndex, ref type);
        }

        public int GetNodesLocationType(ref int type)
        {
            return MeshKernelDll.GetNodesLocationType(ref type);
        }

        public int GetProjection(int meshKernelId, ref int projection)
        {
            return MeshKernelDll.GetProjection(meshKernelId, ref projection);
        }

        public int GetProjectionCartesian(ref int projection)
        {
            return MeshKernelDll.GetProjectionCartesian(ref projection);
        }

        public int GetProjectionSpherical(ref int projection)
        {
            return MeshKernelDll.GetProjectionSpherical(ref projection);
        }

        public int GetProjectionSphericalAccurate(ref int projection)
        {
            return MeshKernelDll.GetProjectionSphericalAccurate(ref projection);
        }

        public int GetSplines(in DisposableGeometryList disposableGeometryListIn, ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices)
        {
            var nativeGeometryListIn = disposableGeometryListIn.CreateNativeObject();
            var nativeGeometryListOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.GetSplines(ref nativeGeometryListIn, ref nativeGeometryListOut, numberOfPointsBetweenVertices);
        }

        public int GetVersion(out string version)
        {
            var bufferSize = 64;
            version = "";
            var versionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            var exitCode = MeshKernelDll.GetVersion(versionPtr);
            if (exitCode != 0)
            {
                return exitCode;
            }
            version = Marshal.PtrToStringAnsi(versionPtr);
            Marshal.FreeCoTaskMem(versionPtr);

            return exitCode;
        }

        public int Mesh1dGetData(int meshKernelId, out DisposableMesh1D disposableMesh1D)
        {
            var newMesh1D = new Mesh1DNative();

            var exitCode = MeshKernelDll.Mesh1dGetDimensions(meshKernelId, ref newMesh1D);

            if (exitCode != 0)
            {
                disposableMesh1D = new DisposableMesh1D();
                return exitCode;
            }

            disposableMesh1D = new DisposableMesh1D(newMesh1D.num_nodes, newMesh1D.num_edges);


            newMesh1D = disposableMesh1D.CreateNativeObject();

            exitCode = MeshKernelDll.Mesh1dGetData(meshKernelId, ref newMesh1D);
            disposableMesh1D = CreateDisposableMesh1d(newMesh1D);

            return exitCode;

        }

        public int Mesh1dSet(int meshKernelId, in DisposableMesh1D disposableMesh1D)
        {
            var mesh1D = disposableMesh1D.CreateNativeObject();
            return MeshKernelDll.Mesh1dSet(meshKernelId, ref mesh1D);
        }

        public int Mesh2dAveragingInterpolation(int meshKernelId,
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
                                                              ref resultsNative);
        }

        public int Mesh2dComputeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dComputeInnerOrtogonalizationIteration(meshKernelId);
        }

        public int Mesh2dComputeOrthogonalization(int meshKernelId,
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
                                                                ref geometryListLandBoundariesNative);
        }

        public int Mesh2dCountHangingEdges(int meshKernelId, ref int numEdges)
        {
            return MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numEdges);
        }

        public int Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices)
        {
            return MeshKernelDll.Mesh2dCountMeshBoundariesAsPolygons(meshKernelId, ref numberOfPolygonVertices);
        }

        public int Mesh2dCountSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesLengthThreshold, ref int numSmallFlowEdges)
        {
            return MeshKernelDll.Mesh2dCountSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges);
        }
        public int Mesh2dDelete(int meshKernelId,
            in DisposableGeometryList disposableGeometryListOut,
            DeleteMeshOptions deletionOption,
            bool invertDeletion)
        {
            var geometryListNativeIn = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dDelete(meshKernelId,
                ref geometryListNativeIn,
                Convert.ToInt32(deletionOption),
                invertDeletion);
        }
        public int Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox)
        {
            return MeshKernelDll.Mesh2dDeleteEdge(meshKernelId, xCoordinate, yCoordinate, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox);
        }

        public int Mesh2dDeleteHangingEdges(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteHangingEdges(meshKernelId);
        }

        /// <inheritdoc />
        public int Mesh2dDeleteNode(int meshKernelId, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dDeleteNode(meshKernelId, vertexIndex);
        }
        public int Mesh2dDeleteOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteOrthogonalization(meshKernelId);
        }

        public int Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(int meshKernelId,
                                                                double smallFlowEdgesThreshold,
                                                                double minFractionalAreaTriangles)
        {
            return MeshKernelDll.Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(meshKernelId, smallFlowEdgesThreshold, minFractionalAreaTriangles);
        }
        public int Mesh2dFinalizeInnerOrtogonalizationIteration(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dFinalizeInnerOrtogonalizationIteration(meshKernelId);
        }

        public int Mesh2dFlipEdges(int meshKernelId, bool isTriangulationRequired, ProjectToLandBoundaryOptions projectToLandBoundaryOption, in DisposableGeometryList selectingPolygon, in DisposableGeometryList landBoundaries)
        {
            int isTriangulationRequiredInt = isTriangulationRequired ? 1 : 0;
            int projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            var geometryListPolygon = selectingPolygon?.CreateNativeObject() ??
                                      new GeometryListNative { numberOfCoordinates = 0 };
            var geometryListLandBoundaries = landBoundaries?.CreateNativeObject() ??
                                             new GeometryListNative { numberOfCoordinates = 0 };
            return MeshKernelDll.Mesh2dFlipEdges(meshKernelId, isTriangulationRequiredInt, projectToLandBoundaryOptionInt, ref geometryListPolygon, ref geometryListLandBoundaries);
        }

        public int Mesh2dGetClosestNode(int meshKernelId,
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
                                                      ref yCoordinateOut);
        }
        public int Mesh2dGetData(int meshKernelId, out DisposableMesh2D disposableMesh2D)
        {

            var newMesh2D = new Mesh2DNative();

            var exitCode = MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);

            if (exitCode != 0)
            {
                disposableMesh2D = new DisposableMesh2D();
                return exitCode;
            }
            disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes,
                                                    newMesh2D.num_edges,
                                                    newMesh2D.num_faces,
                                                    newMesh2D.num_face_nodes);

            newMesh2D = disposableMesh2D.CreateNativeObject();

            exitCode = MeshKernelDll.Mesh2dGetData(meshKernelId, ref newMesh2D);
            disposableMesh2D = CreateDisposableMesh2D(newMesh2D, true);

            return exitCode;
        }

        public int Mesh2dGetEdge(int meshKernelId,
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
                                               ref edgeIndex);
        }

        public int Mesh2dGetHangingEdges(int meshKernelId, out int[] hangingEdges)
        {
            int numberOfHangingEdges = -1;
            hangingEdges = new int[] { };
            var exitCode = MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numberOfHangingEdges);
            if (exitCode != 0)
            {
                return exitCode;
            }

            if (numberOfHangingEdges <= 0)
            {
                return 0;
            }

            IntPtr hangingVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfHangingEdges);
            exitCode = MeshKernelDll.Mesh2dGetHangingEdges(meshKernelId, hangingVerticesPtr);
            if (exitCode != 0)
            {
                return exitCode;
            }

            hangingEdges = new int[numberOfHangingEdges];

            Marshal.Copy(hangingVerticesPtr, hangingEdges, 0, numberOfHangingEdges);

            Marshal.FreeCoTaskMem(hangingVerticesPtr);

            return exitCode;
        }

        public int Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref geometryListNative);
        }

        public int Mesh2dGetNodeIndex(int meshKernelId,
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
                                                    ref vertexIndex);
        }

        public int GetSelectedVerticesInPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int inside, ref int[] selectedVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };

            int numberOfMeshVertices = -1;
            var exitCode = MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            if (exitCode != 0)
            {
                return exitCode;
            }

            var selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
            exitCode = MeshKernelDll.GetSelectedVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, selectedVerticesPtr);
            if (exitCode != 0)
            {
                Marshal.FreeCoTaskMem(selectedVerticesPtr);
                return exitCode;
            }

            selectedVertices = new int[numberOfMeshVertices];
            Marshal.Copy(selectedVerticesPtr, selectedVertices, 0, numberOfMeshVertices);
            Marshal.FreeCoTaskMem(selectedVerticesPtr);
            return exitCode;
        }

        public int Mesh2dGetObtuseTrianglesMassCenters(int meshKernelId, ref DisposableGeometryList disposableGeometryList)
        {
            var disposableGeometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetObtuseTrianglesMassCenters(meshKernelId, ref disposableGeometryListNative);
        }

        public int Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetOrthogonality(meshKernelId, ref geometryListNativeIn);
        }

        public int Mesh2dCountObtuseTriangles(int meshKernelId, ref int numObtuseTriangles)
        {
            return MeshKernelDll.Mesh2dCountObtuseTriangles(meshKernelId, ref numObtuseTriangles);
        }

        public int Mesh2dGetSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesThreshold, ref DisposableGeometryList disposableGeometryList)
        {
            var geometryListNativeIn = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesThreshold, ref geometryListNativeIn);
        }

        public int Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmoothness(meshKernelId, ref geometryListNativeInOut);
        }

        public int Mesh2dInitializeOrthogonalization(int meshKernelId,
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
                ref nativeOrthogonalizationParameters, ref geometryListPolygon, ref geometryListLandBoundaries);
        }

        /// <inheritdoc />
        public int Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dInsertEdge(meshKernelId, startVertexIndex, endVertexIndex, ref edgeIndex);
        }

        public int Mesh2dInsertNode(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex)
        {
            return MeshKernelDll.Mesh2dInsertNode(meshKernelId, xCoordinate, yCoordinate, ref vertexIndex);
        }

        public int Mesh2dIntersectionsFromPolygon(int meshKernelId,
                                                  in DisposableGeometryList boundaryPolygon,
                                                  ref int[] edgeNodes,
                                                  ref int[] edgeIndex,
                                                  ref double[] edgeDistances,
                                                  ref double[] segmentDistances,
                                                  ref int[] segmentIndexes,
                                                  ref int[] faceIndexes,
                                                  ref int[] faceNumEdges,
                                                  ref int[] faceEdgeIndex)
        {
            if (boundaryPolygon.NumberOfCoordinates <= 0)
            {
                return 0;
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


            var boundaryPolygoneNative = boundaryPolygon.CreateNativeObject();
            int successful = MeshKernelDll.Mesh2dIntersectionsFromPolygon(meshKernelId,
                                                                          ref boundaryPolygoneNative,
                                                                          edgeNodesPtr,
                                                                          edgeIndexPtr,
                                                                          edgeDistancesPtr,
                                                                          segmentDistancesPtr,
                                                                          segmentIndexesPtr,
                                                                          faceIndexesPtr,
                                                                          faceNumEdgesPtr,
                                                                          faceEdgeIndexPtr);

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

        public int Mesh2dMakeTriangularMeshFromPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeTriangularMeshFromPolygon(meshKernelId, ref geometryListNative);
        }

        public int Mesh2dMakeTriangularMeshFromSamples(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            var geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeTriangularMeshFromSamples(meshKernelId, ref geometryListNative);
        }

        public int Mesh2dMakeRectangularMesh(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMesh(meshKernelId, ref makeGridParametersNative);
        }

        public int Mesh2dMakeRectangularMeshFromPolygon(int meshKernelId, in MakeGridParameters makeGridParameters, in DisposableGeometryList geometryList)
        {
            var geometryListNative = geometryList.CreateNativeObject();
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMeshFromPolygon(meshKernelId, ref makeGridParametersNative, ref geometryListNative);
        }

        public int Mesh2dMakeRectangularMeshOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMeshOnExtension(meshKernelId, ref makeGridParametersNative);
        }

        /// <inheritdoc />
        public int Mesh2dMergeNodes(int meshKernelId, in DisposableGeometryList disposableGeometryList, double mergingDistance)
        {
            var geometryList = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMergeNodes(meshKernelId, ref geometryList, mergingDistance);
        }

        /// <inheritdoc />
        public int Mesh2dMergeTwoNodes(int meshKernelId, int startVertexIndex, int endVertexIndex)
        {
            return MeshKernelDll.Mesh2dMergeTwoNodes(meshKernelId, startVertexIndex, endVertexIndex);
        }

        public int Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex)
        {
            return MeshKernelDll.Mesh2dMoveNode(meshKernelId, xCoordinate, yCoordinate, vertexIndex);
        }

        public int Mesh2dPrepareOuterIterationOrthogonalization(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dPrepareOuterIterationOrthogonalization(meshKernelId);
        }

        public int Mesh2dRefineBasedOnGriddedSamples(int meshKernelId,
                                                      in DisposableGriddedSamples griddedSamples,
                                                      in MeshRefinementParameters meshRefinementParameters,
                                                      bool useNodalRefinement)
        {
            var griddedSamplesNative = griddedSamples.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();

            return MeshKernelDll.Mesh2dRefineBasedOnGriddedSamples(meshKernelId,
                                                                   ref griddedSamplesNative,
                                                                   ref meshRefinementParametersNative,
                                                                   useNodalRefinement);
        }

        public int Mesh2dRefineBasedOnPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, in MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref meshRefinementParametersNative);
        }

        public int Mesh2dRefineBasedOnSamples(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, double relativeSearchRadius, int minimumNumSamples, in MeshRefinementParameters meshRefinementParameters)
        {
            var disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, relativeSearchRadius, minimumNumSamples, ref meshRefinementParametersNative);
        }

        /// <inheritdoc />
        public int Mesh2dSet(int meshKernelId, in DisposableMesh2D disposableMesh2D)
        {
            var mesh2D = disposableMesh2D.CreateNativeObject();
            return MeshKernelDll.Mesh2dSet(meshKernelId, ref mesh2D);
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

        public int Mesh2dTriangulationInterpolation(int meshKernelId, in DisposableGeometryList samples, int locationType, ref DisposableGeometryList results)
        {
            if (samples.NumberOfCoordinates <= 0)
            {
                return 0;
            }
            var samplesNative = samples.CreateNativeObject();
            var resultsNative = results.CreateNativeObject();
            return MeshKernelDll.Mesh2dTriangulationInterpolation(meshKernelId, ref samplesNative, locationType, ref resultsNative);
        }

        public int Network1dComputeFixedChainages(int meshKernelId, in double[] fixedChainages, double minFaceSize, double fixedChainagesOffset)
        {

            int numFixedChainages = fixedChainages.Length;
            IntPtr fixedChainagesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * numFixedChainages);
            Marshal.Copy(fixedChainages, 0, fixedChainagesPtr, numFixedChainages);
            int succcess = MeshKernelDll.Network1dComputeFixedChainages(meshKernelId, fixedChainagesPtr, numFixedChainages, minFaceSize, fixedChainagesOffset);
            Marshal.FreeCoTaskMem(fixedChainagesPtr);
            return succcess;
        }

        public int Network1dComputeOffsettedChainages(int meshKernelId, double offset)
        {
            return MeshKernelDll.Network1dComputeOffsettedChainages(meshKernelId, offset);
        }

        public int Network1dSet(int meshKernelId, in DisposableGeometryList offset)
        {
            var offsetNative = offset.CreateNativeObject();
            return MeshKernelDll.Network1dSet(meshKernelId, ref offsetNative);
        }

        public int Network1dToMesh1d(int meshKernelId, double minFaceSize)
        {
            return MeshKernelDll.Network1dToMesh1d(meshKernelId, minFaceSize);
        }

        public int PolygonCountOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices);
        }

        public int PolygonCountRefine(int meshKernelId, in DisposableGeometryList disposableGeometryList, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            var geometryListInNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.PolygonCountRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices);
        }

        public int GetPointsInPolygon(int meshKernelId, in DisposableGeometryList inputPolygon, in DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            var geometryListNativeInputPolygon = inputPolygon.CreateNativeObject();
            var geometryListNativeinputPoints = inputPoints.CreateNativeObject();
            var geometryListNativeSelectedPoints = selectedPoints.CreateNativeObject();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints);
        }

        public int PolygonGetOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut);
        }

        public int PolygonRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
             int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            var geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var geometryListNativeOut = disposableGeometryListOut.CreateNativeObject(); // Create an instance for the out parameter
            return MeshKernelDll.PolygonRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut);

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