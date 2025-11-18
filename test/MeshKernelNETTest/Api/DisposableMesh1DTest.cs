using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableMesh1DTest
    {
        [Test]
        public void UpdateFromNativeObjectShouldPopulateMesh1D()
        {
            // Arrange
            var managedMesh = new DisposableMesh1D();
            var nativeMesh = new DisposableMesh1D(4, 3)
            {
                NodeX = new[] { 0.0, 1.0, 2.0, 3.0 },
                NodeY = new[] { 0.0, 0.0, 0.0, 0.0 },
                EdgeNodes = new[] { 0, 1, 1, 2, 2, 3 },
                NumNodes = 4,
                NumEdges = 6,
                NumValidNodes = 4,
                NumValidEdges = 6
            };

            using (nativeMesh)
            {
                Mesh1DNative nativeObject = nativeMesh.CreateNativeObject();

                // Act
                managedMesh.UpdateFromNativeObject(ref nativeObject);

                // Assert
                Assert.That(managedMesh.NodeX, Is.EqualTo(new[] { 0.0, 1.0, 2.0, 3.0 }));
                Assert.That(managedMesh.NodeY, Is.EqualTo(new[] { 0.0, 0.0, 0.0, 0.0 }));
                Assert.That(managedMesh.EdgeNodes, Is.EqualTo(new[] { 0, 1, 1, 2, 2, 3 }));
                Assert.That(managedMesh.NumNodes, Is.EqualTo(4));
                Assert.That(managedMesh.NumEdges, Is.EqualTo(6));
                Assert.That(managedMesh.NumValidNodes, Is.EqualTo(4));
                Assert.That(managedMesh.NumValidEdges, Is.EqualTo(6));
            }
        }
    }
}