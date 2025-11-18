using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableCurvilinearGridTest
    {
        [Test]
        public void UpdateFromNativeObjectShouldPopulateCurvilinearGrid()
        {
            // Arrange
            var managedGrid = new DisposableCurvilinearGrid();
            var nativeGrid = new DisposableCurvilinearGrid(3, 3)
            {
                NodeX = new[] { 1.0, 2.0, 3.0, 1.0, 2.0, 3.0, 1.0, 2.0, 3.0 },
                NodeY = new[] { 4.0, 5.0, 6.0, 4.0, 5.0, 6.0, 4.0, 5.0, 6.0 }
            };

            using (nativeGrid)
            {
                CurvilinearGridNative nativeObject = nativeGrid.CreateNativeObject();

                // Act
                managedGrid.UpdateFromNativeObject(ref nativeObject);

                // Assert
                Assert.That(managedGrid.NodeX, Is.EqualTo(new[] { 1.0, 2.0, 3.0, 1.0, 2.0, 3.0, 1.0, 2.0, 3.0 }));
                Assert.That(managedGrid.NodeY, Is.EqualTo(new[] { 4.0, 5.0, 6.0, 4.0, 5.0, 6.0, 4.0, 5.0, 6.0 }));
                Assert.That(managedGrid.NumM, Is.EqualTo(3));
                Assert.That(managedGrid.NumN, Is.EqualTo(3));
            }
        }
    }
}