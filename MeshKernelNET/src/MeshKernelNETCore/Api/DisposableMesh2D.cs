using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using GeoAPI.Geometries;
using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{

    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableMesh2D : DisposableMeshObject
    {

        [ProtoMember(1)]
        public int[] edgeNodes;

        [ProtoMember(2)]
        public int[] faceNodes;

        [ProtoMember(3)]
        public int[] nodesPerFace;

        [ProtoMember(4)]
        public double[] nodeX;

        [ProtoMember(5)]
        public double[] nodeY;

        [ProtoMember(6)]
        public double[] edgeX;

        [ProtoMember(7)]
        public double[] edgeY;

        [ProtoMember(8)]
        public double[] faceX;

        [ProtoMember(9)]
        public double[] faceY;

        [ProtoMember(10)]
        public int numNodes;

        [ProtoMember(11)]
        public int numEdges;

        [ProtoMember(12)]
        public int numFaces;

        [ProtoMember(13)]
        public int numFaceNodes;

        private bool disposed = false;

        public DisposableMesh2D() { }

        public DisposableMesh2D(int nNodes, int nEdges, int nFaces, int nFaceNodes)
        {
            numNodes = nNodes;
            numEdges = nEdges;
            numFaces = nFaces;
            numFaceNodes = nFaceNodes;

            edgeNodes = new int[numEdges * 2];
            faceNodes = new int[numFaceNodes];
            nodesPerFace = new int[numFaces];
            nodeX = new double[numNodes];
            nodeY = new double[numNodes];
            edgeX = new double[numEdges];
            edgeY = new double[numEdges];
            faceX = new double[numFaces];
            faceY = new double[numFaces];
        }

        public Mesh2D CreateMesh2D()
        {
            if (!IsMemoryPinned)
            {
                PinMemory();
            }

            return new Mesh2D
            {
                edge_nodes = GetPinnedObjectPointer(edgeNodes),
                face_nodes = GetPinnedObjectPointer(faceNodes),
                nodes_per_face = GetPinnedObjectPointer(nodesPerFace),
                node_x = GetPinnedObjectPointer(nodeX),
                node_y = GetPinnedObjectPointer(nodeY),
                edge_x = GetPinnedObjectPointer(faceX),
                edge_y = GetPinnedObjectPointer(faceY),
                face_x = GetPinnedObjectPointer(faceX),
                face_y = GetPinnedObjectPointer(faceY),
                num_nodes = numNodes,
                num_edges = numEdges,
                num_faces = numFaces,
                num_face_nodes = numFaceNodes
            };
        }

        ~DisposableMesh2D()
        {
            Dispose(false);
        }
    }

}
