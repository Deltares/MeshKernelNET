using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using MeshKernelNET.Helpers;

namespace MeshKernelNET.Native
{
    // never hit by code coverage because tests use remoting and this only contains dll imports
    [ExcludeFromCodeCoverage]
    internal static class MeshKernelDll
    {
        private const string MeshKernelDllName = "MeshKernelApi.dll";

        static MeshKernelDll()
        {
            string dir = Path.GetDirectoryName(typeof(MeshKernelDll).Assembly.Location);
            NativeLibrary.LoadNativeDll(MeshKernelDllName, Path.Combine(dir, @"win-x64\native"));
        }

        /// <summary>
        /// Create a new mesh state and return the generated
        /// <param name="meshKernelId"/>
        /// </summary>
        /// <param name="projectionType">
        /// Cartesian (0), spherical (1) or spherical accurate(2) mesh
        /// <param name="meshKernelId">Identifier for the created grid state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_allocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int AllocateState([In] int projectionType, [In][Out] ref int meshKernelId);

        /// <summary>
        /// Clear the undo state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_clear_undo_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ClearUndoState([In] int meshKernelId);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the closest 2d faces at the boundary
        /// (ggeo_make1D2DRiverLinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The polygon selecting the faces to connect</param>
        /// <param name="searchRadius">
        /// The radius used for searching neighboring faces, if equal to doubleMissingValue, the search
        /// radius will be calculated internally
        /// </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_boundary", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeBoundary([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons, [In] double searchRadius);

        /// <summary>
        /// Computes 1d-2d contacts, where a single 1d node is connected to multiple 2d face circumcenters
        /// (ggeo_make1D2Dembeddedlinks_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_multiple", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeMultiple([In] int meshKernelId, [In] IntPtr oneDNodeMask);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_single", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeSingle([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons, [In] double projectionFactor);

        /// <summary>
        /// Computes 1d-2d contacts, where 1d nodes are connected to the 2d faces mass centers containing the input point
        /// (ggeo_make1D2Dstreetinletpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="points">The points selecting the faces to connect</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_with_points", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeWithPoints([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative points);

        /// <summary>
        /// Computes 1d-2d contacts, where a 2d face per polygon is connected to the closest 1d node
        /// (ggeo_make1D2Droofgutterpipes_dll)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="oneDNodeMask">The mask to apply to 1d nodes (1 = generate a connection, 0 = do not generate a connection)</param>
        /// <param name="polygons">The polygons to connect</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_compute_with_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsComputeWithPolygons([In] int meshKernelId, [In] IntPtr oneDNodeMask, [In] ref GeometryListNative polygons);

        /// <summary>
        /// Gets the 1d-2d contacts indices (from index / to indices)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="contacts">Contacts data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsGetData([In] int meshKernelId, [In][Out] ref ContactsNative contacts);

        /// <summary>
        /// Gets the number of 1d-2d contacts
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="contacts">Contacts data, filled on the dimension part</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_contacts_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ContactsGetDimensions([In] int meshKernelId, [In][Out] ref ContactsNative contacts);

        /// <summary>
        /// Make curvilinear grid from splines with an advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines corners</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param>
        /// <param name="splinesToCurvilinearParametersNative">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeOrthogonalGridFromSplines([In] int meshKernelId,
                                                                               [In] ref GeometryListNative geometryListNative,
                                                                               [In] ref CurvilinearParametersNative curvilinearParametersNative,
                                                                               [In] ref SplinesToCurvilinearParametersNative splinesToCurvilinearParametersNative);

        /// <summary>
        /// Make curvilinear grid from splines with an advancing front.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeFromSplines([In] int meshKernelId,
                                                                 [In] ref GeometryListNative geometryListNative,
                                                                 [In] ref CurvilinearParametersNative curvilinearParametersNative);

        /// <summary>
        /// Computes the curvature of a curvilinear grid.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="direction">The direction in which to compute the smoothness (0 for M direction, 1 for N direction)</param>
        /// <param name="curvature">The grid curvature values in the selected direction</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_curvature", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeCurvature([In] int meshKernelId, [In] int direction, [In][Out] IntPtr curvature);

        /// <summary>
        /// Computes the smoothness of a curvilinear grid.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="direction">The direction in which to compute the smoothness (0 for m direction, 1 for n direction)</param>
        /// <param name="smoothness">The grid smoothness values in the selected direction</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_smoothness", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeSmoothness([In] int meshKernelId, [In] int direction, [In][Out] IntPtr smoothness);

        /// <summary>
        /// Computes a curvilinear mesh in a polygon. 3 separate polygon nodes need to be selected.
        /// </summary>
        /// <param name="meshKernelId">>Id of the mesh state</param>
        /// <param name="geometryListNative">The input polygons</param>
        /// <param name="firstNode">The first selected node</param>
        /// <param name="secondNode">The second selected node</param>
        /// <param name="thirdNode">
        /// The third node<</param>
        /// <param name="useFourthSide">Use (true/false) the fourth polygon side to compute the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_transfinite_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeTransfiniteFromPolygon([In] int meshKernelId,
                                                                            [In] ref GeometryListNative geometryListNative,
                                                                            [In] int firstNode,
                                                                            [In] int secondNode,
                                                                            [In] int thirdNode,
                                                                            [In] bool useFourthSide);

        /// <summary>
        /// Generates curvilinear grid from splines with transfinite interpolation
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="geometryListNativeIn">The splines</param>
        /// <param name="curvilinearParametersNative">The curvilinear parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_transfinite_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeTransfiniteFromSplines([In] int meshKernelId,
                                                                            [In] ref GeometryListNative geometryListNativeIn,
                                                                            [In] ref CurvilinearParametersNative curvilinearParametersNative);

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
        internal static extern int CurvilinearComputeTransfiniteFromTriangle([In] int meshKernelId,
                                                                             [In] ref GeometryListNative geometryListNative,
                                                                             [In] int firstNode,
                                                                             [In] int secondNode,
                                                                             [In] int thirdNode);

        /// <summary>
        /// Convert a curviliner mesh stored in meshkernel to an unstructured curvilinear mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_convert_to_mesh2d", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearConvertToMesh2D([In] int meshKernelId);

        /// <summary>
        /// Delete the node closest to a point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xPointCoordinate">The x coordinate of the point</param>
        /// <param name="yPointCoordinate">The y coordinate of the point</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_delete_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearDeleteNode([In] int meshKernelId, [In] double xPointCoordinate, [In] double yPointCoordinate);

        /// <summary>
        /// Finalizes curvilinear grid from splines algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_delete_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearDeleteOrthogonalGridFromSplines([In] int meshKernelId);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_derefine", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearDerefine([In] int meshKernelId,
                                                     [In] double xLowerLeftCorner,
                                                     [In] double yLowerLeftCorner,
                                                     [In] double xUpperRightCorner,
                                                     [In] double yUpperRightCorner);

        /// <summary>
        /// Finalizes the line shift algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_finalize_line_shift", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearFinalizeLineShift([In] int meshKernelId);

        /// <summary>
        /// Finalizes the curvilinear orthogonalization algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_finalize_orthogonalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CurvilinearFinalizeOrthogonalize([In] int meshKernelId);

        /// <summary>
        /// Gets the curvilinear grid data as a CurvilinearGrid struct (converted as set of edges and nodes)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="curvilinearGridNative">The structure containing the curvilinear grid arrays</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetData([In] int meshKernelId, [In][Out] ref CurvilinearGridNative curvilinearGridNative);

        /// <summary>
        ///  Counts the number of nodes in curvilinear grid boundary polygons.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="lowerLeftN">The n index of the lower left corner</param>
        /// <param name="lowerLeftM">The m index of the lower left corner</param>
        /// <param name="upperRightN">The n index of the upper right corner</param>
        /// <param name="upperRightM">The m index of the upper right corner</param>
        /// <param name="numberOfPolygonNodes">The number of polygon nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_count_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearCountGetBoundariesAsPolygons([In] int meshKernelId, [In] int lowerLeftN, [In] int lowerLeftM, [In] int upperRightN, [In] int upperRightM, [In][Out] ref int numberOfPolygonNodes);

        /// <summary>
        /// Gets the boundary polygon of a curvilinear grid, nodes with invalid coordinates are excluded.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="lowerLeftN">The n index of the lower left corner</param>
        /// <param name="lowerLeftM">The m index of the lower left corner</param>
        /// <param name="upperRightN">The n index of the upper right corner</param>
        /// <param name="upperRightM">The m index of the upper right corner</param>
        /// <param name="boundaryPolygons">The geometry list containing the boundary polygons. If multiple polygons are present, a separator is used</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetBoundariesAsPolygons([In] int meshKernelId, [In] int lowerLeftN, [In] int lowerLeftM, [In] int upperRightN, [In] int upperRightM, [In][Out] ref GeometryListNative boundaryPolygons);

        /// <summary>
        /// Gets the curvilinear grid dimensions as a CurvilinearGrid struct (converted as set of edges and nodes).
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="curvilinearGridNative">The structure containing the dimensions of the curvilinear grid</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetDimensions([In] int meshKernelId, [In][Out] ref CurvilinearGridNative curvilinearGridNative);

        /// <summary>
        /// Gets the curvilinear location close to a specific coordinate.
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="locationType">The location type</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_get_location_index", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearGetLocationIndex([In] int meshKernelId,
                                                               [In] double xCoordinate,
                                                               [In] double yCoordinate,
                                                               [In] int locationType,
                                                               [In] ref BoundingBoxNative boundingBox,
                                                               [In][Out] ref int locationIndex);

        /// <summary>
        /// Initializes the curvilinear line shift algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_initialize_line_shift", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearInitializeLineShift([In] int meshKernelId);

        /// <summary>
        /// Generates a curvilinear grid from splines with the advancing front method. Initialization step (interactive)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The input splines corners</param>
        /// <param name="curvilinearParametersNative">The input parameters to generate the curvilinear grid</param>
        /// <param name="splinesToCurvilinearParameters">The parameters of the advancing front algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_initialize_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearInitializeOrthogonalGridFromSplines([In] int meshKernelId,
                                                                                  [In] ref GeometryListNative geometryListNative,
                                                                                  [In] ref CurvilinearParametersNative curvilinearParametersNative,
                                                                                  [In] ref SplinesToCurvilinearParametersNative splinesToCurvilinearParameters);

        /// <summary>
        /// Initializes the orthogonal curvilinear algorithm
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="orthogonalizationParameters">The orthogonalization parameters to use in the algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_initialize_orthogonalize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearInitializeOrthogonalize([In] int meshKernelId,
                                                                      [In] ref OrthogonalizationParametersNative orthogonalizationParameters);

        /// <summary>
        /// Inserts a new face on a curvilinear grid. The new face will be inserted on top of the closest edge by linear
        /// extrapolation.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">
        /// The x coordinate of the point used for finding the closest face/param>
        /// <param name="yCoordinate">The y coordinate of the point used for finding the closest face</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_insert_face", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearInsertFace([In] int meshKernelId, [In] double xCoordinate, [In] double yCoordinate);

        /// <summary>
        /// One advancement of the front in curvilinear grid from splines (interactive)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="layer">
        /// The layer index/param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_iterate_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearIterateOrthogonalGridFromSplines([In] int meshKernelId, [In] int layer);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_line_attraction_repulsion", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearLineAttractionRepulsion([In] int meshKernelId,
                                                                      [In] double repulsionParameter,
                                                                      [In] double xFirstNodeOnTheLine,
                                                                      [In] double yFirstNodeOnTheLine,
                                                                      [In] double xSecondNodeOnTheLine,
                                                                      [In] double ySecondNodeOnTheLine,
                                                                      [In] double xLowerLeftCorner,
                                                                      [In] double yLowerLeftCorner,
                                                                      [In] double xUpperRightCorner,
                                                                      [In] double yUpperRightCorner);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_line_mirror", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearLineMirror([In] int meshKernelId,
                                                         [In] double mirroringFactor,
                                                         [In] double xFirstGridLineNode,
                                                         [In] double yFirstGridLineNode,
                                                         [In] double xSecondGridLineNode,
                                                         [In] double ySecondGridLineNode);

        /// @brief Computes the new grid, shifting the line towards the moved nodes and distributing the shifting in block specified before
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_line_shift", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearLineShift([In] int meshKernelId);

        /// <summary>
        /// Make a rectangular grid
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParametersNative">The structure containing the make grid parameters </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_rectangular_grid", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeRectangularGrid([In] int meshKernelId,
                                                                     [In] ref MakeGridParametersNative makeGridParametersNative);

        /// <summary>
        /// Make a rectangular grid from polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParametersNative">The structure containing the make grid parameters </param>
        /// <param name="geometryListNative">The polygon to account for</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_rectangular_grid_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeRectangularGridFromPolygon([In] int meshKernelId,
                                                                                [In] ref MakeGridParametersNative makeGridParametersNative,
                                                                                [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Makes rectangular grid based on a defined on an extension
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="makeGridParametersNative">The structure containing the make grid parameters </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_compute_rectangular_grid_on_extension", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearComputeRectangularGridOnExtension([In] int meshKernelId, [In] ref MakeGridParametersNative makeGridParametersNative);

        /// <summary>
        /// Moves a point of a curvilinear grid from one location to another
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFromPoint">The x coordinate of point to move</param>
        /// <param name="yFromPoint">The y coordinate of point to move</param>
        /// <param name="xToPoint">The new x coordinate of the point</param>
        /// <param name="yToPoint">The new y coordinate of the point</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_move_node", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearMoveNode([In] int meshKernelId,
                                                       [In] double xFromPoint,
                                                       [In] double yFromPoint,
                                                       [In] double xToPoint,
                                                       [In] double yToPoint);

        /// <summary>
        /// Moves a node of the line to shift, the operation can be performed multiple times.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFromCoordinate">The x coordinate of the node to move (the closest curvilinear grid node will be found)</param>
        /// <param name="yFromCoordinate">The y coordinate of the node to move (the closest curvilinear grid node will be found)</param>
        /// <param name="xToCoordinate">The x coordinate of the new node position</param>
        /// <param name="yToCoordinate">The y coordinate of the new node position</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_move_node_line_shift", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearMoveNodeLineShift([In] int meshKernelId,
                                                                [In] double xFromCoordinate,
                                                                [In] double yFromCoordinate,
                                                                [In] double xToCoordinate,
                                                                [In] double yToCoordinate);

        /// <summary>
        /// Orthogonalize a curvilinear grid
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_orthogonalize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearOrthogonalize([In] int meshKernelId);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearRefine([In] int meshKernelId,
                                                     [In] double xLowerLeftCorner,
                                                     [In] double yLowerLeftCorner,
                                                     [In] double xUpperRightCorner,
                                                     [In] double yUpperRightCorner,
                                                     [In] int refinement);

        /// <summary>
        /// Converts curvilinear grid to mesh and refreshes the state (interactive)
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_refresh_orthogonal_grid_from_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearRefreshOrthogonalGridFromSplines([In] int meshKernelId);

        /// <summary>
        /// Sets the curvilinear grid
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSet([In] int meshKernelId, [In] ref CurvilinearGridNative grid);

        /// <summary>
        /// Defines a block on the curvilinear where the shifting is distributed
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xLowerLeftCorner">The x coordinate of the lower left corner of the block</param>
        /// <param name="yLowerLeftCorner">The y coordinate of the lower left corner of the block</param>
        /// <param name="xUpperRightCorner">The x coordinate of the upper right corner of the block</param>
        /// <param name="yUpperRightCorner">The y coordinate of the upper right corner of the block</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_set_block_line_shift", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSetBlockLineShift([In] int meshKernelId,
                                                                [In] double xLowerLeftCorner,
                                                                [In] double yLowerLeftCorner,
                                                                [In] double xUpperRightCorner,
                                                                [In] double yUpperRightCorner);

        /// <summary>
        /// efine a block on the curvilinear grid where to perform orthogonalization
        /// </summary>
        /// <param name="meshKernelId">The meshKernelId of the block to orthogonalize</param>
        /// <param name="xLowerLeftCorner">The xLowerLeftCorner of the block to orthogonalize</param>
        /// <param name="yLowerLeftCorner">The yLowerLeftCorner of the block to orthogonalize</param>
        /// <param name="xUpperRightCorner">The xUpperRightCorner of the block to orthogonalize</param>
        /// <param name="yUpperRightCorner">The yUpperRightCorner of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_set_block_orthogonalize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSetBlockOrthogonalize([In] int meshKernelId,
                                                                    [In] double xLowerLeftCorner,
                                                                    [In] double yLowerLeftCorner,
                                                                    [In] double xUpperRightCorner,
                                                                    [In] double yUpperRightCorner);

        /// <summary>
        /// Freezes a line in the curvilinear orthogonalization process
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xFirstGridLineNode">The x coordinate of the first point of the line to freeze</param>
        /// <param name="yFirstGridLineNode">The y coordinate of the first point of the line to freeze</param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second point of the line to freeze</param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second point of the line to freeze</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_set_frozen_lines_orthogonalize", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSetFrozenLinesOrthogonalize([In] int meshKernelId,
                                                                          [In] double xFirstGridLineNode,
                                                                          [In] double yFirstGridLineNode,
                                                                          [In] double xSecondGridLineNode,
                                                                          [In] double ySecondGridLineNode);

        /// <summary>
        /// Sets the start and end nodes of the line to shift
        /// </summary>
        /// <param name="meshKernelId">The meshKernelId of the block to orthogonalize</param>
        /// <param name="xFirstGridLineNode">The x coordinate of the first curvilinear grid node to shift</param>
        /// <param name="yFirstGridLineNode">The y coordinate of the first curvilinear grid node to shift</param>
        /// <param name="xSecondGridLineNode">The x coordinate of the second curvilinear grid node to shift</param>
        /// <param name="ySecondGridLineNode">The y coordinate of the second curvilinear grid node to shift</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_set_line_line_shift", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSetLineLineShift([In] int meshKernelId,
                                                               [In] double xFirstGridLineNode,
                                                               [In] double yFirstGridLineNode,
                                                               [In] double xSecondGridLineNode,
                                                               [In] double ySecondGridLineNode);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_smoothing", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSmoothing([In] int meshKernelId,
                                                        [In] int smoothingIterations,
                                                        [In] double xLowerLeftCorner,
                                                        [In] double yLowerLeftCorner,
                                                        [In] double xUpperRightCorner,
                                                        [In] double yUpperRightCorner);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_curvilinear_smoothing_directional", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CurvilinearSmoothingDirectional([In] int meshKernelId,
                                                                   [In] int smoothingIterations,
                                                                   [In] double xFirstGridlineNode,
                                                                   [In] double yFirstGridlineNode,
                                                                   [In] double xSecondGridLineNode,
                                                                   [In] double ySecondGridLineNode,
                                                                   [In] double xLowerLeftCornerSmoothingArea,
                                                                   [In] double yLowerLeftCornerSmoothingArea,
                                                                   [In] double xUpperRightCornerSmoothingArea,
                                                                   [In] double yUpperRightCornerSmoothingArea);

        /// <summary>
        /// Deallocate mesh state
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_deallocate_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int DeallocateState([In] int meshKernelId);

        /// <summary>
        /// Deallocate mesh state and remove it completely, no undo for this meshKernelId will be possible after expunging
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_expunge_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int ExpungeState([In] int meshKernelId);

        /// <summary>
        /// Gets an int indicating the closest point averaging method type
        /// </summary>
        /// <param name="method">The int indicating the closest point averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_closest_point", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodClosestPoint([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the inverse distance weights averaging method type
        /// </summary>
        /// <param name="method">The int indicating the inverse weight distance averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_inverse_distance_weighting", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodInverseDistanceWeighting([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the max value averaging method type
        /// </summary>
        /// <param name="method">The int indicating the max value averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_max", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodMax([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the minimum averaging method type
        /// </summary>
        /// <param name="method">The int indicating the minimum averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_min", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodMin([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the minimum absolute value averaging method type
        /// </summary>
        /// <param name="method">The int indicating the minimum absolute value averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_min_absolute_value", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodMinAbsoluteValue([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the simple averaging method type
        /// </summary>
        /// <param name="method">The int indicating the averaging method type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_averaging_method_simple_averaging", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetAveragingMethodSimpleAveraging([In][Out] ref int method);

        /// <summary>
        /// Gets an int indicating the edge location type
        /// </summary>
        /// <param name="type">The int indicating the edge location type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_edges_location_type", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetEdgesLocationType([In][Out] ref int type);

        /// <summary>
        /// Gets a pointer to error message
        /// </summary>
        /// <param name="message">The pointer to the latest error message</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetError([In][Out] IntPtr message);

        /// <summary>
        /// Gets the success exit code
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_success", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeSuccess([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type MeshKernelError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_meshkernel_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeMeshKernelError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type NotImplementedError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_not_implemented_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeNotImplementedError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type AlgorithmError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_algorithm_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeAlgorithmError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type ConstraintError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_constraint_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeConstraintError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type MeshGeometryError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_mesh_geometry_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeMeshGeometryError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type LinearAlgebraError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_linear_algebra_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeLinearAlgebraError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type RangeError
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_range_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeRangeError([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type StdLibException
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_stdlib_exception", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeStdLibException([In][Out] ref int exitCode);

        /// <summary>
        /// Gets the exit code of an exception of type UnknownException
        /// </summary>
        /// <param name="exitCode">The exit code</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_exit_code_unknown_exception", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetExitCodeUnknownException([In][Out] ref int exitCode);

        /// <summary>
        /// Gets an int indicating the faces location type
        /// </summary>
        /// <param name="type">The int indicating the face location type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_faces_location_type", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetFacesLocationType([In][Out] ref int type);

        /// <summary>
        /// get geometry error
        /// </summary>
        /// <param name="invalidIndex">The index of the erroneous entity</param>
        /// <param name="type">The entity type (node, edge or face, see MeshLocations)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_geometry_error", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetGeometryError([In][Out] ref int invalidIndex, [In][Out] ref int type);

        /// <summary>
        /// Gets an int indicating the node location type
        /// </summary>
        /// <param name="type">The int indicating the node location type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_nodes_location_type", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetNodesLocationType([In][Out] ref int type);

        /// <summary>
        /// Gets the coordinate projection of the meshkernel state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="projection">The int indicating the projection type</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_projection", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetProjection([In] int meshKernelId, [In][Out] ref int projection);

        /// <summary>
        /// Gets an int indicating the cartesian projection
        /// </summary>
        /// <param name="projection">The int indicating the cartesian projection</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_projection_cartesian", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetProjectionCartesian([In][Out] ref int projection);

        /// <summary>
        /// Gets an int indicating the spherical projection
        /// </summary>
        /// <param name="projection">The int indicating the spherical projection</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_projection_spherical", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetProjectionSpherical([In][Out] ref int projection);

        /// <summary>
        /// Gets an int indicating the spherical accurate projection
        /// </summary>
        /// <param name="projection">The int indicating the spherical accurate projection</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_projection_spherical_accurate", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetProjectionSphericalAccurate([In][Out] ref int projection);

        /// <summary>
        /// Gets an integer indicating the interpolation type double
        /// </summary>
        /// <param name="type">The integer indicating the interpolation type double</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_interpolation_type_double", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetInterpolationTypeDouble([In][Out] ref int type);

        /// <summary>
        /// Get spline intermediate points
        /// </summary>
        /// <param name="geometryListNativeIn">The input corner nodes of the splines</param>
        /// <param name="geometryListNativeOut">The output spline</param>
        /// <param name="numberOfPointsBetweenVertices">The number of spline vertices between the corners points</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_splines", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSplines([In] ref GeometryListNative geometryListNativeIn,
                                              [In][Out] ref GeometryListNative geometryListNativeOut,
                                              [In] int numberOfPointsBetweenVertices);

        /// <summary>
        /// Gets the version string
        /// </summary>
        /// <param name="version">The version string</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_version", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetVersion([In][Out] IntPtr version);

        /// <summary>
        /// Gets the Mesh1D data
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The mesh1d of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh1d_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh1dGetData([In] int meshKernelId, [In][Out] ref Mesh1DNative mesh1d);

        /// <summary>
        /// Gets the Mesh1D data dimensions
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The structure containing the dimensions of the Mesh1D</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh1d_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh1dGetDimensions([In] int meshKernelId, [In][Out] ref Mesh1DNative mesh1d);

        /// <summary>
        /// Sets the meshkernel::Mesh1D state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="mesh1d">The mesh1d</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh1d_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh1dSet(int meshKernelId, [In] ref Mesh1DNative mesh1d);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_averaging_interpolation", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dAveragingInterpolation([In] int meshKernelId,
                                                                [In] ref GeometryListNative samples,
                                                                [In] int locationType,
                                                                [In] int averagingMethodType,
                                                                [In] double relativeSearchSize,
                                                                [In] int minNumSamples,
                                                                [In][Out] ref GeometryListNative results);
        /// <summary>
        /// De-refine a whole mesh using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_derefinement", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliDerefinement([In] int meshKernelId);

        /// <summary>
        /// De-refine a mesh on polygon using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the de-refinement </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_derefinement_on_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliDerefinementOnPolygon([In] int meshKernelId,
                                                                      [In] ref GeometryListNative geometryListNativePolygon);

        /// <summary>
        /// Get the list of elements that will be removed after applying the Casulli de-refinement algorithm to a whole mesh
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNativeElements">The elements to be removed </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_derefinement_elements", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliDerefinementElements([In] int meshKernelId,
                                                                     [In][Out] ref GeometryListNative geometryListNativeElements);

        /// <summary>
        /// Get the list of elements that will be removed after applying the Casulli de-refinement algorithm to a mesh on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNativePolygon">The input polygon</param>
        /// <param name="geometryListNativeElements">The elements to be removed </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_derefinement_elements_on_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliDerefinementElementsOnPolygon([In] int meshKernelId,
                                                                              [In] ref GeometryListNative geometryListNativePolygon,
                                                                              [In][Out] ref GeometryListNative geometryListNativeElements);


        /// <summary>
        /// Refine a whole mesh using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_refinement", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliRefinement([In] int meshKernelId);

        /// <summary>
        /// Refine a mesh on polygon using the Casulli algorithm
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param
        /// <param name="geometryListNativePolygon">The polygon where to perform the refinement </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_casulli_refinement_on_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCasulliRefinementOnPolygon([In] int meshKernelId,
                                                                    [In] ref GeometryListNative geometryListNativePolygon);

        /// <summary>
        /// Performs inner orthogonalization iteration, by slowly moving the mesh nodes to new optimal positions (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_compute_inner_ortogonalization_iteration", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dComputeInnerOrtogonalizationIteration([In] int meshKernelId);

        /// <summary>
        /// Mesh2d orthogonalization
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries </param>
        /// <param name="orthogonalizationParametersNative">The structure containing the orthogonalization parameters </param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization </param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_compute_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dComputeOrthogonalization([In] int meshKernelId,
                                                                  [In] int projectToLandBoundaryOption,
                                                                  [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative,
                                                                  [In] ref GeometryListNative geometryListNativePolygon,
                                                                  [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Connect two disconnected regions along boundary
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state pertaining to the current domain</param>
        /// <param name="mesh2DNative">The mesh to merge to the the current domain</param>
        /// <param name="searchFraction">Fraction of the shortest edge (along an edge to be connected) to use when determining neighbour edge closeness</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_connect_meshes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dConnectMeshes([In] int meshKernelId, [In] ref Mesh2DNative mesh2DNative, [In] double searchFraction);

        /// <summary>
        /// Converts the projection of a mesh2d
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="projection">The new projection for the mesh</param>
        /// <param name="zone">The UTM zone and information string</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_convert_projection", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dConvertProjection([In] int meshKernelId,
                                                           [In] int projection,
                                                           [In][MarshalAs(UnmanagedType.LPStr)] string zone);

        /// <summary>
        /// Converts the projection of a mesh2d
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="startingFaceCoordinateX">The x coordinate of the point identifying the face where to start the conversion</param>
        /// <param name="startingFaceCoordinateY">The y coordinate of the point identifying the face where to start the conversion</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_convert_to_curvilinear", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dConvertToCurvilinear([In] int meshKernelId,
                                                              [In] double startingFaceCoordinateX,
                                                              [In] double startingFaceCoordinateY);

        /// <summary>
        /// Count the number of hanging edges in a mesh2d.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="numEdges">The number of hanging edges</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_hanging_edges", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCountHangingEdges([In] int meshKernelId, [In][Out] ref int numEdges);

        /// <summary>
        /// Counts the number of polygon vertices contained in the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="numberOfPolygonVertices">The number of polygon points</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_mesh_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCountMeshBoundariesAsPolygons([In] int meshKernelId, [In][Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Counts the mesh2d small flow edge centers
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smallFlowEdgesLengthThreshold">The smallFlowEdgesLengthThreshold of the block to orthogonalize</param>
        /// <param name="numSmallFlowEdges">The numSmallFlowEdges of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_small_flow_edge_centers", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCountSmallFlowEdgeCenters([In] int meshKernelId,
                                                                   [In] double smallFlowEdgesLengthThreshold,
                                                                   [In][Out] ref int numSmallFlowEdges);

        /// <summary>
        /// Deletes a mesh in a polygon using several options
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The polygon where to perform the operation</param>
        /// <param name="deletionOption">The deletion option (to be detailed)</param>
        /// <param name="invertDeletion">Inverts the deletion of selected features</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dDelete([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] int deletionOption, [In] bool invertDeletion);

        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteEdge([In] int meshKernelId,
                                                    [In] double xCoordinate,
                                                    [In] double yCoordinate,
                                                    [In] double xLowerLeftBoundingBox,
                                                    [In] double yLowerLeftBoundingBox,
                                                    [In] double xUpperRightBoundingBox,
                                                    [In] double yUpperRightBoundingBox);

        /// <summary>
        /// Deletes a mesh 2d edge given the index of the edge
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="edgeIndex">The index of the edge to delete</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_edge_by_index", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteEdgeByIndex([In] int meshKernelId, [In] int edgeIndex);

        /// <summary>
        /// Deletes all hanging edges. An hanging edge is an edge where one of the two nodes is not connected.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_hanging_edges", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteHangingEdges([In] int meshKernelId);

        /// <summary>
        /// Deletes a mesh2d node
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="nodeIndex">The index of the node to delete</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_node", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteNode([In] int meshKernelId, [In] int nodeIndex);

        /// <summary>
        /// Clean up back-end orthogonalization algorithm  (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteOrthogonalization([In] int meshKernelId);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_delete_small_flow_edges_and_small_triangles", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dDeleteSmallFlowEdgesAndSmallTriangles([In] int meshKernelId,
                                                                               [In] double smallFlowEdgesThreshold,
                                                                               [In] double minFractionalAreaTriangles);

        /// <summary>
        /// Finalizes the orthogonalization outer iteration, computing the new coefficients for grid adaption and the new face
        /// circumcenters (interactive mode).
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_finalize_inner_ortogonalization_iteration", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dFinalizeInnerOrtogonalizationIteration([In] int meshKernelId);

        /// <summary>
        /// Flips mesh2d edges, to optimize the mesh smoothness. This operation is usually performed after
        /// `mkernel_mesh2d_refine_based_on_samples` or `mkernel_mesh2d_refine_based_on_polygon`.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="isTriangulationRequired">
        /// The option to triangulate also non triangular cells (if activated squares becomes
        /// triangles)
        /// </param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="selectingPolygon">
        /// The polygon where to perform the edge flipping (num_coordinates = 0 for an empty
        /// polygon)
        /// </param>
        /// <param name="landBoundaries">
        /// The land boundaries to account for when flipping the edges (num_coordinates = 0 for no
        /// land boundaries)
        /// </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_flip_edges", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dFlipEdges([In] int meshKernelId,
                                                   [In] int isTriangulationRequired,
                                                   [In] int projectToLandBoundaryOption,
                                                   [In] ref GeometryListNative selectingPolygon,
                                                   [In] ref GeometryListNative landBoundaries);

        /// <summary>
        /// Get the coordinate of the closest vertex
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinateIn">The x coordinate of the input node</param>
        /// <param name="yCoordinateIn">The y coordinate of the input node</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="searchRadius">The radii where to search for mesh nodes</param>
        /// <param name="xCoordinateOut">The x coordinate of the found Mesh2DNative node</param>
        /// <param name="yCoordinateOut">The y coordinate of the found Mesh2DNative node</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_closest_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dGetClosestNode([In] int meshKernelId,
                                                      [In] double xCoordinateIn,
                                                      [In] double yCoordinateIn,
                                                      [In] double searchRadius,
                                                      [In] double xLowerLeftBoundingBox,
                                                      [In] double yLowerLeftBoundingBox,
                                                      [In] double xUpperRightBoundingBox,
                                                      [In] double yUpperRightBoundingBox,
                                                      [In][Out] ref double xCoordinateOut,
                                                      [In][Out] ref double yCoordinateOut);

        /// <summary>
        /// Gets the mesh state as a <see cref="Mesh2DNative"/> structure including faces information
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="mesh2DNative">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_data", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetData([In] int meshKernelId,
                                                 [In][Out] ref Mesh2DNative mesh2DNative);

        /// <summary>
        /// Gets the mesh2d dimensions <see cref="Mesh2DNative"/> structure
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="mesh2DNative">Grid data</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_dimensions", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2DGetDimensions([In] int meshKernelId,
                                                       [In][Out] ref Mesh2DNative mesh2DNative);

        /// <summary>
        /// Deletes the closest mesh edge within the search radius from the input point
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">x coordinate of the vertex</param>
        /// <param name="yCoordinate">y coordinate of the vertex</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="edgeIndex">The edge index</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetEdge([In] int meshKernelId,
                                                 [In] double xCoordinate,
                                                 [In] double yCoordinate,
                                                 [In] double xLowerLeftBoundingBox,
                                                 [In] double yLowerLeftBoundingBox,
                                                 [In] double xUpperRightBoundingBox,
                                                 [In] double yUpperRightBoundingBox,
                                                 [In][Out] ref int edgeIndex);

        /// <summary>
        /// Gets the indices of hanging edges. An hanging edge is an edge where one of the two nodes is not connected.
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="edges">Pointer to memory where the indices of the hanging edges will be stored</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_hanging_edges", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetHangingEdges([In] int meshKernelId, [In][Out] IntPtr edges);

        /// <summary>
        /// Gets the curvilinear location closet to a specific coordinate.
        /// </summary>
        /// 
        /// <param name="meshKernelId"> meshKernelId The id of the mesh state </param>
        /// <param name="xCoordinate">The input point coordinates</param>
        /// <param name="yCoordinate">The input point coordinates</param>
        /// <param name="locationType">The location type</param>
        /// <param name="boundingBox">The input bounding box</param>
        /// <param name="locationIndex">The location index</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_location_index", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetLocationIndex([In] int meshKernelId,
                                                          [In] double xCoordinate,
                                                          [In] double yCoordinate,
                                                          [In] int locationType,
                                                          [In] ref BoundingBoxNative boundingBox,
                                                          [In][Out] ref int locationIndex);

        /// <summary>
        /// Retrives the mesh boundary polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The output network boundary polygon</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_mesh_boundaries_as_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetMeshBoundariesAsPolygons([In] int meshKernelId, [In][Out] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Finds the mesh2d node closest to a point, within a search radius
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="xCoordinateIn">The x coordinate of the point</param>
        /// <param name="yCoordinateIn">The y coordinate of the point</param>
        /// <param name="searchRadius">The search radius</param>
        /// <param name="xLowerLeftBoundingBox">The x coordinate of the lower left corner of the bounding box</param>
        /// <param name="yLowerLeftBoundingBox">The y coordinate of the lower left corner of the bounding box</param>
        /// <param name="xUpperRightBoundingBox">The x coordinate of the upper right corner of the bounding box</param>
        /// <param name="yUpperRightBoundingBox">The y coordinate of the upper right corner of the bounding box</param>
        /// <param name="vertexIndex">the index of the closest vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_node_index", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dGetNodeIndex([In] int meshKernelId,
                                                    [In] double xCoordinateIn,
                                                    [In] double yCoordinateIn,
                                                    [In] double searchRadius,
                                                    [In] double xLowerLeftBoundingBox,
                                                    [In] double yLowerLeftBoundingBox,
                                                    [In] double xUpperRightBoundingBox,
                                                    [In] double yUpperRightBoundingBox,
                                                    [In][Out] ref int vertexIndex);

        /// <summary>
        /// Gets the selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="inside">Selection of the nodes inside the polygon (1) or outside (0)</param>
        /// <param name="selectedVerticesPtr">The selected vertices nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetSelectedVerticesInPolygon([In] int meshKernelId,
                                                                [In] ref GeometryListNative geometryListIn,
                                                                [In] int inside,
                                                                [In][Out] IntPtr selectedVerticesPtr);

        /// <summary>
        /// Gets the mass centers of obtuse mesh2d triangles. Obtuse triangles are those having one edge longer than the sum of the
        /// other two
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="result">The result of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_obtuse_triangles_mass_centers", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetObtuseTrianglesMassCenters([In] int meshKernelId, [In][Out] ref GeometryListNative result);

        /// <summary>
        /// Gets the orthogonality
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The orthogonality values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_orthogonality", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetOrthogonality([In] int meshKernelId, [In][Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Counts the number of selected mesh node indexes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="inside">Selection of the nodes inside the polygon (1) or outside (0)</param>
        /// <param name="numberOfMeshVertices">The number of selected nodes</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_nodes_in_polygons", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int CountVerticesInPolygon([In] int meshKernelId,
                                                          [In] ref GeometryListNative geometryListIn,
                                                          [In] int inside,
                                                          [In][Out] ref int numberOfMeshVertices);

        /// <summary>
        /// Counts the number of polygon nodes contained in the mesh boundary polygons computed in function
        /// `mkernel_mesh2d_get_mesh_boundaries_as_polygons`
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="numObtuseTriangles">The numObtuseTriangles of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_count_obtuse_triangles", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dCountObtuseTriangles([In] int meshKernelId, [In][Out] ref int numObtuseTriangles);

        /// <summary>
        /// Gets the small mesh2d flow edges. The flow edges are the edges connecting faces circumcenters
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="smallFlowEdgesThreshold">The smallFlowEdgesThreshold of the block to orthogonalize</param>
        /// <param name="result">The result of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_small_flow_edge_centers", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetSmallFlowEdgeCenters([In] int meshKernelId, [In] double smallFlowEdgesThreshold, [In][Out] ref GeometryListNative result);

        /// <summary>
        /// Gets the smoothness, expressed as the ratio between the values of two neighboring faces areas.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The smoothness values of each edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_get_smoothness", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dGetSmoothness([In] int meshKernelId, [In][Out] ref GeometryListNative geometryListIn);

        /// <summary>
        /// Orthogonalization initialization (first function to use in interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="projectToLandBoundaryOption">The option to determine how to snap to land boundaries</param>
        /// <param name="orthogonalizationParametersNative">The structure containing the user defined orthogonalization parameters</param>
        /// <param name="geometryListNativePolygon">The polygon where to perform the orthogonalization</param>
        /// <param name="geometryListNativeLandBoundaries">The land boundaries to account for in the orthogonalization process</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_initialize_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dInitializeOrthogonalization([In] int meshKernelId,
                                                                     [In] int projectToLandBoundaryOption,
                                                                     [In] ref OrthogonalizationParametersNative orthogonalizationParametersNative,
                                                                     [In] ref GeometryListNative geometryListNativePolygon,
                                                                     [In] ref GeometryListNative geometryListNativeLandBoundaries);

        /// <summary>
        /// Insert a new edge connecting
        /// <param name="startVertexIndex"/>
        /// and
        /// <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to connect</param>
        /// <param name="endVertexIndex">The index of the second vertex to connect</param>
        /// <param name="newEdgeIndex">The index of the new edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_insert_edge", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dInsertEdge([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex, [In][Out] ref int edgeIndex);

        /// <summary>
        /// Insert a new mesh2d edge from 2 coordinates
        /// <param name="startVertexIndex"/>
        /// <param name="meshKernelId"> The id of the mesh state
        /// <param name="firstNodeX">    The index of the first node to connect
        /// <param name="firstNodeY">      The index of the second node to connect
        /// <param name="secondNodeX">    The index of the first node to connect
        /// <param name="secondNodeY ">     The index of the second node to connect
        /// <param name="firstNodeIndex">      The index of the first node
        /// <param name="secondNodeIndex">      The index of the second node
        /// <param name="edgeIndex">      The index of the new generated edge
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_insert_edge_from_coordinates", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dInsertEdgeFromCoordinates([In] int meshKernelId,
                                                    [In] double firstNodeX,
                                                    [In] double firstNodeY,
                                                    [In] double secondNodeX,
                                                    [In] double secondNodeY,
                                                    [In][Out] ref int firstNodeIndex,
                                                    [In][Out] ref int secondNodeIndex,
                                                    [In][Out] ref int edgeIndex);

        /// <summary>
        /// Insert a new mesh2d edge connecting two nodes
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">The index of the first node to connect</param>
        /// <param name="yCoordinate">The index of the second node to connect</param>
        /// <param name="vertexIndex">The index of the new generated edge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_insert_node", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Mesh2dInsertNode([In] int meshKernelId, [In] double xCoordinate, [In] double yCoordinate, [In][Out] ref int vertexIndex);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_intersections_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dIntersectionsFromPolygon([In] int meshKernelId,
                                                                  [In] ref GeometryListNative boundaryPolygon,
                                                                  [In][Out] IntPtr edgeNodes,
                                                                  [In][Out] IntPtr edgeIndex,
                                                                  [In][Out] IntPtr edgeDistances,
                                                                  [In][Out] IntPtr segmentDistances,
                                                                  [In][Out] IntPtr segmentIndexes,
                                                                  [In][Out] IntPtr faceIndexes,
                                                                  [In][Out] IntPtr faceNumEdges,
                                                                  [In][Out] IntPtr faceEdgeIndex);

        /// <summary>
        /// Compute the global mesh with a given number of points along the longitude and latitude directions.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="numLongitudeNodes">The number of points along the longitude</param>
        /// <param name="numLatitudeNodes">The number of points along the latitude (half hemisphere)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_global", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeGlobal(int meshKernelId, int numLongitudeNodes, int numLatitudeNodes);

        /// <summary>
        /// Make a triangular grid in a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The polygon where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_triangular_mesh_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeTriangularMeshFromPolygon([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Make a triangular mesh from samples
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The samples where to triangulate</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_triangular_mesh_from_samples", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeTriangularMeshFromSamples([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Makes a rectangular mesh
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_rectangular_mesh", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeRectangularMesh([In] int meshKernelId,
                                                             [In] ref MakeGridParametersNative makeGridParameters);

        /// <summary>
        /// Makes a rectangular mesh from a polygon
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <param name="geometryList">The polygons to account for</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_rectangular_mesh_from_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeRectangularMeshFromPolygon([In] int meshKernelId,
                                                                        [In] ref MakeGridParametersNative makeGridParameters,
                                                                        [In] ref GeometryListNative geometryList);

        /// <summary>
        /// Makes a rectangular mesh on a defined extension
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="makeGridParameters">The structure containing the make grid parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_make_rectangular_mesh_on_extension", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMakeRectangularMeshOnExtension([In] int meshKernelId,
                                                                        [In] ref MakeGridParametersNative makeGridParameters);

        /// <summary>
        /// Merges vertices, effectively removing small edges. The merging distance is computed internally based on the minimum
        /// edge size.
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">
        /// The polygon where to perform the operation. If empty the operation is performed over
        /// the entire mesh
        /// </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_merge_nodes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMergeNodes([In] int meshKernelId, [In] ref GeometryListNative geometryListNative);

        /// <summary>
        /// Merges vertices within a defined distance in a polygon, effectively removing small edges
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">
        /// The polygon where to perform the operation. If empty the operation is performed over
        /// the entire mesh
        /// </param>
        /// <param name="mergingDistance">The distance below which two nodes will be merged</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_merge_nodes_with_merging_distance", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMergeNodesWithMergingDistance([In] int meshKernelId, [In] ref GeometryListNative geometryListNative, [In] double mergingDistance);

        /// <summary>
        /// Merges vertex
        /// <param name="startVertexIndex"/>
        /// to
        /// <param name="endVertexIndex"/>
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="startVertexIndex">The index of the first vertex to merge</param>
        /// <param name="endVertexIndex">The index of the second vertex to merge</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_merge_two_nodes", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMergeTwoNodes([In] int meshKernelId, [In] int startVertexIndex, [In] int endVertexIndex);

        /// <summary>
        /// Function to move a selected node to a new position
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="xCoordinate">The new x coordinate</param>
        /// <param name="xCoordinate">The new y coordinate</param>
        /// <param name="nodeIndex">The index of the mesh2d node to be moved</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_move_node", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dMoveNode([In] int meshKernelId,
                                                  [In] double xCoordinate,
                                                  [In] double yCoordinate,
                                                  [In] int nodeIndex);

        /// <summary>
        /// Prepare outer orthogonalization iteration (interactive mode)
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_prepare_outer_iteration_orthogonalization", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dPrepareOuterIterationOrthogonalization([In] int meshKernelId);

        /// <summary>
        /// Refine based on gridded samples
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="griddedSamplesNative">The gridded samples</param>
        /// <param name="meshRefinementParameters">The mesh refinement parameters</param>
        /// <param name="useNodalRefinement">Use nodal refinement</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_refine_based_on_gridded_samples", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRefineBasedOnGriddedSamples([In] int meshKernelId,
                                                                     [In] ref GriddedSamplesNative griddedSamplesNative,
                                                                     [In] ref MeshRefinementParametersNative meshRefinementParameters,
                                                                     [In] bool useNodalRefinement);

        /// <summary>
        /// Refine a grid based on polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The closed polygon where to perform the refinement</param>
        /// <param name="meshRefinementParametersNative">The settings for the mesh refinement algorithm</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_refine_based_on_polygon", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRefineBasedOnPolygon([In] int meshKernelId,
                                                              [In] ref GeometryListNative geometryListNative,
                                                              [In] ref MeshRefinementParametersNative meshRefinementParametersNative);

        /// <summary>
        /// Refine a grid based on the samples contained in the geometry list
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListNative">The sample set</param>
        /// <param name="relativeSearchRadius">
        /// The relative search radius relative to the face size, used for some interpolation
        /// algorithms
        /// </param>
        /// <param name="minimumNumSamples">The minimum number of samples used for some averaging algorithms</param>
        /// <param name="meshRefinementParameters">The mesh refinement parameters</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_refine_based_on_samples", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRefineBasedOnSamples([In] int meshKernelId,
                                                              [In] ref GeometryListNative geometryListNative,
                                                              [In] double relativeSearchRadius,
                                                              [In] int minimumNumSamples,
                                                              [In] ref MeshRefinementParametersNative meshRefinementParameters);

        /// <summary>
        /// Rotates a mesh2d about a given point by a given angle
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="centreX">>X-coordinate of the centre of rotation</param>
        /// <param name="centreY">Y-coordinate of the centre of rotation></param>
        /// <param name="angle">Angle of rotation in degrees</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_rotate", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dRotate([In] int meshKernelId,
                                                [In] double centreX,
                                                [In] double centreY,
                                                [In] double angle);

        /// <summary>
        /// Translates a mesh2d
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="translationX">X-component of the translation vector</param>
        /// <param name="translationY">Y-component of the translation vector</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_translate", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dTranslate([In] int meshKernelId,
                                                   [In] double translationX,
                                                   [In] double translationY);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dSet([In] int meshKernelId, [In] ref Mesh2DNative mesh2DNative);

        /// <summary>
        /// Snaps a mesh to a land boundary
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="selectingPolygon">The polygon where to perform the snapping</param>
        /// <param name="landBoundaries"> The input land boundaries</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_snap_to_landboundary", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dSnapToLandBoundary([In] int meshKernelId, [In] ref GeometryListNative selectingPolygon, [In] ref GeometryListNative landBoundaries);

        /// <summary>
        /// Get the double value used in the back-end library as separator and missing value
        /// </summary>
        /// <returns>The double missing value</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetSeparator();

        /// <summary>
        /// Gets the double value used to separate the inner part of a polygon from its outer part
        /// </summary>
        /// <returns>The double missing value used in meshkernel</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_get_inner_outer_separator", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double GetInnerOuterSeparator();

        /// <summary>
        /// Triangle interpolation
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="samples">The samples coordinates and values</param>
        /// <param name="locationType">The location type</param>
        /// <param name="results">The interpolation results with x and y coordinates</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_mesh2d_triangulation_interpolation", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Mesh2dTriangulationInterpolation([In] int meshKernelId, [In] ref GeometryListNative samples, [In] int locationType, [In][Out] ref GeometryListNative results);

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
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_network1d_compute_fixed_chainages", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Network1dComputeFixedChainages([In] int meshKernelId,
                                                                  [In] IntPtr fixedChainages,
                                                                  [In] int sizeFixedChainages,
                                                                  [In] double minFaceSize,
                                                                  [In] double fixedChainagesOffset);

        /// <summary>
        /// Network1d compute offsetted chainages
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="offset">The meshKernelId of the block to orthogonalize</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_network1d_compute_offsetted_chainages", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Network1dComputeOffsettedChainages([In] int meshKernelId, [In] double offset);

        /// <summary>
        /// Sets the Network1D state
        /// </summary>
        /// <param name="meshKernelId">The id of the mesh state</param>
        /// <param name="polylines">The polylines describing the network</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_network1d_set", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Network1dSet([In] int meshKernelId, [In] ref GeometryListNative polylines);

        /// <summary>
        /// Convert network chainages to mesh1d nodes and edges
        /// </summary>
        /// <param name="meshKernelId">
        /// The meshKernelId The id of the mesh state
        /// <param name="minFaceSize">
        /// The minFaceSize The minimum face size below which two nodes will be merged
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_network1d_to_mesh1d", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Network1dToMesh1d(int meshKernelId, double minFaceSize);

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
        internal static extern int PolygonCountOffset([In] int meshKernelId,
                                                      [In] ref GeometryListNative geometryListIn,
                                                      [In] int innerPolygon,
                                                      [In] double distance,
                                                      [In][Out] ref int numberOfPolygonVertices);

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
        internal static extern int PolygonCountEquidistantRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In][Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Count the number of vertices after linear refinement of a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="numberOfPolygonVertices">The number of vertices after refinement </param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_count_linear_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonCountLinearRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In][Out] ref int numberOfPolygonVertices);

        /// <summary>
        /// Selects points in polygons
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="inputPolygon">The polygon(s) used for selection</param>
        /// <param name="inputPoints">The points to select</param>
        /// <param name="selectedPoints">The selected points in the zCoordinates field (0.0 not selected, 1.0 selected)</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_get_included_points", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int GetPointsInPolygon([In] int meshKernelId, [In] ref GeometryListNative inputPolygon, [In] ref GeometryListNative inputPoints, [In][Out] ref GeometryListNative selectedPoints);

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
        internal static extern int PolygonGetOffset([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int innerPolygon, [In] double distance, [In][Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Equidistant refinement of a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="distance">The refinement distance</param>
        /// <param name="geometryListOut"></param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonEquidistantRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In] double distance, [In][Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Linear refinement of a polygon
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param>
        /// <param name="geometryListIn">The input polygons</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <param name="geometryListOut"></param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_linear_refine", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonLinearRefine([In] int meshKernelId, [In] ref GeometryListNative geometryListIn, [In] int firstIndex, [In] int secondIndex, [In][Out] ref GeometryListNative geometryListOut);

        /// <summary>
        /// Snaps part of a polygon to a land boundary
        /// </summary>
        /// <param name="meshKernelId">Id of the mesh state</param
        /// <param name="landboundaries">The land boundaries</param>
        /// <param name="polygon">The input polygon</param>
        /// <param name="firstIndex">The index of the first vertex</param>
        /// <param name="secondIndex">The index of the second vertex</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_polygon_snap_to_landboundary", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PolygonSnapToLandBoundary([In] int meshKernelId,
                                                             [In] ref GeometryListNative landboundaries,
                                                             [In][Out] ref GeometryListNative polygon,
                                                             [In] int firstIndex,
                                                             [In] int secondIndex);

        /// <summary>
        /// Redo editing action
        /// </summary>
        /// <param name="redone">If the editing action has been re-done</param>
        /// <param name="meshKernelId">The mesh kernel id related to the redo action</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_redo_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int RedoState([In][Out] ref bool redone, [In] ref int meshKernelId);

        /// <summary>
        /// Redo editing action
        /// </summary>
        /// <param name="undone">If the editing action has been un-done</param>
        /// <param name="meshKernelId">The mesh kernel id related to the undo action</param>
        /// <returns>Error code</returns>
        [DllImport(MeshKernelDllName, EntryPoint = "mkernel_undo_state", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int UndoState([In][Out] ref bool undone, [In] ref int meshKernelId);
    }
}