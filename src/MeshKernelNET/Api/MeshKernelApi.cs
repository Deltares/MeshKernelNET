using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using NetTopologySuite.Operation.Valid;

namespace MeshKernelNET.Api
{
    [ExcludeFromCodeCoverage]
    // Excluded because it is tested through the MeshKernelApiRemote
    // DotCover on the build server does not work correctly with remoting
    public sealed class MeshKernelApi : IMeshKernelApi
    {
        /// <inheritdoc/>
        public int AllocateState(int projectionType)
        {
            var meshKernelId = 0;
            MeshKernelDll.AllocateState(projectionType, ref meshKernelId);
            return meshKernelId;
        }

        /// <inheritdoc/>
        public int ClearUndoState(int meshKernelId)
        {
            return MeshKernelDll.ClearUndoState(meshKernelId);
        }

        public int ContactsComputeBoundary(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double searchRadius)
        {
            GeometryListNative polygonsNative = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeBoundary(meshKernelId, oneDNodeMask, ref polygonsNative, searchRadius);
        }

        /// <inheritdoc/>
        public int ContactsComputeMultiple(int meshKernelId, in IntPtr oneDNodeMask)
        {
            return MeshKernelDll.ContactsComputeMultiple(meshKernelId, oneDNodeMask);
        }

        /// <inheritdoc/>
        public int ContactsComputeSingle(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double projectionFactor)
        {
            GeometryListNative geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeSingle(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon, projectionFactor);
        }

        /// <inheritdoc/>
        public int ContactsComputeWithPoints(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList points)
        {
            GeometryListNative pointsNative = points.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPoints(meshKernelId, oneDNodeMask, ref pointsNative);
        }

        /// <inheritdoc/>
        public int ContactsComputeWithPolygons(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons)
        {
            GeometryListNative geometryListNativeInputPolygon = polygons.CreateNativeObject();
            return MeshKernelDll.ContactsComputeWithPolygons(meshKernelId, oneDNodeMask, ref geometryListNativeInputPolygon);
        }

        public int ContactsGetData(int meshKernelId, out DisposableContacts disposableContacts)
        {
            disposableContacts = new DisposableContacts();
            ContactsNative contacts = disposableContacts.CreateNativeObject();
            int exitCode = MeshKernelDll.ContactsGetDimensions(meshKernelId, ref contacts);
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

        public int Mesh2dSnapToLandBoundary(int meshKernelId, in DisposableGeometryList selectingPolygon, in DisposableGeometryList landBoundaries)
        {
            var selectingPolygonNative = selectingPolygon.CreateNativeObject();
            var landBoundariesNative = landBoundaries.CreateNativeObject();
            return MeshKernelDll.Mesh2dSnapToLandBoundary(meshKernelId,  ref selectingPolygonNative,  ref landBoundariesNative);
        }

        public int CurvilinearComputeTransfiniteFromSplines(int meshKernelId,
                                                            in DisposableGeometryList disposableGeometryListIn,
                                                            in CurvilinearParameters curvilinearParameters)
        {
            GeometryListNative geometryListIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeTransfiniteFromSplines(meshKernelId, ref geometryListIn, ref curvilinearParametersNative);
        }

        public int CurvilinearComputeCurvature(int meshKernelId, CurvilinearDirectionOptions direction, ref double[] curvature)
        {
            IntPtr curvaturePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * curvature.Length);
            int success = MeshKernelDll.CurvilinearComputeSmoothness(meshKernelId, Convert.ToInt32(direction), curvaturePtr);
            Marshal.Copy(curvaturePtr, curvature, 0, curvature.Length);
            Marshal.FreeCoTaskMem(curvaturePtr);
            return success;
        }

        public int CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId,
                                                               in DisposableGeometryList disposableGeometryListIn,
                                                               in CurvilinearParameters curvilinearParameters,
                                                               in SplinesToCurvilinearParameters splinesToCurvilinearParameters)
        {
            GeometryListNative geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeOrthogonalGridFromSplines(meshKernelId, ref geometryListNative,
                                                                             ref curvilinearParametersNative, ref splinesToCurvilinearParametersNative);
        }

        public int CurvilinearComputeGridFromSplines(int meshKernelId,
                                                     in DisposableGeometryList disposableGeometryListIn,
                                                     in CurvilinearParameters curvilinearParameters)
        {
            GeometryListNative geometryListNative = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearComputeFromSplines(meshKernelId, ref geometryListNative, ref curvilinearParametersNative);
        }

        
        public int CurvilinearComputeSmoothness(int meshKernelId, CurvilinearDirectionOptions direction, ref double[] smoothness)
        {
            IntPtr smoothnessPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * smoothness.Length);
            int success = MeshKernelDll.CurvilinearComputeSmoothness(meshKernelId, Convert.ToInt32(direction), smoothnessPtr);
            Marshal.Copy(smoothnessPtr, smoothness, 0, smoothness.Length);
            Marshal.FreeCoTaskMem(smoothnessPtr);
            return success;
        }

        public int CurvilinearComputeTransfiniteFromPolygon(int meshKernelId,
                                                            in DisposableGeometryList geometryList,
                                                            int firstNode,
                                                            int secondNode,
                                                            int thirdNode,
                                                            bool useFourthSide)
        {
            GeometryListNative geometryListNative = geometryList.CreateNativeObject();
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
            GeometryListNative geometryListNative = geometryList.CreateNativeObject();
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

        public int CurvilinearDeleteExterior(int meshKernelId, BoundingBox boundingBox)
        {
            BoundingBoxNative boundingBoxNative = boundingBox.ToBoundingBoxNative();
            return MeshKernelDll.CurvilinearDeleteExterior(meshKernelId, ref boundingBoxNative);
        }

        public int CurvilinearDeleteInterior(int meshKernelId,
                                             BoundingBox boundingBox)
        {
            BoundingBoxNative boundingBoxNative = boundingBox.ToBoundingBoxNative();
            return MeshKernelDll.CurvilinearDeleteInterior(meshKernelId, ref boundingBoxNative);
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

        public int CurvilinearGridGetData(int meshKernelId, out DisposableCurvilinearGrid disposableCurvilinearGrid)
        {
            var curvilinearGrid = new CurvilinearGridNative();

            int exitCode = MeshKernelDll.CurvilinearGetDimensions(meshKernelId, ref curvilinearGrid);
            if (exitCode != 0)
            {
                disposableCurvilinearGrid = new DisposableCurvilinearGrid();
                return exitCode;
            }

            disposableCurvilinearGrid = new DisposableCurvilinearGrid(curvilinearGrid.num_n, curvilinearGrid.num_m);
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

        public int CurvilinearGetBoundariesAsPolygons(int meshKernelId, int lowerLeftN, int lowerLeftM, int upperRightN,  int upperRightM, out DisposableGeometryList boundaryPolygons)
        {
            int numberOfPolygonNodes = 0; 
            boundaryPolygons = new DisposableGeometryList();
            int exitCode = MeshKernelDll.CurvilinearCountGetBoundariesAsPolygons(meshKernelId, 
                                                                                 lowerLeftN, 
                                                                                 lowerLeftM, 
                                                                                 upperRightN, 
                                                                                 upperRightM, 
                                                                                 ref numberOfPolygonNodes);
            if (exitCode != 0)
            {
                return exitCode;
            }

            using(var exchangePolygons = new DisposableGeometryList())
            {
                exchangePolygons.NumberOfCoordinates = numberOfPolygonNodes;
                exchangePolygons.XCoordinates = new double[numberOfPolygonNodes];
                exchangePolygons.YCoordinates = new double[numberOfPolygonNodes];
                exchangePolygons.GeometrySeparator = GetSeparator();
                exchangePolygons.InnerOuterSeparator = GetInnerOuterSeparator();

                var geometryListNative = exchangePolygons.CreateNativeObject();
                exitCode = MeshKernelDll.CurvilinearGetBoundariesAsPolygons(meshKernelId,
                                                                            lowerLeftN,
                                                                            lowerLeftM,
                                                                            upperRightN,
                                                                            upperRightM,
                                                                            ref geometryListNative);

                if (exitCode != 0)
                {
                    return exitCode;
                }

                boundaryPolygons.NumberOfCoordinates = exchangePolygons.NumberOfCoordinates;
                boundaryPolygons.XCoordinates = geometryListNative.xCoordinates.CreateValueArray<double>(numberOfPolygonNodes);
                boundaryPolygons.YCoordinates = geometryListNative.yCoordinates.CreateValueArray<double>(numberOfPolygonNodes);
                boundaryPolygons.GeometrySeparator = exchangePolygons.GeometrySeparator;
                boundaryPolygons.InnerOuterSeparator = exchangePolygons.InnerOuterSeparator;
            }
            return exitCode;
        }

        public int CurvilinearGetEdgeLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetEdgesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.CurvilinearGetLocationIndex(meshKernelId,
                                                                 xCoordinate,
                                                                 yCoordinate,
                                                                 locationType,
                                                                 ref boundingBoxNative,
                                                                 ref locationIndex);
            return exitCode;
        }
        public int CurvilinearGetFaceLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetFacesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }

            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.CurvilinearGetLocationIndex(meshKernelId,
                                                                 xCoordinate,
                                                                 yCoordinate,
                                                                 locationType,
                                                                 ref boundingBoxNative,
                                                                 ref locationIndex);
            return exitCode;
        }

        public int CurvilinearGetNodeLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetNodesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.CurvilinearGetLocationIndex(meshKernelId,
                                                                 xCoordinate,
                                                                 yCoordinate,
                                                                 locationType,
                                                                 ref boundingBoxNative,
                                                                 ref locationIndex);
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
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            var curvilinearParametersNative = curvilinearParameters.ToCurvilinearParametersNative();
            var splinesToCurvilinearParametersNative = splinesToCurvilinearParameters.ToSplinesToCurvilinearParametersNative();
            return MeshKernelDll.CurvilinearInitializeOrthogonalGridFromSplines(meshKernelId,
                                                                                ref geometryListNativeIn,
                                                                                ref curvilinearParametersNative,
                                                                                ref splinesToCurvilinearParametersNative);
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
                                         int numRowsToGenerate,
                                         double xFirstGridLineNode,
                                         double yFirstGridLineNode,
                                         double xSecondGridLineNode,
                                         double ySecondGridLineNode)
        {
            return MeshKernelDll.CurvilinearLineMirror(meshKernelId,
                                                       mirroringFactor,
                                                       numRowsToGenerate,
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
            GeometryListNative geometryListNative = disposableGeometryListIn.CreateNativeObject();
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

        public int CurvilinearOrthogonalize(int meshKernelId,
                                            ref OrthogonalizationParameters orthogonalizationParameters, 
                                            double xLowerLeftCorner,
                                            double yLowerLeftCorner,
                                            double xUpperRightCorner,
                                            double yUpperRightCorner)
        {
            var orthogonalizationParametersNative = orthogonalizationParameters.ToOrthogonalizationParametersNative();
            return MeshKernelDll.CurvilinearOrthogonalize(meshKernelId, 
                                                          ref orthogonalizationParametersNative,
                                                          xLowerLeftCorner,
                                                          yLowerLeftCorner,
                                                          xUpperRightCorner,
                                                          yUpperRightCorner);
        }

        public int CurvilinearFullRefine(int meshKernelId, int mRefinement,int nRefinement)
        {
            return MeshKernelDll.CurvilinearFullRefine(meshKernelId, mRefinement, nRefinement);
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
            CurvilinearGridNative gridNative = grid.CreateNativeObject();
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

        public int CurvilinearFrozenLineAdd(int meshKernelId,
                                                          double xFirstGridLineNode,
                                                          double yFirstGridLineNode,
                                                          double xSecondGridLineNode,
                                                          double ySecondGridLineNode,
                                                          ref int frozenLineId)
        {
            return MeshKernelDll.CurvilinearFrozenLineAdd(meshKernelId,
                                                          xFirstGridLineNode,
                                                          yFirstGridLineNode,
                                                          xSecondGridLineNode,
                                                          ySecondGridLineNode, 
                                                          ref frozenLineId);
        }

        public int CurvilinearFrozenLineDelete(int meshKernelId, int frozenLineId)
        {
            return MeshKernelDll.CurvilinearFrozenLineDelete(meshKernelId, frozenLineId);
        }
        public int CurvilinearFrozenLineIsValid(int meshKernelId, int frozenLineId, ref bool isValid)
        {
            return MeshKernelDll.CurvilinearFrozenLineIsValid(meshKernelId, frozenLineId, ref isValid);
        }

        public int CurvilinearFrozenLineGet(int meshKernelId, 
                                            int frozenLineId, 
                                            ref double xFirstGridLineNode,
                                            ref double yFirstGridLineNode,
                                            ref double xSecondGridLineNode,
                                            ref double ySecondGridLineNode)
        {
            return MeshKernelDll.CurvilinearFrozenLineGet(meshKernelId, 
                                                          frozenLineId, 
                                                          ref xFirstGridLineNode,
                                                          ref yFirstGridLineNode, 
                                                          ref xSecondGridLineNode, 
                                                          ref ySecondGridLineNode);
        }

        public int CurvilinearFrozenLinesGetCount(int meshKernelId, ref int numFrozenLines)
        {
            return MeshKernelDll.CurvilinearFrozenLinesGetCount(meshKernelId, ref numFrozenLines);
        }

        public int CurvilinearFrozenLinesGetIds(int meshKernelId, out int[] frozenLinesIds)
        {
            int numFrozenLines = -1;
            MeshKernelDll.CurvilinearFrozenLinesGetCount(meshKernelId, ref numFrozenLines);
            if (numFrozenLines < 0)
            {
                frozenLinesIds = null;
                return -1;
            }

            frozenLinesIds = new int[numFrozenLines];
            if (numFrozenLines == 0)
            {
                return 0;
            }
            IntPtr frozenLinesIdsPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * frozenLinesIds.Length);
            int success = MeshKernelDll.CurvilinearFrozenLinesGetIds(meshKernelId, frozenLinesIdsPtr);
            Marshal.Copy(frozenLinesIdsPtr, frozenLinesIds, 0, frozenLinesIds.Length);
            Marshal.FreeCoTaskMem(frozenLinesIdsPtr);
            return success;
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


        public int CurvilinearSnapToLandBoundary(int meshKernelId,
                                                 in DisposableGeometryList land,
                                                 double sectionControlPoint1x,
                                                 double sectionControlPoint1y,
                                                 double sectionControlPoint2x,
                                                 double sectionControlPoint2y,
                                                 double regionControlPointX,
                                                 double regionControlPointY)
        {
            var geometryListNative = land.CreateNativeObject();
            return MeshKernelDll.CurvilinearSnapToLandBoundary(meshKernelId,
                                                               ref geometryListNative,
                                                               sectionControlPoint1x,
                                                               sectionControlPoint1y,
                                                               sectionControlPoint2x,
                                                               sectionControlPoint2y,
                                                               regionControlPointX,
                                                               regionControlPointY);
        }

        public int CurvilinearSnapToSpline(int meshKernelId,
                                           in DisposableGeometryList spline,
                                           double sectionControlPoint1x,
                                           double sectionControlPoint1y,
                                           double sectionControlPoint2x,
                                           double sectionControlPoint2y,
                                           double regionControlPointX,
                                           double regionControlPointY)
        {
            var geometryListNative = spline.CreateNativeObject();
            return MeshKernelDll.CurvilinearSnapToSpline(meshKernelId,
                                                         ref geometryListNative,
                                                         sectionControlPoint1x,
                                                         sectionControlPoint1y,
                                                         sectionControlPoint2x,
                                                         sectionControlPoint2y,
                                                         regionControlPointX,
                                                         regionControlPointY);
        }


        public int DeallocateState(int meshKernelId)
        {
            return MeshKernelDll.DeallocateState(meshKernelId);
        }

        public int ExpungeState(int meshKernelId)
        {
            return MeshKernelDll.ExpungeState(meshKernelId);
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
            IntPtr errorMessagePtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            int exitCode = MeshKernelDll.GetError(errorMessagePtr);
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
            GeometryListNative nativeGeometryListIn = disposableGeometryListIn.CreateNativeObject();
            GeometryListNative nativeGeometryListOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.GetSplines(ref nativeGeometryListIn, ref nativeGeometryListOut, numberOfPointsBetweenVertices);
        }

        public int GetVersion(out string version)
        {
            var bufferSize = 64;
            version = "";
            IntPtr versionPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(char)) * bufferSize);
            int exitCode = MeshKernelDll.GetVersion(versionPtr);
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

            int exitCode = MeshKernelDll.Mesh1dGetDimensions(meshKernelId, ref newMesh1D);

            if (exitCode != 0)
            {
                disposableMesh1D = new DisposableMesh1D();
                return exitCode;
            }

            disposableMesh1D = new DisposableMesh1D(newMesh1D.num_nodes,
                                                    newMesh1D.num_edges);

            newMesh1D = disposableMesh1D.CreateNativeObject();

            exitCode = MeshKernelDll.Mesh1dGetData(meshKernelId, ref newMesh1D);
            disposableMesh1D = CreateDisposableMesh1d(newMesh1D);

            return exitCode;
        }

        public int Mesh1dSet(int meshKernelId, in DisposableMesh1D disposableMesh1D)
        {
            Mesh1DNative mesh1D = disposableMesh1D.CreateNativeObject();
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
            GeometryListNative samplesNative = samples.CreateNativeObject();
            GeometryListNative resultsNative = results.CreateNativeObject();

            return MeshKernelDll.Mesh2dAveragingInterpolation(meshKernelId,
                                                              ref samplesNative,
                                                              locationType,
                                                              averagingMethodType,
                                                              relativeSearchSize,
                                                              minNumSamples,
                                                              ref resultsNative);
        }

        public int Mesh2dCasulliDerefinement(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dCasulliDerefinement(meshKernelId);
        }

        public int Mesh2dCasulliDerefinementOnPolygon(int meshKernelId,
                                                      [In] DisposableGeometryList geometryListPolygon)
        {
            GeometryListNative geometryListNativePolygon = geometryListPolygon?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            return MeshKernelDll.Mesh2dCasulliDerefinementOnPolygon(meshKernelId,
                                                                    ref geometryListNativePolygon);
        }

        public int Mesh2dCasulliDerefinementElements(int meshKernelId,
                                                     [In][Out] ref DisposableGeometryList geometryListElements)
        {
            GeometryListNative geometryListNativeElements = geometryListElements.CreateNativeObject();
            return MeshKernelDll.Mesh2dCasulliDerefinementElements(meshKernelId,
                                                                   ref geometryListNativeElements);
        }

        public int Mesh2dCasulliDerefinementElementsOnPolygon(int meshKernelId,
                                                      [In] DisposableGeometryList geometryListPolygon,
                                                      [In][Out] ref DisposableGeometryList geometryListElements)
        {
            GeometryListNative geometryListNativePolygon = geometryListPolygon?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            GeometryListNative geometryListNativeElements = geometryListElements.CreateNativeObject();
            return MeshKernelDll.Mesh2dCasulliDerefinementElementsOnPolygon(meshKernelId,
                                                                            ref geometryListNativePolygon,
                                                                            ref geometryListNativeElements);
        }

        public int Mesh2dCasulliRefinement(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dCasulliRefinement(meshKernelId);
        }

        public int Mesh2dCasulliRefinementOnPolygon(int meshKernelId,
                                                    [In] DisposableGeometryList geometryListPolygon)
        {
            GeometryListNative geometryListPolygonNative = geometryListPolygon?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            return MeshKernelDll.Mesh2dCasulliRefinementOnPolygon(meshKernelId, ref geometryListPolygonNative);
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

            var projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            GeometryListNative geometryListPolygonNative = geometryListPolygon?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };
            GeometryListNative geometryListLandBoundariesNative = geometryListLandBoundaries?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };

            return MeshKernelDll.Mesh2dComputeOrthogonalization(meshKernelId,
                                                                projectToLandBoundaryOptionInt,
                                                                ref orthogonalizationParametersNative,
                                                                ref geometryListPolygonNative,
                                                                ref geometryListLandBoundariesNative);
        }

        public int Mesh2dConnectMeshes(int meshKernelId, in DisposableMesh2D disposableMesh2D, double searchFraction)
        {
            Mesh2DNative mesh2D = disposableMesh2D.CreateNativeObject();
            return MeshKernelDll.Mesh2dConnectMeshes(meshKernelId,
                                                     ref mesh2D,
                                                     searchFraction);
        }

        public int Mesh2dConvertProjection([In] int meshKernelId,
                                           [In] ProjectionOptions projection,
                                           [In] string zone)
        {
            return MeshKernelDll.Mesh2dConvertProjection(meshKernelId,
                                                         (int)projection,
                                                         zone);
        }

        public int Mesh2dConvertCurvilinear([In] int meshKernelId,
                                            [In] double startingFaceCoordinateX,
                                            [In] double startingFaceCoordinateY)
        {
            return MeshKernelDll.Mesh2dConvertToCurvilinear(meshKernelId,
                                                            startingFaceCoordinateX,
                                                            startingFaceCoordinateY);
        }

        public int Mesh2dCountHangingEdges(int meshKernelId, ref int numEdges)
        {
            return MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numEdges);
        }

        public int Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, in DisposableGeometryList selectingPolygon, ref int numberOfPolygonVertices)
        {
            GeometryListNative selectingPolygonNative = selectingPolygon.CreateNativeObject();
            return MeshKernelDll.Mesh2dCountMeshBoundariesAsPolygons(meshKernelId, ref selectingPolygonNative, ref numberOfPolygonVertices);
        }

        public int Mesh2dCountSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesLengthThreshold, ref int numSmallFlowEdges)
        {
            return MeshKernelDll.Mesh2dCountSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesLengthThreshold, ref numSmallFlowEdges);
        }

        public int Mesh2dDelete(int meshKernelId,
                                in DisposableGeometryList disposableGeometryListOut,
                                DeleteMeshInsidePolygonOptions deletionOption,
                                bool invertDeletion)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dDelete(meshKernelId,
                                              ref geometryListNativeIn,
                                              Convert.ToInt32(deletionOption),
                                              invertDeletion);
        }

        public int Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox)
        {
            return MeshKernelDll.Mesh2dDeleteEdge(meshKernelId, xCoordinate, yCoordinate, xLowerLeftBoundingBox, yLowerLeftBoundingBox, xUpperRightBoundingBox, yUpperRightBoundingBox);
        }

        public int Mesh2dDeleteEdgeByIndex(int meshKernelId, int edgeIndex)
        {
            return MeshKernelDll.Mesh2dDeleteEdgeByIndex(meshKernelId, edgeIndex);
        }

        public int Mesh2dDeleteHangingEdges(int meshKernelId)
        {
            return MeshKernelDll.Mesh2dDeleteHangingEdges(meshKernelId);
        }

        /// <inheritdoc/>
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
            var projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;
            GeometryListNative geometryListPolygon = selectingPolygon?.CreateNativeObject() ??
                                                     new GeometryListNative { numberOfCoordinates = 0 };
            GeometryListNative geometryListLandBoundaries = landBoundaries?.CreateNativeObject() ??
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

            int exitCode = MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);

            if (exitCode != 0)
            {
                disposableMesh2D = new DisposableMesh2D();
                return exitCode;
            }

            disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes,
                                                    newMesh2D.num_edges,
                                                    newMesh2D.num_faces,
                                                    newMesh2D.num_face_nodes,
                                                    newMesh2D.num_valid_nodes,
                                                    newMesh2D.num_valid_edges);

            newMesh2D = disposableMesh2D.CreateNativeObject();

            exitCode = MeshKernelDll.Mesh2dGetData(meshKernelId, ref newMesh2D);
            disposableMesh2D = CreateDisposableMesh2D(newMesh2D, true);

            return exitCode;
        }

        public int Mesh2dGetNodeEdgeData(int meshKernelId, out DisposableMesh2D disposableMesh2D)
        {
            var newMesh2D = new Mesh2DNative();

            int exitCode = MeshKernelDll.Mesh2DGetDimensions(meshKernelId, ref newMesh2D);

            if (exitCode != 0)
            {
                disposableMesh2D = new DisposableMesh2D();
                return exitCode;
            }

            disposableMesh2D = new DisposableMesh2D(newMesh2D.num_nodes,
                                                    newMesh2D.num_edges, 
                                                    newMesh2D.num_valid_nodes, 
                                                    newMesh2D.num_valid_edges);

            newMesh2D = disposableMesh2D.CreateNativeObject();

            exitCode = MeshKernelDll.Mesh2dGetNodeEdgeData(meshKernelId, ref newMesh2D);
            disposableMesh2D = CreateDisposableMesh2D(newMesh2D);

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
            int exitCode = MeshKernelDll.Mesh2dCountHangingEdges(meshKernelId, ref numberOfHangingEdges);
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

        public int Mesh2dGetEdgeLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetEdgesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.Mesh2dGetLocationIndex(meshKernelId,
                                                            xCoordinate,
                                                            yCoordinate,
                                                            locationType,
                                                            ref boundingBoxNative,
                                                            ref locationIndex);
            return exitCode;
        }

        public int Mesh2dGetFaceLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetFacesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.Mesh2dGetLocationIndex(meshKernelId,
                                                            xCoordinate,
                                                            yCoordinate,
                                                            locationType,
                                                            ref boundingBoxNative,
                                                            ref locationIndex);
            return exitCode;
        }

        public int Mesh2dGetNodeLocationIndex(int meshKernelId, double xCoordinate, double yCoordinate, BoundingBox boundingBox, ref int locationIndex)
        {
            int locationType = -1;
            var exitCode = MeshKernelDll.GetNodesLocationType(ref locationType);
            if (exitCode != 0)
            {
                return exitCode;
            }
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();
            exitCode = MeshKernelDll.Mesh2dGetLocationIndex(meshKernelId,
                                                            xCoordinate,
                                                            yCoordinate,
                                                            locationType,
                                                            ref boundingBoxNative,
                                                            ref locationIndex);
            return exitCode;
        }

        public int Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, in DisposableGeometryList selectingPolygon, ref DisposableGeometryList disposableGeometryList)
        {
            GeometryListNative geometryListNative = disposableGeometryList.CreateNativeObject();
            GeometryListNative selectingPolygonNative = selectingPolygon.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetMeshBoundariesAsPolygons(meshKernelId, ref selectingPolygonNative, ref geometryListNative);
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
            GeometryListNative geometryListNativeIn = disposableGeometryListIn?.CreateNativeObject() ?? new GeometryListNative { numberOfCoordinates = 0 };

            int numberOfMeshVertices = -1;
            int exitCode = MeshKernelDll.CountVerticesInPolygon(meshKernelId, ref geometryListNativeIn, inside, ref numberOfMeshVertices);
            if (exitCode != 0)
            {
                return exitCode;
            }

            IntPtr selectedVerticesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * numberOfMeshVertices);
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
            GeometryListNative disposableGeometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetObtuseTrianglesMassCenters(meshKernelId, ref disposableGeometryListNative);
        }

        public int Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetOrthogonality(meshKernelId, ref geometryListNativeIn);
        }

        public int Mesh2dCountObtuseTriangles(int meshKernelId, ref int numObtuseTriangles)
        {
            return MeshKernelDll.Mesh2dCountObtuseTriangles(meshKernelId, ref numObtuseTriangles);
        }

        public int Mesh2dGetSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesThreshold, ref DisposableGeometryList disposableGeometryList)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmallFlowEdgeCenters(meshKernelId, smallFlowEdgesThreshold, ref geometryListNativeIn);
        }

        public int Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut)
        {
            GeometryListNative geometryListNativeInOut = disposableGeometryListOut.CreateNativeObject();
            return MeshKernelDll.Mesh2dGetSmoothness(meshKernelId, ref geometryListNativeInOut);
        }

        public int Mesh2dInitializeOrthogonalization(int meshKernelId,
                                                     ProjectToLandBoundaryOptions projectToLandBoundaryOption,
                                                     in OrthogonalizationParameters orthogonalizationParameters,
                                                     in DisposableGeometryList geometryListNativePolygon,
                                                     in DisposableGeometryList geometryListNativeLandBoundaries)
        {
            var projectToLandBoundaryOptionInt = (int)projectToLandBoundaryOption;

            var nativeOrthogonalizationParameters = orthogonalizationParameters.ToOrthogonalizationParametersNative();

            GeometryListNative geometryListPolygon = geometryListNativePolygon?.CreateNativeObject() ??
                                                     new GeometryListNative { numberOfCoordinates = 0 };

            GeometryListNative geometryListLandBoundaries = geometryListNativeLandBoundaries?.CreateNativeObject() ??
                                                            new GeometryListNative { numberOfCoordinates = 0 };

            return MeshKernelDll.Mesh2dInitializeOrthogonalization(meshKernelId, projectToLandBoundaryOptionInt,
                                                                   ref nativeOrthogonalizationParameters, ref geometryListPolygon, ref geometryListLandBoundaries);
        }

        /// <inheritdoc/>
        public int Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dInsertEdge(meshKernelId, startVertexIndex, endVertexIndex, ref edgeIndex);
        }

        /// <inheritdoc/>
        public int Mesh2dInsertEdgeFromCoordinates(int meshKernelId,
                                                   double firstNodeX,
                                                   double firstNodeY,
                                                   double secondNodeX,
                                                   double secondNodeY,
                                                   ref int firstNodeIndex,
                                                   ref int secondNodeIndex,
                                                   ref int edgeIndex)
        {
            return MeshKernelDll.Mesh2dInsertEdgeFromCoordinates(meshKernelId,
                                                                 firstNodeX,
                                                                 firstNodeY,
                                                                 secondNodeX,
                                                                 secondNodeY,
                                                                 ref firstNodeIndex,
                                                                 ref secondNodeIndex,
                                                                 ref edgeIndex);
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

            IntPtr edgeNodesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges * 2);
            IntPtr edgeIndexPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges);
            IntPtr edgeDistancesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * newMesh2D.num_edges);
            IntPtr segmentDistancesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(double)) * newMesh2D.num_edges);
            IntPtr segmentIndexesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_edges);

            IntPtr faceIndexesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);
            IntPtr faceNumEdgesPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);
            IntPtr faceEdgeIndexPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)) * newMesh2D.num_faces);

            GeometryListNative boundaryPolygoneNative = boundaryPolygon.CreateNativeObject();
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

        public int Mesh2dMakeGlobal(int meshKernelId, int numLongitudeNodes, int numLatitudeNodes)
        {
            return MeshKernelDll.Mesh2dMakeGlobal(meshKernelId, numLongitudeNodes, numLatitudeNodes);
        }

        public int Mesh2dMakeTriangularMeshFromPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            GeometryListNative geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeTriangularMeshFromPolygon(meshKernelId, ref geometryListNative);
        }

        public int Mesh2dMakeTriangularMeshFromSamples(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            GeometryListNative geometryListNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMakeTriangularMeshFromSamples(meshKernelId, ref geometryListNative);
        }

        public int Mesh2dMakeRectangularMesh(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMesh(meshKernelId, ref makeGridParametersNative);
        }

        public int Mesh2dMakeRectangularMeshFromPolygon(int meshKernelId, in MakeGridParameters makeGridParameters, in DisposableGeometryList geometryList)
        {
            GeometryListNative geometryListNative = geometryList.CreateNativeObject();
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMeshFromPolygon(meshKernelId, ref makeGridParametersNative, ref geometryListNative);
        }

        public int Mesh2dMakeRectangularMeshOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters)
        {
            var makeGridParametersNative = makeGridParameters.ToMakeGridParametersNative();
            return MeshKernelDll.Mesh2dMakeRectangularMeshOnExtension(meshKernelId, ref makeGridParametersNative);
        }

        /// <inheritdoc/>
        public int Mesh2dMergeNodes(int meshKernelId, in DisposableGeometryList disposableGeometryList)
        {
            GeometryListNative geometryList = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMergeNodes(meshKernelId, ref geometryList);
        }

        /// <inheritdoc/>
        public int Mesh2dMergeNodesWithMergingDistance(int meshKernelId, in DisposableGeometryList disposableGeometryList, double mergingDistance)
        {
            GeometryListNative geometryList = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.Mesh2dMergeNodesWithMergingDistance(meshKernelId, ref geometryList, mergingDistance);
        }

        /// <inheritdoc/>
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

        public int Mesh2dRefineBasedOnGriddedSamples<T>(int meshKernelId,
                                                     in DisposableGriddedSamples<T> griddedSamples,
                                                     in MeshRefinementParameters meshRefinementParameters,
                                                     bool useNodalRefinement)
        {
            GriddedSamplesNative griddedSamplesNative = griddedSamples.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();

            return MeshKernelDll.Mesh2dRefineBasedOnGriddedSamples(meshKernelId,
                                                                   ref griddedSamplesNative,
                                                                   ref meshRefinementParametersNative,
                                                                   useNodalRefinement);
        }

        public int Mesh2dRefineBasedOnPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, in MeshRefinementParameters meshRefinementParameters)
        {
            GeometryListNative disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnPolygon(meshKernelId, ref disposableGeometryListInNative, ref meshRefinementParametersNative);
        }

        public int Mesh2dRefineBasedOnSamples(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, double relativeSearchRadius, int minimumNumSamples, in MeshRefinementParameters meshRefinementParameters)
        {
            GeometryListNative disposableGeometryListInNative = disposableGeometryListIn.CreateNativeObject();
            var meshRefinementParametersNative = meshRefinementParameters.ToMeshRefinementParametersNative();
            return MeshKernelDll.Mesh2dRefineBasedOnSamples(meshKernelId, ref disposableGeometryListInNative, relativeSearchRadius, minimumNumSamples, ref meshRefinementParametersNative);
        }

        public int Mesh2dRotate(int meshKernelId, double centreX, double centreY, double angle)
        {
            return MeshKernelDll.Mesh2dRotate(meshKernelId, centreX, centreY, angle);
        }

        public int Mesh2dTranslate(int meshKernelId, double translationX, double translationY)
        {
            return MeshKernelDll.Mesh2dTranslate(meshKernelId, translationX, translationY);
        }

        /// <inheritdoc/>
        public int Mesh2dSet(int meshKernelId, in DisposableMesh2D disposableMesh2D)
        {
            Mesh2DNative mesh2D = disposableMesh2D.CreateNativeObject();
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

            GeometryListNative samplesNative = samples.CreateNativeObject();
            GeometryListNative resultsNative = results.CreateNativeObject();
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
            GeometryListNative offsetNative = offset.CreateNativeObject();
            return MeshKernelDll.Network1dSet(meshKernelId, ref offsetNative);
        }

        public int Network1dToMesh1d(int meshKernelId, double minFaceSize)
        {
            return MeshKernelDll.Network1dToMesh1d(meshKernelId, minFaceSize);
        }

        public int PolygonCountOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref int numberOfPolygonVertices)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonCountOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref numberOfPolygonVertices);
        }

        public int PolygonCountLinearRefine(int meshKernelId, 
                                            in DisposableGeometryList disposableGeometryListIn,
                                            int firstIndex,
                                            int secondIndex, ref int numberOfPolygonNodes)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            return MeshKernelDll.PolygonCountLinearRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, ref numberOfPolygonNodes);
        }

        public int PolygonCountEquidistantRefine(int meshKernelId, in DisposableGeometryList disposableGeometryList, int firstIndex, int secondIndex, double distance, ref int numberOfPolygonVertices)
        {
            GeometryListNative geometryListInNative = disposableGeometryList.CreateNativeObject();
            return MeshKernelDll.PolygonCountEquidistantRefine(meshKernelId, ref geometryListInNative, firstIndex, secondIndex, distance, ref numberOfPolygonVertices);
        }

        public int GetPointsInPolygon(int meshKernelId, in DisposableGeometryList inputPolygon, in DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints)
        {
            GeometryListNative geometryListNativeInputPolygon = inputPolygon.CreateNativeObject();
            GeometryListNative geometryListNativeinputPoints = inputPoints.CreateNativeObject();
            GeometryListNative geometryListNativeSelectedPoints = selectedPoints.CreateNativeObject();
            return MeshKernelDll.GetPointsInPolygon(meshKernelId, ref geometryListNativeInputPolygon, ref geometryListNativeinputPoints, ref geometryListNativeSelectedPoints);
        }

        public int PolygonGetOffset(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, bool innerPolygon, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            GeometryListNative geometryListNativeOut = disposableGeometryListOut.CreateNativeObject();
            int innerPolygonInt = innerPolygon ? 1 : 0;
            return MeshKernelDll.PolygonGetOffset(meshKernelId, ref geometryListNativeIn, innerPolygonInt, distance, ref geometryListNativeOut);
        }

        public int PolygonEquidistantRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
                                 int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            GeometryListNative geometryListNativeOut = disposableGeometryListOut.CreateNativeObject(); // Create an instance for the out parameter
            return MeshKernelDll.PolygonEquidistantRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, distance, ref geometryListNativeOut);
        }


        public int PolygonLinearRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
                                       int secondIndex, ref DisposableGeometryList disposableGeometryListOut)
        {
            GeometryListNative geometryListNativeIn = disposableGeometryListIn.CreateNativeObject();
            GeometryListNative geometryListNativeOut = disposableGeometryListOut.CreateNativeObject(); // Create an instance for the out parameter
            return MeshKernelDll.PolygonLinearRefine(meshKernelId, ref geometryListNativeIn, firstIndex, secondIndex, ref geometryListNativeOut);
        }

        public int PolygonSnapToLandBoundary(int meshKernelId, 
                                             in DisposableGeometryList landboundaries, 
                                             ref DisposableGeometryList polygon,
                                             int firstIndex,
                                             int secondIndex)
        {
            GeometryListNative landboundariesNative = landboundaries.CreateNativeObject();
            GeometryListNative polygonNative = polygon.CreateNativeObject(); // Create an instance for the out parameter
            return MeshKernelDll.PolygonSnapToLandBoundary(meshKernelId, ref landboundariesNative, ref polygonNative, firstIndex, secondIndex);
        }

        public int SplinesToLandBoundary(int meshKernelId, 
            in DisposableGeometryList landboundaries, 
            ref DisposableGeometryList splines, 
            int firstIndex,
            int secondIndex)
        {
            GeometryListNative landboundariesNative = landboundaries.CreateNativeObject();
            GeometryListNative polygonNative = splines.CreateNativeObject(); 
            return MeshKernelDll.SplinesToLandBoundary(meshKernelId, ref landboundariesNative, ref polygonNative, firstIndex, secondIndex);
        }

        /// <inheritdoc/>
        public int RedoState(ref bool redone, ref int meshKernelId)
        {
            return MeshKernelDll.RedoState(ref redone, ref meshKernelId);
        }

        public int ClearState()
        {
            return MeshKernelDll.ClearState();
        }

        /// <inheritdoc/>
        public int UndoState(ref bool undone, ref int meshKernelId)
        {
            return MeshKernelDll.UndoState(ref undone, ref meshKernelId);
        }

        private DisposableMesh2D CreateDisposableMesh2D(Mesh2DNative newMesh2DNative, bool addCellInformation = false)
        {
            var disposableMesh2D = new DisposableMesh2D
            {
                NodeX = newMesh2DNative.node_x.CreateValueArray<double>(newMesh2DNative.num_nodes),
                NodeY = newMesh2DNative.node_y.CreateValueArray<double>(newMesh2DNative.num_nodes),
                EdgeNodes = newMesh2DNative.edge_nodes.CreateValueArray<int>(newMesh2DNative.num_edges * 2).ToArray(),
                NumEdges = newMesh2DNative.num_edges,
                NumNodes = newMesh2DNative.num_nodes,
                NumValidNodes = newMesh2DNative.num_valid_nodes,
                NumValidEdges = newMesh2DNative.num_valid_edges
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
                NumValidNodes = newMesh1DNative.num_valid_nodes,
                NumValidEdges = newMesh1DNative.num_valid_edges,
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