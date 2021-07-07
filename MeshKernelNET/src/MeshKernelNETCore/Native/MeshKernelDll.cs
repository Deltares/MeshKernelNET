using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Helpers;

namespace MeshKernelNETCore.Native
{
    // never hit by code coverage because tests use remoting and this only contains dll imports
    [ExcludeFromCodeCoverage]
    internal static class MeshKernelDll
    {
        private const string MeshKernelDllName = "MeshKernelApi.dll";

        static MeshKernelDll()
        {
            var dir = Path.GetDirectoryName(typeof(MeshKernelDll).Assembly.Location);
            NativeLibrary.LoadNativeDll(MeshKernelDllName, Path.Combine(dir, "Lib"));
        }

        #region State management

        /// <summary>
        /// Create a new mesh state and return the generated <param name="meshKernelId"/>
        /// </summary>
        /// <param name="meshKernelId">Identifier for the created grid state</param>
        /// <param name="projectionType">  Cartesian (0), spherical (1) or spherical accurate(2) mesh
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_allocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AllocateState([In] int projectionType, [In, Out] ref int meshKernelId);

        /// <summary>
        /// Deallocate mesh state (collections of mesh arrays with auxiliary variables)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_deallocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DeallocateState([In] int meshKernelId);

        /// <summary>
        /// Gets the mesh2d dimensions <see cref="Mesh2DNative"/> structure
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="mesh2DNative">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2DGetDimensions([In] int meshKernelId, [In, Out] ref Mesh2DNative mesh2DNative);

        /// <summary>
        /// Gets the mesh state as a <see cref="Mesh2DNative"/> structure including faces information
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="mesh2DNative">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetData([In] int meshKernelId, [In, Out] ref Mesh2DNative mesh2DNative);

        /// <summary>
        /// Synchronize provided mesh (<param name="meshGeometryDimensions"/> and <param name="mesh2D"/>) with the mesh state with <param name="meshKernelId"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="meshGeometryDimensions">Mesh dimensions</param>
        /// <param name="mesh2D">Mesh data</param>
        /// <param name="IsGeographic">Cartesian or spherical mesh</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dSet([In] int meshKernelId, [In] ref Mesh2DNative mesh2DNative);

        #endregion

        #region Node operations

        /// <summary>
        /// Deletes a node with specified <param name="nodeIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="nodeIndex">The nodeIndex to delete</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_node", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteNode([In] int meshKernelId, [In] int nodeIndex);

        #endregion

        #region Edges operations  

        /// <summary>
        /// Flip the edges
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="isTriangulationRequired">The option to triangulate also non triangular cells (if activated squares becomes triangles) </param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="selectingPolygon">The polygon where to perform the edge flipping (num_coordinates = 0 for an empty polygon)</param>
        /// <param name="projectToLandBoundaryOption">The land boundaries to account for when flipping the edges (num_coordinates = 0 for no land boundaries)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_flip_edges", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dFlipEdges([In] int meshKernelId, [In] int isTriangulationRequired, [In] int projectToLandBoundaryOption, [In] ref GeometryListNative selectingPolygon, [In] ref GeometryListNative landBoundaries);

        /// <summary>
        /// Insert a new edge connecting <param name="startVertexIndex"/> and <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to connect</param>
        /// <param name="endVertexIndex">The index of the second vertex to connect</param>
        /// <param name="newEdgeIndex">The index of the new edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_insert_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dInsertEdge([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex, [In, Out] ref int edgeIndex);
        #endregion

        #region Vertices operations  
        /// <summary>
        /// Merges vertex <param name="startVertexIndex"/> to <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to merge</param>
        /// <param name="endVertexIndex">The index of the second vertex to merge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_merge_two_nodes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMergeTwoNodes([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex);

        /// <summary>
        /// Merges vertices within a distance of 0.001 m, effectively removing small edges 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The polygon where to perform the operation</param>
        /// <param name="mergingDistance">The distance below which two nodes will be merged</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_merge_nodes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMergeNodes([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] double mergingDistance);

        /// <summary>
        /// Inserts a new mesh node
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="vertexIndex">The index of the new mesh node</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_insert_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dInsertNode([In] int meshKernelId, [In] double xCoordinate, [In] double yCoordinate, [In, Out] ref int vertexIndex);

        /// <summary>
        /// Deletes a mesh in a polygon using several options
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="disposableGeometryList">The polygon where to perform the operation</param>
        /// <param name="deletionOption">The deletion option (to be detailed)</param>
        /// <param name="invertDeletion">Inverts the deletion of selected features</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dDelete([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int deletionOption, [In] bool invertDeletion);

        /// <summary>
        /// Get the index of the closest vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input point coordinates</param>
        /// <param name="searchRadius">the radius where to search for the vertex</param>
        /// <param name="vertexIndex">the index of the closest vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_node_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dGetNodeIndex([In] int meshKernelId, [In] double xCoordinateIn, [In] double yCoordinateIn, [In] double searchRadius, [In, Out] ref int vertexIndex);

        /// <summary>
        /// Get the coordinate of the closest vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinateIn">The x coordinate of the node to insert</param>
        /// <param name="yCoordinateIn">The y coordinate of the node to insert</param>
        /// <param name="searchRadius">The radii where to search for mesh nodes</param>
        /// <param name="xCoordinateOut">The x coordinate of the found Mesh2DNative node</param>
        /// <param name="yCoordinateOut">The y coordinate of the found Mesh2DNative node</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_closest_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dGetClosestNode([In] int meshKernelId, [In] double xCoordinateIn, [In] double yCoordinateIn, [In] double searchRadius, [In, Out] ref double xCoordinateOut, [In, Out] ref double yCoordinateOut);

        #endregion

        #region Orthogonalization
        /// <summary>
        /// Orthogonalization
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParametersNative">The structure containing the orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_compute_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dComputeOrthogonalization([In] int meshKernelId, [In] int projectToLandBoundaryOption, [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative, [In] ref GeometryListNative geometryListNativePolygon, [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Orthogonalization initialization (first function to use in interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParameters">The structure containing the user defined orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_initialize_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dInitializeOrthogonalization([In] int meshKernelId, [In] int projectToLandBoundaryOption, [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative, [In] ref GeometryListNative geometryListNativePolygon, [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Prepare outer orthogonalization iteration (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_prepare_outer_iteration_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dPrepareOuterIterationOrthogonalization([In] int meshKernelId);

        /// <summary>
        /// Perform inner orthogonalization iteration (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_compute_inner_ortogonalization_iteration", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dComputeInnerOrtogonalizationIteration([In] int meshKernelId);

        /// <summary>
        /// Perform outer orthogonalization iteration  (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_finalize_inner_ortogonalization_iteration", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dFinalizeInnerOrtogonalizationIteration([In] int meshKernelId);

        /// <summary>
        /// Clean up back-end orthogonalization algorithm  (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteOrthogonalization([In] int meshKernelId);

        /// <summary>
        /// Make a triangular grid in a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The polygon where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_mesh_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeMeshFromPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);


        /// <summary>
        /// Make a triangular mesh from samples
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The samples where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_mesh_from_samples", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeMeshFromSamples([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        #endregion


        #region Curvilinear grids

        /// <summary>
        /// Gets the curvilinear grid dimensions as a CurvilinearGridNative struct (converted as set of edges and nodes) <see cref="CurvilinearGridNative"/> structure
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="curvilinearGridNative">The structure containing the dimensions of the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetDimensions([In] int meshKernelId, [In, Out] ref CurvilinearGridNative curvilinearGridNative);

        /// <summary>
        /// Gets the mesh state as a <see cref="Mesh2DNative"/> structure including faces information
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="curvilinearGridNative">The structure containing the curvilinear grid arrays</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetData([In] int meshKernelId, [In, Out] ref CurvilinearGridNative curvilinearGridNative);

        /// <summary>
        /// Make a new curvilinear mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters </param>
        /// <param name="geometryListNative">The polygon to account for</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_make_uniform", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearMakeUniform([In] int meshKernelId, [In] ref MakeGridParametersNative makeGridParameters, [In] ref GeometryListNative geometryListNative);


        /// <summary>
        /// Get spline intermediate points 
        /// </summary>
        /// <param name="disposableGeometryListIn">The input corner vertices of the splines</param>
        /// <param name="disposableGeometryListOut">The output spline </param>
        /// <param name="numberOfPointsBetweenVertices">The number of spline vertices between the corners points</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSplines([In] ref GeometryListNative geometryListNativeIn, [In, Out] ref GeometryListNative geometryListNativeOut, [In] int numberOfPointsBetweenVertices);

        /// <summary>
        /// Generates curvilinear grid from splines with transfinite interpolation
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="geometryListNativeIn">The splines</param>
        /// <param name="curvilinearParametersNative">The curvilinear parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_transfinite_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeTransfiniteFromSplines([In] int meshKernelId, [In] ref GeometryListNative geometryListNativeIn, [In] ref CurvilinearParametersNative curvilinearParametersNative);

        /// <summary>
        /// Make curvilinear grid from splines with an advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines corners</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param> 
        /// <param name="splinesToCurvilinearParametersNative">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeOrthogonalGridFromSplines([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] ref CurvilinearParametersNative curvilinearParametersNative, [In] ref SplinesToCurvilinearParametersNative splinesToCurvilinearParametersNative);

        /// <summary>
        /// Computes a curvilinear mesh in a polygon. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">>Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node<</param>
        /// <param name="useFourthSide">Use (true/false) the fourth polygon side to compute the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_transfinite_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeTransfiniteFromPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int firstNode, [In] int secondNode, [In] int thirdNode, [In] bool useFourthSide);

        /// <summary>
        /// Computes a curvilinear mesh in a triangle. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">>Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">The third node</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_transfinite_from_triangle", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeTransfiniteFromTriangle([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int firstNode, [In] int secondNode, [In] int thirdNode);

        /// <summary>
        /// Convert a curviliner mesh stored in meshkernel to an unstructured curvilinear mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_convert_to_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearConvertToMesh2D([In] int meshKernelId);

        #endregion

        #region Mesh operations
        /// <summary>
        /// Counts the number of polygon vertices contained in the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="numberOfPolygonVertices">The number of polygon points</param>
        /// <returns>Error code</returns>          
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_mesh_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCountMeshBoundariesAsPolygons([In] int meshKernelId, [In, Out] ref int numberOfPolygonVertices);


        /// <summary>
        /// Retrives the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryList">The output network boundary polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_mesh_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetMeshBoundariesAsPolygons([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Refine a grid based on the samples contained in the geometry list
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The sample set</param>
        /// <param name="relativeSearchRadius">The relative search radius relative to the face size, used for some interpolation algorithms</param>
        /// <param name="minimumNumSamples">The minimum number of samples used for some averaging algorithms</param>
        /// <param name="meshRefinementParameters">The mesh refinement parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_refine_based_on_samples", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRefineBasedOnSamples([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] double relativeSearchRadius, [In] int minimumNumSamples, [In] ref MeshRefinementParametersNative meshRefinementParameters);

        /// <summary>
        /// Refine a grid based on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The closed polygon where to perform the refinement</param>
        /// <param name="interpolationParametersNative">The interpolation parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_refine_based_on_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRefineBasedOnPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] ref InterpolationParametersNative interpolationParametersNative);

        #endregion

        #region Polygon operations

        /// <summary>
        /// Get the number of vertices of the offsetted polygon 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The coordinate of the offset point</param>
        /// <param name="innerPolygon">Compute inner/outer polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices of the generated polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_count_offset", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonCountOffset([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int innerPolygon, [In] double distance, [In, Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Get the offsetted polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The coordinate of the offset point</param>
        /// <param name="innerPolygon">Compute inner/outer polygon</param>
        /// <param name="distance">The offset distance</param>
        /// <param name="geometryListOut">The offsetted polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_get_offset", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonGetOffset([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int innerPolygon, [In] double distance, [In, Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Count the number of vertices after polygon refinment
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_count_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonCountRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In, Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Gets the refined polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="geometryListOut"></param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In, Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Get the double value used in the back-end library as separator and missing value
        /// </summary>
        /// <returns>The double missing value</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetSeparator();

        /// <summary>
        /// Get the double value used in the back-end library to separate the inner part of a polygon from its outer part
        /// </summary>
        /// <returns>The double missing value</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_inner_outer_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetInnerOuterSeparator();

        #endregion

        /// <summary>
        /// Counts the number of selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="numberOfMeshVertices">The number of selected nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_count_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountVerticesInPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int inside, [In, Out] ref int numberOfMeshVertices);

        /// <summary>
        /// Gets the selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="selectedVerticesPtr">The selected vertices nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSelectedVerticesInPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int inside, [In, Out] ref IntPtr selectedVerticesPtr);

        /// <summary>
        /// Gets the orthogonality
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The orthogonality values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_orthogonality", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetOrthogonality([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Gets the smoothness 
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The smoothness values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_smoothness", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetSmoothness([In] int meshKernelId, [In, Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteEdge([In] int meshKernelId, [In] ref double xCoordinate, [In] ref double yCoordinate);


        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="edgeIndex">The edge index</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetEdge([In] int meshKernelId, [In] ref double xCoordinate, [In] ref double yCoordinate, [In, Out] ref int edgeIndex);

        /// <summary>
        /// Function to move a selected node to a new position
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">The new x coordinate</param>
        /// <param name="xCoordinate">The new y coordinate</param>
        /// <param name="nodeIndex">The index of the mesh2d node to be moved</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_move_node", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMoveNode([In] int meshKernelId, [In] ref double xCoordinate, [In] ref double yCoordinate, [In, Out] ref int nodeIndex);


        /// <summary>
        /// Selects points in polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="inputPolygon">The polygon(s) used for selection</param>
        /// <param name="inputPoints">The points to select</param>
        /// <param name="selectedPoints">The selected points in the zCoordinates field (0.0 not selected, 1.0 selected)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_points_in_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPointsInPolygon([In] int meshKernelId, [In] ref GeometryListNative inputPolygon, [In] ref GeometryListNative inputPoints, [In, Out] ref GeometryListNative selectedPoints);


        #region Compute contacts
        /// <summary>
        /// Computes 1d-2d contacts, where each single 1d node is connected to one mesh2d face circumcenter (ggeo_make1D2Dinternalnetlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = connect node, 0 = do not connect)</param>
        /// <param name="polygons">The polygons selecting the area where the 1d-2d contacts will be generated</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_single", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeSingle([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons);

        /// <summary>
        /// Computes 1d-2d contacts, where a single 1d node is connected to multiple 2d face circumcenters (ggeo_make1D2Dembeddedlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_multiple", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeMultiple([In] int meshKernelId, [In] IntPtr oneDNodeMask);

        /// <summary>
        /// Computes 1d-2d contacts, where a 2d face per polygon is connected to the closest 1d node (ggeo_make1D2Droofgutterpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The polygons to connect</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_with_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeWithPolygons([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the 2d faces mass centers containing the input point (ggeo_make1D2Dstreetinletpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="points">The points selecting the faces to connect</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_with_points", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeWithPoints([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative points);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the closest 2d faces at the boundary (ggeo_make1D2DRiverLinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The points selecting the faces to connect</param>
        /// <param name="searchRadius">The radius used for searching neighboring faces, if equal to doubleMissingValue, the search radius will be calculated internally</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_boundary", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeBoundary([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons, [In] double searchRadius);

        /// <summary>
        /// Gets the number of 1d-2d contacts
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="contacts">Contacts data, filled on the dimension part</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsGetDimensions([In] int meshKernelId, [In, Out] ref ContactsNative contacts);

        /// <summary>
        /// Gets the 1d-2d contacts indices (from index / to indices)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="contacts">Contacts data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsGetData([In] int meshKernelId, [In, Out] ref ContactsNative contacts);

        #endregion

        /// <summary>
        /// Sets the meshkernel::Mesh1D state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The mesh1d</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh1d_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh1dSet(int meshKernelId, [In] ref Mesh1DNative mesh1d);
    }
}