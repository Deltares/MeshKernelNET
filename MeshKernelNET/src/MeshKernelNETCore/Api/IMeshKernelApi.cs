using System;
using MeshKernelNETCore.Native;

namespace MeshKernelNETCore.Api
{
    public interface IMeshKernelApi : IDisposable
    {
        /// <summary>
        ///     Create a new grid state and return the generated meshKernelId/>
        /// </summary>
        /// <param name="projectionType">
        ///     Cartesian (0), spherical (1) or spherical accurate(2) state
        ///     <returns>Generated meshKernelId</returns>
        int AllocateState(int projectionType);

        /// <summary>
        ///     Deallocate grid state (collections of mesh arrays with auxiliary variables)
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>If the operation succeeded</returns>
        bool DeallocateState(int meshKernelId);

        /// <summary>
        ///     Synchronize provided grid (
        ///     <param name="disposableMesh2D" />
        ///     ) with the grid state with
        ///     <param name="meshKernelId" />
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableMesh2D">Grid state as <see cref="DisposableMesh2D" /> object</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dSet(int meshKernelId, DisposableMesh2D disposableMesh2D);


        /// <summary>
        ///     Gets the grid state as a <see cref="Mesh2DNative" /> structure including the cell information
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableMesh2D"><see cref = "DisposableMesh2D" /> with the grid state
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dGetData(int meshKernelId, out DisposableMesh2D disposableMesh2D);

        /// <summary>
        ///     Deletes a node with specified
        ///     <param name="vertexIndex" />
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="vertexIndex">The index of the node to delete</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dDeleteNode(int meshKernelId, int vertexIndex);

        /// <summary>
        ///     Flips the links
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="isTriangulationRequired">The option to triangulate also non triangular cells </param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="selectingPolygon">The polygon selecting the domain where to flip the edges</param>
        /// <param name="landBoundaries">
        ///     The land boundaries to account for when flipping the edges(num_coordinates = 0 for no land boundaries)
        ///     <returns>If the operation succeeded</returns>
        bool Mesh2dFlipEdges(int meshKernelId, bool isTriangulationRequired,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption, DisposableGeometryList selectingPolygon,
            DisposableGeometryList landBoundaries);

        /// <summary>
        ///     Merges vertex
        ///     <param name="startVertexIndex" />
        ///     to
        ///     <param name="endVertexIndex" />
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="startVertexIndex">The index of the first vertex to merge</param>
        /// <param name="endVertexIndex">The index of the second vertex to merge</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dMergeTwoNodes(int meshKernelId, int startVertexIndex, int endVertexIndex);

        /// <summary>
        ///     Merges vertices within a distance of 0.001 m, effectively removing small edges
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="mergingDistance">
        ///     The distance below which two nodes will be merged
        ///     <returns>If the operation succeeded</returns>
        bool Mesh2dMergeNodes(int meshKernelId, DisposableGeometryList disposableGeometryList, double mergingDistance);

        /// <summary>
        ///     Orthogonalization initialization
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParameters">The structure containing the user defined orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        bool Mesh2dInitializeOrthogonalization(int meshKernelId,
            ProjectToLandBoundaryOptions projectToLandBoundaryOption,
            OrthogonalizationParameters orthogonalizationParameters,
            DisposableGeometryList geometryListNativePolygon,
            DisposableGeometryList geometryListNativeLandBoundaries);

        /// <summary>
        ///     Prepare outer orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dPrepareOuterIterationOrthogonalization(int meshKernelId);

        /// <summary>
        ///     Perform inner orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dComputeInnerOrtogonalizationIteration(int meshKernelId);

        /// <summary>
        ///     Perform outer orthogonalization iteration
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dFinalizeInnerOrtogonalizationIteration(int meshKernelId);

        /// <summary>
        ///     Clean up back-end orthogonalization algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dDeleteOrthogonalization(int meshKernelId);

        /// <summary>
        ///     Make a triangular grid in a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The polygon where to triangulate</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dMakeMeshFromPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryList);


        /// <summary>
        ///     Make a triangular grid from samples
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryList">The samples where to triangulate</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dMakeMeshFromSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryList);

        /// <summary>
        ///     Counts the number of polygon vertices contained in the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="numberOfPolygonVertices">The number of polygon points</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dCountMeshBoundariesAsPolygons(int meshKernelId, ref int numberOfPolygonVertices);

        /// <summary>
        ///     Retrives the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="geometryList">The output network boundary polygon</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dGetMeshBoundariesAsPolygons(int meshKernelId, ref DisposableGeometryList disposableGeometryList);

        /// <summary>
        ///     Refines a grid based on samples
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input samples</param>
        /// <param name="interpolationParameters">The settings for the interpolation algorithm</param>
        /// <param name="samplesRefineParameters">The settings for the interpolation related to samples</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dRefineBasedOnSamples(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
            double relativeSearchRadius, int minimumNumSamples, MeshRefinementParameters meshRefinementParameters);

        /// <summary>
        ///     Refines a grid based on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The closed polygon where to perform the refinement</param>
        /// <param name="meshRefinementParameters">The settings for the mesh refinement algorithm</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dRefineBasedOnPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
            MeshRefinementParameters meshRefinementParameters);

        /// <summary>
        ///     Get the edges orthogonality
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListOut">In Values field the orthogonality values are stored</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dGetOrthogonality(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        ///     Get the edges smoothness
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListOut">In Values field the smoothness values are stored</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dGetSmoothness(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        ///     Inserts a new vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="vertexIndex">the index of the new vertex</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dInsertNode(int meshKernelId, double xCoordinate, double yCoordinate, ref int vertexIndex);

        /// <summary>
        ///     Insert a new edge
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="startVertexIndex">The index of the first vertex to connect</param>
        /// <param name="endVertexIndex">The index of the second vertex to connect</param>
        /// <param name="edgeIndex">The index of the new edge</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dInsertEdge(int meshKernelId, int startVertexIndex, int endVertexIndex, ref int edgeIndex);

        /// <summary>
        ///     Get the index of the closest existing vertex
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
        bool Mesh2dGetNodeIndex(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius,
            double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox,
            double yUpperRightBoundingBox, ref int vertexIndex);

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
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dGetClosestNode(int meshKernelId, double xCoordinateIn, double yCoordinateIn, double searchRadius,
            double xLowerLeftBoundingBox, double yLowerLeftBoundingBox, double xUpperRightBoundingBox,
            double yUpperRightBoundingBox, ref double xCoordinateOut, ref double yCoordinateOut);

        /// <summary>
        ///     Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <returns> true if the edge has been deleted, false if not (the edge is outside the search radius) </returns>
        bool Mesh2dDeleteEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox,
            double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox);

        /// <summary>
        ///     Finds the closest edge within the search radius from the input point
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
        bool Mesh2dGetEdge(int meshKernelId, double xCoordinate, double yCoordinate, double xLowerLeftBoundingBox,
            double yLowerLeftBoundingBox, double xUpperRightBoundingBox, double yUpperRightBoundingBox,
            ref int edgeIndex);

        /// <summary>
        ///     Deletes a mesh in a polygon using several options
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListOut">The polygon where to perform the operation</param>
        /// <param name="deletionOption">The deletion option (to be detailed)</param>
        /// <param name="invertDeletion">Inverts the deletion of selected features</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dDelete(int meshKernelId, ref DisposableGeometryList disposableGeometryListOut, int deletionOption,
            bool invertDeletion);

        /// <summary>
        ///     Function to move a selected vertex to a new position
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="vertexIndex">The vertex index (to be detailed)</param>
        /// <returns>If the operation succeeded</returns>
        bool Mesh2dMoveNode(int meshKernelId, double xCoordinate, double yCoordinate, int vertexIndex);

        /// <summary>
        ///     Make a new grid
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters </param>
        /// <param name="disposableGeometryListIn"></param>
        /// <returns>Error code</returns>
        bool CurvilinearMakeUniform(int meshKernelId, ref MakeGridParameters makeGridParameters,
            ref DisposableGeometryList disposableGeometryListIn);

        /// <summary>
        ///     Make curvilinear grid from splines
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The corner vertices of the splines</param>
        /// <param name="curvilinearParameters">The parameters for the generation of the curvilinear grid</param>
        /// <returns>Error code</returns>
        bool CurvilinearComputeTransfiniteFromSplines(int meshKernelId, 
            ref DisposableGeometryList disposableGeometryListIn,
            ref CurvilinearParameters curvilinearParameters);

        /// <summary>
        ///     Make curvilinear grid from splines with advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The corner vertices of the splines</param>
        /// <param name="curvilinearParameters">The parameters for the generation of the curvilinear grid</param>
        /// <param name="splinesToCurvilinearParameters">The parameters of the advancing front algorithm</param>
        /// <returns>If the operation succeeded</returns>
        bool CurvilinearComputeOrthogonalGridFromSplines(int meshKernelId,
            ref DisposableGeometryList disposableGeometryListIn, 
            ref CurvilinearParameters curvilinearParameters,
            ref SplinesToCurvilinearParameters splinesToCurvilinearParameters);

        /// <summary>
        ///     Computes a curvilinear mesh in a polygon. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="geometryListNative">The input polygon</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node</param>
        /// <param name="useFourthSide">Use (true/false) the fourth polygon side to compute the curvilinear grid</param>
        /// <returns>If the operation succeeded</returns>
        bool CurvilinearComputeTransfiniteFromPolygon(int meshKernelId, ref DisposableGeometryList geometryList,
            int firstNode, int secondNode, int thirdNode, bool useFourthSide);


        /// <summary>
        ///     Computes a curvilinear mesh in a triangle. 3 separate polygon nodes need to be selected. The MeshKernel
        ///     implementation differs from the polygon case.
        ///     For this reason a different api had to be made.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">
        ///     The third node<</param>
        /// <returns>If the operation succeeded</returns>
        bool CurvilinearComputeTransfiniteFromTriangle(int meshKernelId, 
                                                       ref DisposableGeometryList geometryList,
                                                       int firstNode, 
                                                       int secondNode, 
                                                       int thirdNode);

        /// <summary>
        ///     Converts a curvilinear mesh to an unstructured mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>If the operation succeeded</returns>
        bool CurvilinearConvertToMesh2D(int meshKernelId);

        /// <summary>
        ///     Gets the curvilinear data and returns a DisposableCurvilinearGrid
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableCurvilinearGrid"> The disposable curvilinear grid</returns>
        /// <returns>If the operation succeeded</returns>
        bool CurvilinearGridGetData(int meshKernelId, out DisposableCurvilinearGrid disposableCurvilinearGrid);
        
        /// <summary>
        ///     Get spline intermediate points
        /// </summary>
        /// <param name="disposableGeometryListIn">The input corner vertices of the splines</param>
        /// <param name="disposableGeometryListOut">The output spline </param>
        /// <param name="numberOfPointsBetweenVertices">The number of spline vertices between the corners points</param>
        /// <returns>Error code</returns>
        bool GetSplines(DisposableGeometryList disposableGeometryListIn,
            ref DisposableGeometryList disposableGeometryListOut, int numberOfPointsBetweenVertices);

        /// <summary>
        ///     Get the number of vertices of the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="innerPolygon">Compute inner polygon or not</param>
        /// <param name="numberOfPolygonVertices">The number of vertices of the offsetted polygon</param>
        /// <returns>If the operation succeeded</returns>
        bool PolygonCountOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
            bool innerPolygon, double distance, ref int numberOfPolygonVertices);

        /// <summary>
        ///     Get the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="innerPolygon">Compute inner polygon or not</param>
        /// <param name="disposableGeometryListOut">The offsetted polygon</param>
        /// <returns>If the operation succeeded</returns>
        bool PolygonGetOffset(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, bool innerPolygon,
            double distance, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        ///     Count the number of polygon vertices after refinement
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>If the operation succeeded</returns>
        bool PolygonCountRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
            int secondIndex, double distance, ref int numberOfPolygonVertices);

        /// <summary>
        ///     Gets the refined polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="disposableGeometryListOut">The refined polygon</param>
        /// <returns>If the operation succeeded</returns>
        bool PolygonRefine(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn, int firstIndex,
            int secondIndex, double distance, ref DisposableGeometryList disposableGeometryListOut);

        /// <summary>
        ///     Returns the vertices indexes inside selected polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="disposableGeometryListIn"></param>
        /// <param name="inside"> Select inside (0) or outside (1) polygon</param>
        /// <returns>If the operation succeeded</returns>
        bool GetSelectedVerticesInPolygon(int meshKernelId, ref DisposableGeometryList disposableGeometryListIn,
            int inside, ref int[] selectedVertices);

        /// <summary>
        ///     Selects points in polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the grid state</param>
        /// <param name="inputPolygon">The polygon(s) used for selection</param>
        /// <param name="inputPoints">The points to select</param>
        /// <param name="selectedPoints">The selected points in the zCoordinates field (0.0 not selected, 1.0 selected)</param>
        /// <returns>If the operation succeeded</returns>
        bool GetPointsInPolygon(int meshKernelId, ref DisposableGeometryList inputPolygon,
            ref DisposableGeometryList inputPoints, ref DisposableGeometryList selectedPoints);

        /// <summary>
        /// Computes 1d-2d contacts, where each single 1d node is connected to one mesh2d face circumcenter (ggeo_make1D2Dinternalnetlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = connect node, 0 = do not connect)</param>
        /// <param name="polygons">The polygons selecting the area where the 1d-2d contacts will be generated</param>
        /// <param name="projectionFactor">The projection factor used for generating the contacts when 1d nodes are not inside the 2d mesh</param>
        /// <returns>Error code</returns>
        bool ContactsComputeSingle(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons, double projectionFactor);

        /// <summary>
        /// Computes 1d-2d contacts, where a single 1d node is connected to multiple 2d faces circumcenters (ggeo_make1D2Dembeddedlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <returns>Error code</returns>
        bool ContactsComputeMultiple(int meshKernelId, ref IntPtr oneDNodeMask);

        /// <summary>
        /// Computes 1d-2d contacts, where a 2d face per polygon is connected to the closest 1d node (ggeo_make1D2Droofgutterpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The polygons to connect</param>
        /// <returns>Error code</returns>
        bool ContactsComputeWithPolygons(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the 2d faces mass centers containing the input point (ggeo_make1D2Dstreetinletpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="points">The points selecting the faces to connect</param>
        /// <returns>Error code</returns>
        bool ContactsComputeWithPoints(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList points);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the closest 2d faces at the boundary (ggeo_make1D2DRiverLinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The points selecting the faces to connect</param>
        /// <param name="searchRadius">The radius used for searching neighboring faces, if equal to doubleMissingValue, the search radius will be calculated internally</param>
        /// <returns>Error code</returns>
        bool ContactsComputeBoundary(int meshKernelId, ref IntPtr oneDNodeMask, ref DisposableGeometryList polygons, double searchRadius);

        /// <summary>
        ///     Gets the double value used in the back-end library as separator and missing value
        /// </summary>
        /// <returns>The geometry separator</returns>
        double GetSeparator();

        /// <summary>
        ///     Gets the double value used to separate the inner part of a polygon from its outer part (e.g. donut like shape polygons)
        /// </summary>
        /// <returns>The inner outer separator for polygons</returns>
        double GetInnerOuterSeparator();
    }
}