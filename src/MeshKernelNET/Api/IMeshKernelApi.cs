using System;
using System.Runtime.InteropServices;
using MeshKernelNET.Native;

namespace MeshKernelNET.Api
{
    public interface IMeshKernelApi : IDisposable
    {
        /// <summary>
        /// Create a new grid state and return the generated meshKernelId/>
        /// </summary>
        /// <param name="projectionType">
        /// Cartesian (0), spherical (1) or spherical accurate(2) state
        /// <returns>Generated meshKernelId</returns>
        int AllocateState(int projectionType);

        /// <summary>
        /// Clear the undo state
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int ClearUndoState(int meshKernelId);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the closest 2d faces at the boundary
        /// (ggeo_make1D2DRiverLinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The points selecting the faces to connect</param>
        /// <param name="searchRadius">
        /// The radius used for searching neighboring faces, if equal to doubleMissingValue, the search
        /// radius will be calculated internally
        /// </param>
        /// <returns>Error code</returns>
        int ContactsComputeBoundary(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double searchRadius);

        /// <summary>
        /// Computes 1d-2d contacts, where a single 1d node is connected to multiple 2d faces circumcenters
        /// (ggeo_make1D2Dembeddedlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <returns>Error code</returns>
        int ContactsComputeMultiple(int meshKernelId, in IntPtr oneDNodeMask);

        /// <summary>
        /// Computes 1d-2d contacts, where each single 1d node is connected to one mesh2d face circumcenter
        /// (ggeo_make1D2Dinternalnetlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = connect node, 0 = do not connect)</param>
        /// <param name="polygons">The polygons selecting the area where the 1d-2d contacts will be generated</param>
        /// <param name="projectionFactor">
        /// The projection factor used for generating the contacts when 1d nodes are not inside the
        /// 2d mesh
        /// </param>
        /// <returns>Error code</returns>
        int ContactsComputeSingle(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons, double projectionFactor);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the 2d faces mass centers containing the input point
        /// (ggeo_make1D2Dstreetinletpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="points">The points selecting the faces to connect</param>
        /// <returns>Error code</returns>
        int ContactsComputeWithPoints(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList points);

        /// <summary>
        /// Computes 1d-2d contacts, where a 2d face per polygon is connected to the closest 1d node
        /// (ggeo_make1D2Droofgutterpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The polygons to connect</param>
        /// <returns>Error code</returns>
        int ContactsComputeWithPolygons(int meshKernelId, in IntPtr oneDNodeMask, in DisposableGeometryList polygons);

        /// <summary>
        /// Gets the 1d-2d contacts indices (from index / to indices)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="disposableContacts">The out disposable contacts</param>
        /// <returns>Error code</returns>
        int ContactsGetData(int meshKernelId, out DisposableContacts disposableContacts);

        /// <summary>
        /// Make curvilinear grid from splines
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The corner vertices of the splines</param>
        /// <param name="curvilinearParameters">The parameters for the generation of the curvilinear grid</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeTransfiniteFromSplines(int meshKernelId,
                                                     in DisposableGeometryList disposableGeometryListIn,
                                                     in CurvilinearParameters curvilinearParameters);

        /// <summary>
        /// Computes the curvature of a curvilinear grid.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="direction">The direction in which to compute the smoothness</param>
        /// <param name="curvature">The grid curvature values in the selected direction</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeCurvature(int meshKernelId, CurvilinearDirectionOptions direction, ref double[] curvature);

        /// <summary>
        /// Make curvilinear grid from splines with advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The corner vertices of the splines</param>
        /// <param name="curvilinearParameters">The parameters for the generation of the curvilinear grid</param>
        /// <param name="splinesToCurvilinearParameters">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId,
                                                        in DisposableGeometryList disposableGeometryListIn,
                                                        in CurvilinearParameters curvilinearParameters,
                                                        in SplinesToCurvilinearParameters splinesToCurvilinearParameters);

        /// <summary>
        /// Make curvilinear grid from splines.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input splines</param>
        /// <param name="curvilinearParameters">The parameters for the generation of the curvilinear grid</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeGridFromSplines(int meshKernelId,
                                              in DisposableGeometryList disposableGeometryListIn,
                                              in CurvilinearParameters curvilinearParameters);

        /// <summary>
        /// Computes the smoothness of a curvilinear grid.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="direction">The direction in which to compute the smoothness</param>
        /// <param name="smoothness">The grid smoothness values in the selected direction</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeSmoothness(int meshKernelId, CurvilinearDirectionOptions direction, ref double[] smoothness);

        /// <summary>
        /// Computes a curvilinear mesh in a polygon. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="geometryListNative">The input polygon</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node</param>
        /// <param name="useFourthSide">Use (true/false) the fourth polygon side to compute the curvilinear grid</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, in DisposableGeometryList geometryList,
                                                     int firstNode, int secondNode, int thirdNode, bool useFourthSide);

        /// <summary>
        /// Computes a curvilinear mesh in a triangle. 3 separate polygon nodes need to be selected. The MeshKernel
        /// implementation differs from the polygon case.
        /// For this reason a different api had to be made.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">
        /// The third node<</param>
        /// <returns>Error code</returns>
        int CurvilinearComputeTransfiniteFromTriangle(int meshKernelId,
                                                      in DisposableGeometryList geometryList,
                                                      int firstNode,
                                                      int secondNode,
                                                      int thirdNode);

        /// <summary>
        /// Converts a curvilinear mesh to an unstructured mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        int CurvilinearConvertToMesh2D(int meshKernelId);

        /// <summary>
        /// Delete the node closest to a point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xPointCoordinate">The x coordinate of the point</param>
        /// <param name="yPointCoordinate">The y coordinate of the point</param>
        /// <returns>Error code</returns>
        int CurvilinearDeleteNode(int meshKernelId, double xPointCoordinate, double yPointCoordinate);

        /// <summary>
        /// Finalizes curvilinear grid from splines algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        int CurvilinearDeleteOrthogonalGridFromSplines(int meshKernelId);

        /// <summary>
        /// Directional curvilinear grid de-refinement. Grid lines are removed perpendicularly to the segment defined by
        /// lowerLeftCorner and xUpperRightCorner.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block to de-refine</param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block to de-refine</param>
        /// <param name="xUpperRightCorner">The x coordinate of the upper right corner of the block to de-refine</param>
        /// <param name="yUpperRightCorner">The y coordinate of the upper right corner of the block to de-refine</param>
        /// <returns>Error code</returns>
        int CurvilinearDerefine(int meshKernelId, double xLowerLeftCorner, double yLowerLeftCorner, double xUpperRightCorner, double yUpperRightCorner);

        /// <summary>
        /// Finalizes the line shift algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        int CurvilinearFinalizeLineShift(int meshKernelId);

        /// <summary>
        /// Gets the curvilinear data and returns a DisposableCurvilinearGrid
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableCurvilinearGrid">
        /// The disposable curvilinear grid</returns>
        /// <returns>Error code</returns>
        int CurvilinearGridGetData(int meshKernelId, out DisposableCurvilinearGrid disposableCurvilinearGrid);

        /// <summary>
        /// Gets the boundary polygon of a curvilinear grid, nodes with invalid coordinates are excluded
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="lowerLeftN">The n index of the lower left corner</param>
        /// <param name="lowerLeftM">The m index of the lower left corner</param>
        /// <param name="upperRightN">The n index of the upper right corner</param>
        /// <param name="upperRightM">The m index of the upper right corner</param>
        /// <param name="boundaryPolygons">The geometry list containing the boundary polygons. If multiple polygons are present, a separator is used</param>
        /// <returns>Error code</returns>
        int CurvilinearGetBoundariesAsPolygons(int meshKernelId, int lowerLeftN, int lowerLeftM, int upperRightN, int upperRightM, out DisposableGeometryList boundaryPolygons);

        /// <summary>
        /// Gets the index of the closest curvilinear edge
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int CurvilinearGetEdgeLocationIndex(int meshKernelId,
                                            double xCoordinate,
                                            double yCoordinate,
                                            BoundingBox boundingBox,
                                            ref int locationIndex);

        /// <summary>
        /// Gets the index of the closest curvilinear face
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int CurvilinearGetFaceLocationIndex(int meshKernelId,
                                            double xCoordinate,
                                            double yCoordinate,
                                            BoundingBox boundingBox,
                                            ref int locationIndex);

        /// <summary>
        /// Gets the index of the closest curvilinear node
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int CurvilinearGetNodeLocationIndex(int meshKernelId,
                                            double xCoordinate,
                                            double yCoordinate,
                                            BoundingBox boundingBox,
                                            ref int locationIndex);

        /// <summary>
        /// Generates a curvilinear grid from splines with the advancing front method. Initialization step (interactive)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines corners</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param>
        /// <param name="splinesToCurvilinearParameters">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        int CurvilinearInitializeOrthogonalGridFromSplines(int meshKernelId,
                                                           in DisposableGeometryList geometryListNative,
                                                           in CurvilinearParameters curvilinearParametersNative,
                                                           in SplinesToCurvilinearParameters splinesToCurvilinearParameters);

        /// <summary>
        /// Initializes the orthogonal curvilinear algorithm
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="orthogonalizationParameters">The orthogonalization parameters to use in the algorithm</param>
        /// <returns>Error code</returns>
        int CurvilinearInitializeOrthogonalize(int meshKernelId, in OrthogonalizationParameters orthogonalizationParameters);

        /// <summary>
        /// Inserts a new face on a curvilinear grid. The new face will be inserted on top of the closest edge by linear
        /// extrapolation.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">
        /// The x coordinate of the point used for finding the closest face/param>
        /// <param name="yCoordinate">The y coordinate of the point used for finding the closest face</param>
        /// <returns>Error code</returns>
        int CurvilinearInsertFace(int meshKernelId, double xCoordinate, double yCoordinate);

        /// <summary>
        /// One advancement of the front in curvilinear grid from splines (interactive)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="layer">
        /// The layer index/param>
        /// <returns>Error code</returns>
        int CurvilinearIterateOrthogonalGridFromSplines(int meshKernelId, int layer);

        /// <summary>
        /// Attracts/repulses grid lines in a block towards another set grid line
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="repulsionParameter">
        /// The attraction/repulsion parameter. If positive the grid lines will be attracted
        /// towards the set line, if negative the lines will be repulsed
        /// </param>
        /// <param name="xFirstNodeOnTheLine">The x coordinate of the first node of the set line</param>
        /// <param name="yFirstNodeOnTheLine">The y coordinate of the first node of the set line </param>
        /// <param name="xSecondNodeOnTheLine">The x coordinate of the second node of the set line </param>
        /// <param name="ySecondNodeOnTheLine">The y coordinate of the second node of the set line </param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block where the operation is performed </param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block where the operation is performed </param>
        /// <param name="xUpperRightCorner">
        /// The x coordinate of the upper right corner of the block where the operation is
        /// performed
        /// </param>
        /// <param name="yUpperRightCorner">
        /// The y coordinate of the upper right corner of the block where the operation is
        /// performed
        /// </param>
        /// <returns>Error code</returns>
        int CurvilinearLineAttractionRepulsion(int meshKernelId,
                                               double repulsionParameter,
                                               double xFirstNodeOnTheLine,
                                               double yFirstNodeOnTheLine,
                                               double xSecondNodeOnTheLine,
                                               double ySecondNodeOnTheLine,
                                               double xLowerLeftCorner,
                                               double yLowerLeftCorner,
                                               double xUpperRightCorner,
                                               double yUpperRightCorner);

        /// <summary>
        /// Mirrors a boundary gridline outwards. The boundary grid line is defined by its starting and ending points
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mirroringFactor">The x coordinate of the first node of the set line</param>
        /// <param name="xFirstGridLineNode">The x coordinate of the first node of the set line</param>
        /// <param name="yFirstGridLineNode">The y coordinate of the first node of the set line </param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second node of the set line </param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second node of the set line </param>
        /// <returns>Error code</returns>
        int CurvilinearLineMirror(int meshKernelId,
                                  double mirroringFactor,
                                  double xFirstGridLineNode,
                                  double yFirstGridLineNode,
                                  double xSecondGridLineNode,
                                  double ySecondGridLineNode);

        /// @brief Computes the new grid, shifting the line towards the moved nodes and distributing the shifting in block specified before
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        int CurvilinearLineShift(int meshKernelId);

        /// <summary>
        /// Make a new uniform grid
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters </param>
        /// <returns>Error code</returns>
        int CurvilinearComputeRectangularGrid(int meshKernelId, in MakeGridParameters makeGridParameters);

        /// <summary>
        /// Make a new uniform grid from polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters </param>
        /// <param name="disposableGeometryListIn"></param>
        /// <returns>Error code</returns>
        int CurvilinearComputeRectangularGridFromPolygon(int meshKernelId,
                                                         in MakeGridParameters makeGridParameters,
                                                         in DisposableGeometryList disposableGeometryListIn);

        /// <summary>
        /// Make a new curvilinear mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParametersNative">The structure containing the make grid parameters </param>
        /// <returns>Error code</returns>
        int CurvilinearComputeRectangularGridOnExtension(int meshKernelId, in MakeGridParameters makeGridParametersNative);

        /// <summary>
        /// Moves a point of a curvilinear grid from one location to another
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFromPoint">The x coordinate of point to move</param>
        /// <param name="yFromPoint">The y coordinate of point to move</param>
        /// <param name="xToPoint">The new x coordinate of the point</param>
        /// <param name="yToPoint">The new y coordinate of the point</param>
        /// <returns>Error code</returns>
        int CurvilinearMoveNode(int meshKernelId,
                                double xFromPoint,
                                double yFromPoint,
                                double xToPoint,
                                double yToPoint);

        /// <summary>
        /// Moves a node of the line to shift, the operation can be performed multiple times.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFromCoordinate">The x coordinate of the node to move (the closest curvilinear grid node will be found)</param>
        /// <param name="yFromCoordinate">The y coordinate of the node to move (the closest curvilinear grid node will be found)</param>
        /// <param name="xToCoordinate">The x coordinate of the new node position</param>
        /// <param name="yToCoordinate">The y coordinate of the new node position</param>
        /// <returns>Error code</returns>
        int CurvilinearMoveNodeLineShift(int meshKernelId,
                                         double xFromCoordinate,
                                         double yFromCoordinate,
                                         double xToCoordinate,
                                         double yToCoordinate);

        /// <summary>
        /// Orthogonalize a curvilinear grid
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state </param>
        /// <returns>Error code</returns>
        int CurvilinearOrthogonalize(int meshKernelId);

        /// <summary>
        /// Directional curvilinear grid refinement. Additional gridlines are added perpendicularly to the segment defined by
        /// lowerLeftCorner and xUpperRightCorner.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block to refine</param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block to refine</param>
        /// <param name="xUpperRightCorner">The x coordinate of the upper right corner of the block to refine</param>
        /// <param name="yUpperRightCorner">The y coordinate of the upper right corner of the block to refine</param>
        /// <param name="refinement">The number of grid lines to add between the firstPoint and the secondPoint</param>
        /// <returns>Error code</returns>
        int CurvilinearRefine(int meshKernelId,
                              double xLowerLeftCorner,
                              double yLowerLeftCorner,
                              double xUpperRightCorner,
                              double yUpperRightCorner,
                              int refinement);

        /// <summary>
        /// Converts curvilinear grid to mesh and refreshes the state (interactive)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        int CurvilinearRefreshOrthogonalGridFromSplines(int meshKernelId);

        /// <summary>
        /// Sets the curvilinear grid
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="grid">The curvilinear grid to set</param>
        /// <returns>Error code</returns>
        int CurvilinearSet(int meshKernelId, in DisposableCurvilinearGrid grid);

        /// <summary>
        /// Defines a block on the curvilinear where the shifting is distributed
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block</param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block</param>
        /// <param name="xUpperRightCorner">The x coordinate of the upper right corner of the block</param>
        /// <param name="yUpperRightCorner">The y coordinate of the upper right corner of the block</param>
        /// <returns>Error code</returns>
        int CurvilinearSetBlockLineShift(int meshKernelId,
                                         double xLowerLeftCorner,
                                         double yLowerLeftCorner,
                                         double xUpperRightCorner,
                                         double yUpperRightCorner);

        /// <summary>
        /// efine a block on the curvilinear grid where to perform orthogonalization
        /// </summary>
        /// <param name="meshKernelId">The meshKernelId of the block to orthogonalize</param>
        /// <param name="xLowerLeftCorner">The xLowerLeftCorner of the block to orthogonalize</param>
        /// <param name="yLowerLeftCorner">The yLowerLeftCorner of the block to orthogonalize</param>
        /// <param name="xUpperRightCorner">The xUpperRightCorner of the block to orthogonalize</param>
        /// <param name="yUpperRightCorner">The yUpperRightCorner of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int CurvilinearSetBlockOrthogonalize(int meshKernelId,
                                             double xLowerLeftCorner,
                                             double yLowerLeftCorner,
                                             double xUpperRightCorner,
                                             double yUpperRightCorner);

        /// <summary>
        /// Freezes a line in the curvilinear orthogonalization process
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFirstGridLineNode">The x coordinate of the first point of the line to freeze</param>
        /// <param name="yFirstGridLineNode">The y coordinate of the first point of the line to freeze</param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second point of the line to freeze</param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second point of the line to freeze</param>
        /// <returns>Error code</returns>
        int CurvilinearSetFrozenLinesOrthogonalize(int meshKernelId,
                                                   double xFirstGridLineNode,
                                                   double yFirstGridLineNode,
                                                   double xSecondGridLineNode,
                                                   double ySecondGridLineNode);

        /// <summary>
        /// Sets the start and end nodes of the line to shift
        /// </summary>
        /// <param name="meshKernelId">The meshKernelId of the block to orthogonalize</param>
        /// <param name="xFirstGridLineNode">The x coordinate of the first curvilinear grid node to shift</param>
        /// <param name="yFirstGridLineNode">The y coordinate of the first curvilinear grid node to shift</param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second curvilinear grid node to shift</param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second curvilinear grid node to shift</param>
        /// <returns>Error code</returns>
        int CurvilinearSetLineLineShift(int meshKernelId,
                                        double xFirstGridLineNode,
                                        double yFirstGridLineNode,
                                        double xSecondGridLineNode,
                                        double ySecondGridLineNode);

        /// <summary>
        /// Smooths a curvilinear grid
        /// </summary>
        /// <param name="meshKernelId">The meshKernelId of the block to orthogonalize</param>
        /// <param name="smoothingIterations">The number of smoothing iterations to perform</param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block to smooth</param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block to smooth</param>
        /// <param name="xUpperRightCorner">The x coordinate of the right corner of the block to smooth</param>
        /// <param name="yUpperRightCorner">The y coordinate of the upper right corner of the block to smooth</param>
        /// <returns>Error code</returns>
        int CurvilinearSmoothing(int meshKernelId,
                                 int smoothingIterations,
                                 double xLowerLeftCorner,
                                 double yLowerLeftCorner,
                                 double xUpperRightCorner,
                                 double yUpperRightCorner);

        /// <summary>
        /// Smooths a curvilinear grid along the direction specified by a segment
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smoothingIterations">The number of smoothing iterations to perform</param>
        /// <param name="xFirstGridlineNode">The x coordinate of the first curvilinear grid node</param>
        /// <param name="yFirstGridlineNode">The y coordinate of the first curvilinear grid node</param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second curvilinear grid node</param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second curvilinear grid node</param>
        /// <param name="xLowerLeftCornerSmoothingArea">The x coordinate of the lower left corner of the smoothing area</param>
        /// <param name="yLowerLeftCornerSmoothingArea">The y coordinate of the lower left corner of the smoothing area</param>
        /// <param name="xUpperRightCornerSmoothingArea">The x coordinate of the upper right corner of the smoothing area</param>
        /// <param name="yUpperRightCornerSmoothingArea">The y coordinate of the upper right corner of the smoothing area</param>
        /// <returns>Error code</returns>
        int CurvilinearSmoothingDirectional(int meshKernelId,
                                            int smoothingIterations,
                                            double xFirstGridlineNode,
                                            double yFirstGridlineNode,
                                            double xSecondGridLineNode,
                                            double ySecondGridLineNode,
                                            double xLowerLeftCornerSmoothingArea,
                                            double yLowerLeftCornerSmoothingArea,
                                            double xUpperRightCornerSmoothingArea,
                                            double yUpperRightCornerSmoothingArea);

        /// <summary>
        /// Deallocate grid state (collections of mesh arrays with auxiliary variables)
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int DeallocateState(int meshKernelId);

        /// <summary>
        /// Gets an int indicating the closest point averaging method type
        /// </summary>
        /// <param name="method">The int indicating the closest point averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodClosestPoint(ref int method);

        /// <summary>
        /// Gets an int indicating the inverse distance weights averaging method type
        /// </summary>
        /// <param name="method">The int indicating the inverse weight distance averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodInverseDistanceWeighting(ref int method);

        /// <summary>
        /// Gets an int indicating the max value averaging method type
        /// </summary>
        /// <param name="method">The int indicating the max value averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodMax(ref int method);

        /// <summary>
        /// Gets an int indicating the minimum averaging method type
        /// </summary>
        /// <param name="method">The int indicating the minimum averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodMin(ref int method);

        /// <summary>
        /// Gets an int indicating the minimum absolute value averaging method type
        /// </summary>
        /// <param name="method">The int indicating the minimum absolute value averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodMinAbsoluteValue(ref int method);

        /// <summary>
        /// Gets an int indicating the simple averaging method type
        /// </summary>
        /// <param name="method">The int indicating the averaging method type</param>
        /// <returns>Error code</returns>
        int GetAveragingMethodSimpleAveraging(ref int method);

        /// <summary>
        /// Gets an int indicating the edge location type
        /// </summary>
        /// <param name="type">The int indicating the edge location type</param>
        /// <returns>Error code</returns>
        int GetEdgesLocationType(ref int type);

        /// <summary>
        /// Gets a pointer to error message
        /// </summary>
        /// <param name="errorMessage">The pointer to the latest error message</param>
        /// <returns>Error code</returns>
        int GetError(out string errorMessage);

        /// <summary>
        /// Gets the success exit code
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeSuccess(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type MeshKernelError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeMeshKernelError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type NotImplementedError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeNotImplementedError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type AlgorithmError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeAlgorithmError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type ConstraintError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeConstraintError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type MeshGeometryError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeMeshGeometryError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type LinearAlgebraError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeLinearAlgebraError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type RangeError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        int GetExitCodeRangeError(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type StdLibException
        /// </summary>
        /// <param name="exitCode"></param>
        /// <returns>Error code</returns>
        int GetExitCodeStdLibException(ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type UnknownException
        /// </summary>
        /// <param name="exitCode"></param>
        /// <returns>Error code</returns>
        int GetExitCodeUnknownException(ref int exitCode);

        /// <summary>
        /// Gets an int indicating the faces location type
        /// </summary>
        /// <param name="type">The int indicating the face location type</param>
        /// <returns>Error code</returns>
        int GetFacesLocationType(ref int type);

        /// <summary>
        /// get geometry error
        /// </summary>
        /// <param name="invalidIndex">The index of the erroneous entity</param>
        /// <param name="type">The entity type (node, edge or face, see MeshLocations)</param>
        /// <returns>Error code</returns>
        int GetGeometryError(ref int invalidIndex, ref int type);

        /// <summary>
        /// Gets an int indicating the node location type
        /// </summary>
        /// <param name="type">The int indicating the node location type</param>
        /// <returns>Error code</returns>
        int GetNodesLocationType(ref int type);

        /// <summary>
        /// Gets the coordinate projection of the meshkernel state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="projection">The int indicating the projection type</param>
        /// <returns>Error code</returns>
        int GetProjection(int meshKernelId, ref int projection);

        /// <summary>
        /// Gets an int indicating the cartesian projection
        /// </summary>
        /// <param name="projection">The int indicating the cartesian projection</param>
        /// <returns>Error code</returns>
        int GetProjectionCartesian(ref int projection);

        /// <summary>
        /// Gets an int indicating the spherical projection
        /// </summary>
        /// <param name="projection">The int indicating the spherical projection</param>
        /// <returns>Error code</returns>
        int GetProjectionSpherical(ref int projection);

        /// <summary>
        /// Gets an int indicating the spherical accurate projection
        /// </summary>
        /// <param name="projection">The int indicating the spherical accurate projection</param>
        /// <returns>Error code</returns>
        int GetProjectionSphericalAccurate(ref int projection);

        /// <summary>
        /// Get spline intermediate points
        /// </summary>
        /// <param name="disposableGeometryListIn">The input corner vertices of the splines</param>
        /// <param name="disposableGeometryListOut">The output spline </param>
        /// <param name="numberOfPointsBetweenVertices">The number of spline vertices between the corners points</param>
        /// <returns>Error code</returns>
        int GetSplines(in DisposableGeometryList disposableGeometryListIn,
                       ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices);

        /// <summary>
        /// Gets the version string
        /// </summary>
        /// <param name="version">The version string</param>
        /// <returns>Error code</returns>
        int GetVersion(out string version);

        /// <summary>
        /// Gets the Mesh1D data
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The mesh1d of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Mesh1dGetData(int meshKernelId, out DisposableMesh1D disposableMesh1D);

        /// <summary>
        /// Sets the meshkernel::Mesh1D state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The mesh1d</param>
        /// <returns>Error code</returns>
        int Mesh1dSet(int meshKernelId, in DisposableMesh1D disposableMesh1D);

        /// <summary>
        /// AveragingInterpolation interpolation
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="samples">The samples coordinates and values</param>
        /// <param name="locationType">The location type</param>
        /// <param name="averagingMethodType">The averaging method</param>
        /// <param name="relativeSearchSize">The relative search size around the location</param>
        /// <param name="minNumSamples">
        /// The minimum number of samples used for some interpolation algorithms to perform a valid
        /// interpolation
        /// </param>
        /// <param name="results">The interpolation results with x and y coordinates</param>
        /// <returns>Error code</returns>
        int Mesh2dAveragingInterpolation(int meshKernelId,
                                         in DisposableGeometryList samples,
                                         int locationType,
                                         int averagingMethodType,
                                         double relativeSearchSize,
                                         int minNumSamples,
                                         ref DisposableGeometryList results);
        /// <summary>
        /// De-refine a whole mesh using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns
        int Mesh2dCasulliDerefinement(int meshKernelId);

        /// <summary>
        /// De-refine a mesh on polygon using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListPolygon">The input polygons</param>
        /// <returns></returns>
        int Mesh2dCasulliDerefinementOnPolygon(int meshKernelId,
                                               [In] DisposableGeometryList geometryListPolygon);


        /// <summary>
        /// Get the list of elements that will be removed after applying the Casulli de-refinement algorithm to a whole mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListElements">The elements to be removed </param>
        /// <returns>Error code</returns>
        int Mesh2dCasulliDerefinementElements([In] int meshKernelId,
                                              [In][Out] ref DisposableGeometryList geometryListElements);

        /// <summary>
        /// Get the list of elements that will be removed after applying the Casulli de-refinement algorithm to a mesh on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListPolygon">The input polygon</param>
        /// <param name="geometryListElements">The elements to be removed </param>
        /// <returns>Error code</returns>
        int Mesh2dCasulliDerefinementElementsOnPolygon([In] int meshKernelId,
                                                       [In] DisposableGeometryList geometryListPolygon,
                                                       [In][Out] ref DisposableGeometryList geometryListElements);

        /// <summary>
        /// Refine a whole mesh using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns
        int Mesh2dCasulliRefinement(int meshKernelId);

        /// <summary>
        /// Refine a mesh on polygon using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListPolygon">The input polygons</param>
        /// <returns></returns>
        int Mesh2dCasulliRefinementOnPolygon(int meshKernelId,
                                             [In] DisposableGeometryList geometryListPolygon);

        /// <summary>
        /// Perform inner orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int Mesh2dComputeInnerOrtogonalizationIteration(int meshKernelId);

        /// <summary>
        /// Mesh2d orthogonalization
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries </param>
        /// <param name="orthogonalizationParametersNative">The structure containing the orthogonalization parameters </param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization </param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process </param>
        /// <returns>Error code</returns>
        int Mesh2dComputeOrthogonalization(int meshKernelId,
                                           ProjectToLandBoundaryOptions projectToLandBoundaryOption,
                                           OrthogonalizationParameters orthogonalizationParameters,
                                           in DisposableGeometryList geometryListPolygon,
                                           in DisposableGeometryList geometryListLandBoundaries);

        /// <summary>
        /// Converts the projection of a mesh2d
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="projection">The new projection for the mesh</param>
        /// <param name="zone">The UTM zone and information string</param>
        /// <returns>Error code</returns>
        int Mesh2dConvertProjection([In] int meshKernelId,
                                    [In] ProjectionOptions projection,
                                    [In] string zone);

        /// <summary>
        /// Converts a mesh into a curvilinear grid, with the grid expanding outward from a specified starting point.
        /// This function enables the conversion of a portion of the mesh based on the chosen starting coordinate.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="startingFaceCoordinateX">The x coordinate of the point identifying the face where to start the conversion</param>
        /// <param name="startingFaceCoordinateY">The y coordinate of the point identifying the face where to start the conversion</param>
        /// <returns>Error code</returns>
        int Mesh2dConvertCurvilinear([In] int meshKernelId,
                                     [In] double startingFaceCoordinateX,
                                     [In] double startingFaceCoordinateY);

        /// <summary>
        /// Count the number of hanging edges in a mesh2d.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="numEdges">The number of hanging edges</param>
        /// <returns>Error code</returns>
        int Mesh2dCountHangingEdges(int meshKernelId, ref int numEdges);

        /// <summary>
        /// Counts the number of polygon vertices contained in the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="numberOfPolygonVertices">The number of polygon points</param>
        /// <returns>Error code</returns>
        int Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices);

        /// <summary>
        /// Counts the mesh2d small flow edge centers
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smallFlowEdgesLengthThreshold">The smallFlowEdgesLengthThreshold of the block to orthogonalize</param>
        /// <param name="numSmallFlowEdges">The numSmallFlowEdges of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Mesh2dCountSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesLengthThreshold, ref int numSmallFlowEdges);

        /// <summary>
        /// Deletes a mesh in a polygon using several options
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListOut">The polygon where to perform the operation</param>
        /// <param name="deletionOption">The deletion option (to be detailed)</param>
        /// <param name="invertDeletion">Inverts the deletion of selected features</param>
        /// <returns>Error code</returns>
        int Mesh2dDelete(int meshKernelId,
                         in DisposableGeometryList disposableGeometryListOut,
                         DeleteMeshInsidePolygonOptions deletionOption,
                         bool invertDeletion);

        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <returns> true if the edge has been deleted, false if not (the edge is outside the search radius) </returns>
        int Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox,
                             double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox);

        /// <summary>
        /// Deletes a mesh 2d edge given the index of the edge
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="edgeIndex">The index of the edge to delete</param>
        /// <returns> true if the edge has been deleted, false if not </returns>
        int Mesh2dDeleteEdgeByIndex(int meshKernelId, [In] int edgeIndex);

        /// <summary>
        /// Deletes all hanging edges. An hanging edge is an edge where one of the two nodes is not connected.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        int Mesh2dDeleteHangingEdges([In] int meshKernelId);

        /// <summary>
        /// Deletes a node with specified
        /// <param name="vertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="vertexIndex">The index of the node to delete</param>
        /// <returns>Error code</returns>
        int Mesh2dDeleteNode(int meshKernelId, int vertexIndex);

        /// <summary>
        /// Clean up back-end orthogonalization algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int Mesh2dDeleteOrthogonalization(int meshKernelId);

        /// <summary>
        /// Deletes all small mesh2d flow edges and small triangles. The flow edges are the edges connecting faces circumcenters.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smallFlowEdgesThreshold">The configurable threshold for detecting the small flow edges</param>
        /// <param name="minFractionalAreaTriangles">
        /// The ratio of the face area to the average area of neighboring non triangular
        /// faces
        /// </param>
        /// <returns>Error code</returns>
        int Mesh2dDeleteSmallFlowEdgesAndSmallTriangles(int meshKernelId, double smallFlowEdgesThreshold, double minFractionalAreaTriangles);

        /// <summary>
        /// Perform outer orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int Mesh2dFinalizeInnerOrtogonalizationIteration(int meshKernelId);

        /// <summary>
        /// Flips the links
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="isTriangulationRequired">The option to triangulate also non triangular cells </param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="selectingPolygon">The polygon selecting the domain where to flip the edges</param>
        /// <param name="landBoundaries">
        /// The land boundaries to account for when flipping the edges(num_coordinates = 0 for no land boundaries)
        /// <returns>Error code</returns>
        int Mesh2dFlipEdges(int meshKernelId, bool isTriangulationRequired,
                            ProjectToLandBoundaryOptions projectToLandBoundaryOption, in DisposableGeometryList selectingPolygon,
                            in DisposableGeometryList landBoundaries);

        /// <summary>
        /// Get the coordinates of the closest existing vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinateIn">The x coordinate of the node to insert</param>
        /// <param name="yCoordinateIn">The y coordinate of the node to insert</param>
        /// <param name="searchRadius">The radii where to search for mesh nodes</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="xCoordinateOut">The found x coordinate</param>
        /// <param name="yCoordinateOut">The found y coordinate</param>
        /// <returns>Error code</returns>
        int Mesh2dGetClosestNode(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius,
                                 double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox,
                                 double yUpperRightBoundingBox, ref double xCoordinateOut, ref double yCoordinateOut);


        /// <summary>
        /// Gets the grid state as a <see cref="Mesh2DNative"/> structure including the cell information
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableMesh2D">
        /// <see cref="DisposableMesh2D"/> with the grid state
        /// <returns>Error code</returns>
        int Mesh2dGetData(int meshKernelId, out DisposableMesh2D disposableMesh2D);

        /// <summary>
        /// Finds the closest edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="edgeIndex">The index of the found edge</param>
        /// <returns> true if the edge has been deleted, false if not (the edge is outside the search radius) </returns>
        int Mesh2dGetEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox,
                          double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox,
                          ref int edgeIndex);

        /// <summary>
        /// Gets the indices of hanging edges. An hanging edge is an edge where one of the two nodes is not connected.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="edges">Pointer to memory where the indices of the hanging edges will be stored</param>
        /// <returns>Error code</returns>
        int Mesh2dGetHangingEdges(int meshKernelId, out int[] hangingEdges);

        /// <summary>
        /// Gets the index of the closest mesh edge
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int Mesh2dGetEdgeLocationIndex(int meshKernelId,
                                   double xCoordinate,
                                   double yCoordinate,
                                   BoundingBox boundingBox,
                                   ref int locationIndex);

        /// <summary>
        /// Gets the index of the closest mesh face.
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int Mesh2dGetFaceLocationIndex(int meshKernelId,
                                       double xCoordinate,
                                       double yCoordinate,
                                       BoundingBox boundingBox,
                                       ref int locationIndex);

        /// <summary>
        /// Gets the index of the closest mesh node
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="locationType">The location type</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        int Mesh2dGetNodeLocationIndex(int meshKernelId,
                                   double xCoordinate,
                                   double yCoordinate,
                                   BoundingBox boundingBox,
                                   ref int locationIndex);

        /// <summary>
        /// Retrives the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="geometryList">The output network boundary polygon</param>
        /// <returns>Error code</returns>
        int Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList);

        /// <summary>
        /// Get the index of the closest existing vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinateIn">The x coordinate of the node to insert</param>
        /// <param name="yCoordinateIn">The y coordinate of the node to insert</param>
        /// <param name="searchRadius">the radius where to search for the vertex</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="vertexIndex">the index of the closest vertex</param>
        /// <returns>true if a vertex has been found</returns>
        int Mesh2dGetNodeIndex(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius,
                               double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox,
                               double yUpperRightBoundingBox, ref int vertexIndex);

        /// <summary>
        /// Returns the vertices indexes inside selected polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn"></param>
        /// <param name="inside"> Select inside (0) or outside (1) polygon</param>
        /// <returns>Error code</returns>
        int GetSelectedVerticesInPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryListIn,
                                         int inside, ref int[] selectedVertices);

        /// <summary>
        /// Gets the mass centers of obtuse mesh2d triangles. Obtuse triangles are those having one edge longer than the sum of the
        /// other two
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="result">The result of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Mesh2dGetObtuseTrianglesMassCenters(int meshKernelId, ref DisposableGeometryList result);

        /// <summary>
        /// Get the edges orthogonality
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListOut">In Values field the orthogonality values are stored</param>
        /// <returns>Error code</returns>
        int Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        /// Counts the number of polygon nodes contained in the mesh boundary polygons computed in function
        /// `mkernel_mesh2d_get_mesh_boundaries_as_polygons`
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="numObtuseTriangles">The numObtuseTriangles of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Mesh2dCountObtuseTriangles(int meshKernelId, ref int numObtuseTriangles);

        /// <summary>
        /// Gets the small mesh2d flow edges. The flow edges are the edges connecting faces circumcenters
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smallFlowEdgesThreshold">The smallFlowEdgesThreshold of the block to orthogonalize</param>
        /// <param name="result">The result of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Mesh2dGetSmallFlowEdgeCenters(int meshKernelId, double smallFlowEdgesThreshold, ref DisposableGeometryList result);

        /// <summary>
        /// Gets the smoothness, expressed as the ratio between the values of two neighboring faces areas.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryListOut">The smoothness values of each edge</param>
        /// <returns>Error code</returns>
        int Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        /// Orthogonalization initialization
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParameters">The structure containing the user defined orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        int Mesh2dInitializeOrthogonalization(int meshKernelId,
                                              ProjectToLandBoundaryOptions projectToLandBoundaryOption,
                                              in OrthogonalizationParameters orthogonalizationParameters,
                                              in DisposableGeometryList geometryListNativePolygon,
                                              in DisposableGeometryList geometryListNativeLandBoundaries);

        /// <summary>
        /// Insert a new edge
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="startVertexIndex">The index of the first vertex to connect</param>
        /// <param name="endVertexIndex">The index of the second vertex to connect</param>
        /// <param name="edgeIndex">The index of the new edge</param>
        /// <returns>Error code</returns>
        int Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex);

        /// @brief Insert a new mesh2d edge from 2 coordinates. If the coordinates do not match an existing node within a computed search radius, new ones will be created.
        /// The search radius is computed internally based on the minimum mesh size and the distance between the two nodes.
        /// <param name="meshKernelId"> The id of the mesh state
        /// <param name="firstNodeX">    The index of the first node to connect
        /// <param name="firstNodeY">      The index of the second node to connect
        /// <param name="secondNodeX">    The index of the first node to connect
        /// <param name="secondNodeY ">     The index of the second node to connect
        /// <param name="firstNodeIndex">      The index of the first node
        /// <param name="secondNodeIndex">      The index of the second node
        /// <param name="edgeIndex">      The index of the new generated edge
        /// <returns>Error code</returns>
        int Mesh2dInsertEdgeFromCoordinates(int meshKernelId,
                                            double firstNodeX,
                                            double firstNodeY,
                                            double secondNodeX,
                                            double secondNodeY,
                                            ref int firstNodeIndex,
                                            ref int secondNodeIndex,
                                            ref int edgeIndex);

        /// <summary>
        /// Inserts a new vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="vertexIndex">the index of the new vertex</param>
        /// <returns>Error code</returns>
        int Mesh2dInsertNode(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex);

        /// <summary>
        /// Gets the edges intersected by a polygon, with additional information on the intersections
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="boundaryPolygon">An input polygon, defined as a series of points</param>
        /// <param name="edgeNodes">
        /// The indices of the intersected edge nodes. The first node of the edge is on the left (the
        /// virtual node), the second node of the edge is on the right (the inner node)
        /// </param>
        /// <param name="edgeIndex">For each intersected edge, the edge index</param>
        /// <param name="edgeDistances">
        /// For each intersection, the location of the intersection expressed as adimensional distance
        /// from the edge starting node
        /// </param>
        /// <param name="segmentDistances">
        /// For each intersection, the location of the intersection expressed as adimensional
        /// distance from the polygon segment start
        /// </param>
        /// <param name="segmentIndexes">For each intersection, the segment index</param>
        /// <param name="faceIndexes">For each intersection, the face index</param>
        /// <param name="faceNumEdges">For each intersection, the number of intersections</param>
        /// <param name="faceEdgeIndex">For each intersection, the index of the intersected edge</param>
        /// <returns>Error code</returns>
        int Mesh2dIntersectionsFromPolygon(int meshKernelId,
                                           in DisposableGeometryList boundaryPolygon,
                                           ref int[] edgeNodes,
                                           ref int[] edgeIndex,
                                           ref double[] edgeDistances,
                                           ref double[] segmentDistances,
                                           ref int[] segmentIndexes,
                                           ref int[] faceIndexes,
                                           ref int[] faceNumEdges,
                                           ref int[] faceEdgeIndex);

        /// <summary>
        /// Compute the global mesh with a given number of points along the longitude and latitude directions.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="numLongitudeNodes">The number of points along the longitude</param>
        /// <param name="numLatitudeNodes">The number of points along the latitude (half hemisphere)</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeGlobal(int meshKernelId, int numLongitudeNodes, int numLatitudeNodes);

        /// <summary>
        /// Make a triangular grid in a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The polygon where to triangulate</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeTriangularMeshFromPolygon(int meshKernelId, in DisposableGeometryList disposableGeometryList);

        /// <summary>
        /// Make a triangular grid from samples
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The samples where to triangulate</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeTriangularMeshFromSamples(int meshKernelId, in DisposableGeometryList disposableGeometryList);

        /// <summary>
        /// Makes uniform meshes
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeRectangularMesh(int meshKernelId,
                                      in MakeGridParameters makeGridParameters);

        /// <summary>
        /// Makes uniform meshes from a series of polygons
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <param name="geometryList">The polygons to account for</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeRectangularMeshFromPolygon(int meshKernelId,
                                                 in MakeGridParameters makeGridParameters,
                                                 in DisposableGeometryList geometryList);

        /// <summary>
        /// Makes uniform mesh based on a defined on an extension
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <returns>Error code</returns>
        int Mesh2dMakeRectangularMeshOnExtension(int meshKernelId, in MakeGridParameters makeGridParameters);

        /// <summary>
        /// Merges vertices, effectively removing small edges. The merging distance is computed internally based on the minimum
        /// edge size.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// The distance below which two nodes will be merged
        /// <returns>Error code</returns>
        int Mesh2dMergeNodes(int meshKernelId, in DisposableGeometryList disposableGeometryList);

        /// <summary>
        /// Merges vertices within a distance of 0.001 m, effectively removing small edges
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="mergingDistance">The distance below which two nodes will be merged</param>
        /// The distance below which two nodes will be merged
        /// <returns>Error code</returns>
        int Mesh2dMergeNodesWithMergingDistance(int meshKernelId, in DisposableGeometryList disposableGeometryList, double mergingDistance);

        /// <summary>
        /// Merges vertex
        /// <param name="startVertexIndex"/>
        /// to
        /// <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="startVertexIndex">The index of the first vertex to merge</param>
        /// <param name="endVertexIndex">The index of the second vertex to merge</param>
        /// <returns>Error code</returns>
        int Mesh2dMergeTwoNodes(int meshKernelId, int startVertexIndex, int endVertexIndex);

        /// <summary>
        /// Function to move a selected vertex to a new position
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="vertexIndex">The vertex index (to be detailed)</param>
        /// <returns>Error code</returns>
        int Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex);

        /// <summary>
        /// Prepare outer orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>Error code</returns>
        int Mesh2dPrepareOuterIterationOrthogonalization(int meshKernelId);

        /// <summary>
        /// Refine based on gridded samples
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="griddedSamplesNative">The gridded samples</param>
        /// <param name="meshRefinementParameters">The mesh refinement parameters</param>
        /// <param name="useNodalRefinement">Use nodal refinement</param>
        /// <returns>Error code</returns>
        int Mesh2dRefineBasedOnGriddedSamples<T>(int meshKernelId,
                                                 in DisposableGriddedSamples<T> griddedSamplesNative,
                                                 in MeshRefinementParameters meshRefinementParameters,
                                                 bool useNodalRefinement);

        /// <summary>
        /// Refines a grid based on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The closed polygon where to perform the refinement</param>
        /// <param name="meshRefinementParameters">The settings for the mesh refinement algorithm</param>
        /// <returns>Error code</returns>
        int Mesh2dRefineBasedOnPolygon(int meshKernelId,
                                       in DisposableGeometryList disposableGeometryListIn,
                                       in MeshRefinementParameters meshRefinementParameters);

        /// <summary>
        /// Refines a grid based on samples
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input samples</param>
        /// <param name="interpolationParameters">tThe settings for the interpolation algorithm</param>
        /// <param name="samplesRefineParameters">The settings for the interpolation related to samples</param>
        /// <returns>Error code</returns>
        int Mesh2dRefineBasedOnSamples(int meshKernelId, in DisposableGeometryList disposableGeometryListIn,
                                       double relativeSearchRadius, int minimumNumSamples, in MeshRefinementParameters meshRefinementParameters);

        /// <summary>
        /// Rotates a mesh2d about a given point by a given angle
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="centreX">X-coordinate of the centre of rotation</param>
        /// <param name="centreY">Y-coordinate of the centre of rotation</param>
        /// <param name="angle">Angle of rotation in degrees</param>
        /// <returns>Error code</returns>
        int Mesh2dRotate(int meshKernelId, double centreX, double centreY, double angle);

        /// <summary>
        /// Translates a mesh2d
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="translationX">X-component of the translation vector</param>
        /// <param name="translationY">Y-component of the translation vector</param>
        /// <returns>Error code</returns>
        int Mesh2dTranslate(int meshKernelId, double translationX, double translationY);

        /// <summary>
        /// Synchronize provided mesh (
        /// <param name="meshGeometryDimensions"/>
        /// and
        /// <param name="mesh2D"/>
        /// ) with the mesh state with
        /// <param name="meshKernelId"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="mesh2DNative">Mesh dimensions</param>
        /// <returns>Error code</returns>
        int Mesh2dSet(int meshKernelId, in DisposableMesh2D disposableMesh2D);

        /// <summary>
        /// Gets the double value used in the back-end library as separator and missing value
        /// </summary>
        /// <returns>The geometry separator</returns>
        double GetSeparator();

        /// <summary>
        /// Gets the double value used to separate the inner part of a polygon from its outer part (e.g. donut like shape polygons)
        /// </summary>
        /// <returns>The inner outer separator for polygons</returns>
        double GetInnerOuterSeparator();

        /// <summary>
        /// Triangle interpolation
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="samples">The samples coordinates and values</param>
        /// <param name="locationType">The location type</param>
        /// <param name="results">The interpolation results with x and y coordinates</param>
        /// <returns>Error code</returns>
        int Mesh2dTriangulationInterpolation(int meshKernelId, in DisposableGeometryList samples, int locationType, ref DisposableGeometryList results);

        /// <summary>
        /// Compute the network chainages from fixed point locations
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="fixedChainages">
        /// The fixed chainages for each polyline. Chunks are separated by the separator, each chunk
        /// corresponds to a polyline
        /// </param>
        /// <param name="sizeFixedChainages">The size of fixed chainages vector</param>
        /// <param name="minFaceSize">The minimum face size. The distance between two chainages must be no less than this length</param>
        /// <param name="fixedChainagesOffset">The offset to use for fixed chainages</param>
        /// <returns>Error code</returns>
        int Network1dComputeFixedChainages(int meshKernelId, in double[] fixedChainages, double minFaceSize, double fixedChainagesOffset);

        /// <summary>
        /// Network1d compute offsetted chainages
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="offset">The meshKernelId of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        int Network1dComputeOffsettedChainages(int meshKernelId, double offset);

        /// <summary>
        /// Sets the Network1D state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="polylines">The polylines describing the network</param>
        /// <returns>Error code</returns>
        int Network1dSet([In] int meshKernelId, in DisposableGeometryList polylines);

        /// <summary>
        /// Convert network chainages to mesh1d nodes and edges
        /// </summary>
        /// <param name="meshKernelId">
        /// The meshKernelId The id of the mesh state
        /// <param name="minFaceSize">
        /// The minFaceSize The minimum face size below which two nodes will be merged
        /// <returns>Error code</returns>
        int Network1dToMesh1d(int meshKernelId, double minFaceSize);

        /// <summary>
        /// Get the number of vertices of the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The input polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="innerPolygon">Compute inner polygon or not</param>
        /// <param name="numberOfPolygonVertices">The number of vertices of the offsetted polygon</param>
        /// <returns>Error code</returns>
        int PolygonCountOffset(int meshKernelId, in DisposableGeometryList disposableGeometryList,
                               bool innerPolygon, double distance, ref int numberOfPolygonVertices);

        /// <summary>
        /// Count the number of polygon vertices after equidistant refinement
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>Error code</returns>
        int PolygonCountEquidistantRefine(int meshKernelId,
                               in DisposableGeometryList disposableGeometryListIn,
                               int firstIndex,
                               int secondIndex,
                               double distance,
                               ref int numberOfPolygonVertices);

        /// <summary>
        /// Count the number of polygon vertices after linear refinement
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>Error code</returns>
        int PolygonCountLinearRefine(int meshKernelId,
                                      in DisposableGeometryList disposableGeometryListIn,
                                      int firstIndex,
                                      int secondIndex,
                                      ref int numberOfPolygonVertices);

        /// <summary>
        /// Selects points in polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="inputPolygon">The polygon(s) used for selection</param>
        /// <param name="inputPoints">The points to select</param>
        /// <param name="selectedPoints">The selected points in the zCoordinates field (0.0 not selected, 1.0 selected)</param>
        /// <returns>Error code</returns>
        int GetPointsInPolygon(int meshKernelId,
                               in DisposableGeometryList inputPolygon,
                               in DisposableGeometryList inputPoints,
                               ref DisposableGeometryList selectedPoints);

        /// <summary>
        /// Get the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="innerPolygon">Compute inner polygon or not</param>
        /// <param name="disposableGeometryListOut">The offsetted polygon</param>
        /// <returns>Error code</returns>
        int PolygonGetOffset(int meshKernelId,
                             in DisposableGeometryList disposableGeometryListIn,
                             bool innerPolygon,
                             double distance, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        /// Equidistant refinement of a polygon.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="disposableGeometryListOut">The refined polygon</param>
        /// <returns>Error code</returns>
        int PolygonEquidistantRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
                          int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        /// Linear refinement of a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="disposableGeometryListOut">The refined polygon</param>
        /// <returns>Error code</returns>
        int PolygonLinearRefine(int meshKernelId, in DisposableGeometryList disposableGeometryListIn, int firstIndex,
                          int secondIndex, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        /// Redo editing action
        /// </summary>
        /// <param name="redone">If the editing action has been re-done</param>
        /// <returns>Error code</returns>
        int RedoState(ref bool redone);

        /// <summary>
        /// Undo editing action
        /// </summary>
        /// <param name="undone">If the editing action has been un-done</param>
        /// <returns>Error code</returns>
        int UndoState(ref bool undone);
    }
}