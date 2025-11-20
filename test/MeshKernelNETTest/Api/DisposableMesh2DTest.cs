using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableMesh2DTest
    {
        [Test]
        public void UpdateFromNativeObjectShouldPopulateMesh2D()
        {
            // Arrange
            var managedMesh = new DisposableMesh2D();
            var nativeMesh = new DisposableMesh2D(4, 4, 1, 4, 4, 4)
            {
                NodeX = new[] { 0.0, 1.0, 1.0, 0.0 },
                NodeY = new[] { 0.0, 0.0, 1.0, 1.0 },
                EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 0 },
                EdgeX = new[] { 0.5, 1.0, 0.5, 0.0 },
                EdgeY = new[] { 0.0, 0.5, 1.0, 0.5 },
                FaceX = new[] { 0.5 },
                FaceY = new[] { 0.5 },
                FaceNodes = new[] { 0, 1, 2, 3 },
                NodesPerFace = new[] { 4 },
                NumValidNodes = 4,
                NumValidEdges = 4
            };

            using (nativeMesh)
            {
                Mesh2DNative nativeObject = nativeMesh.CreateNativeObject();

                // Act
                managedMesh.UpdateFromNativeObject(ref nativeObject, true);

                // Assert
                Assert.That(managedMesh.NodeX, Is.EqualTo(new[] { 0.0, 1.0, 1.0, 0.0 }));
                Assert.That(managedMesh.NodeY, Is.EqualTo(new[] { 0.0, 0.0, 1.0, 1.0 }));
                Assert.That(managedMesh.EdgeNodes, Is.EqualTo(new[] { 0, 1, 1, 2, 2, 3, 3, 0 }));
                Assert.That(managedMesh.FaceX, Is.EqualTo(new[] { 0.5 }));
                Assert.That(managedMesh.FaceY, Is.EqualTo(new[] { 0.5 }));
                Assert.That(managedMesh.FaceNodes, Is.EqualTo(new[] { 0, 1, 2, 3 }));
                Assert.That(managedMesh.NodesPerFace, Is.EqualTo(new[] { 4 }));
                Assert.That(managedMesh.NumNodes, Is.EqualTo(4));
                Assert.That(managedMesh.NumEdges, Is.EqualTo(4));
                Assert.That(managedMesh.NumFaces, Is.EqualTo(1));
                Assert.That(managedMesh.NumValidNodes, Is.EqualTo(4));
                Assert.That(managedMesh.NumValidEdges, Is.EqualTo(4));
            }
        }

        [Test]
        public void UpdateFromNativeObjectWithoutCellInformationShouldNotPopulateFaceData()
        {
            // Arrange
            var managedMesh = new DisposableMesh2D();
            var nativeMesh = new DisposableMesh2D(4, 4, 1, 4, 4, 4)
            {
                NodeX = new[] { 0.0, 1.0, 1.0, 0.0 },
                NodeY = new[] { 0.0, 0.0, 1.0, 1.0 },
                EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 0 },
                EdgeX = new[] { 0.5, 1.0, 0.5, 0.0 },
                EdgeY = new[] { 0.0, 0.5, 1.0, 0.5 },
                FaceX = new[] { 0.5 },
                FaceY = new[] { 0.5 },
                FaceNodes = new[] { 0, 1, 2, 3 },
                NodesPerFace = new[] { 4 },
                NumValidNodes = 4,
                NumValidEdges = 4
            };

            using (nativeMesh)
            {
                Mesh2DNative nativeObject = nativeMesh.CreateNativeObject();

                // Act
                managedMesh.UpdateFromNativeObject(ref nativeObject, false);

                // Assert
                Assert.That(managedMesh.NodeX, Is.EqualTo(new[] { 0.0, 1.0, 1.0, 0.0 }));
                Assert.That(managedMesh.NodeY, Is.EqualTo(new[] { 0.0, 0.0, 1.0, 1.0 }));
                Assert.That(managedMesh.EdgeNodes, Is.EqualTo(new[] { 0, 1, 1, 2, 2, 3, 3, 0 }));
                Assert.That(managedMesh.NumNodes, Is.EqualTo(4));
                Assert.That(managedMesh.NumEdges, Is.EqualTo(4));
                Assert.That(managedMesh.FaceNodes, Is.Null);
                Assert.That(managedMesh.NodesPerFace, Is.Null);
            }
        }
    }
}