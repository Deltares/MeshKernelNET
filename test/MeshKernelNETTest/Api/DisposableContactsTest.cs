using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableContactsTest
    {
        [Test]
        public void UpdateFromNativeObjectShouldPopulateContacts()
        {
            // Arrange
            var managedContacts = new DisposableContacts();
            var nativeContacts = new DisposableContacts
            {
                Mesh1dIndices = new[] { 1, 2, 3 },
                Mesh2dIndices = new[] { 4, 5, 6 },
                NumContacts = 3,
            };

            using (nativeContacts)
            {
                ContactsNative nativeObject = nativeContacts.CreateNativeObject();

                // Act
                managedContacts.UpdateFromNativeObject(ref nativeObject);

                // Assert
                Assert.That(managedContacts.Mesh1dIndices, Is.EqualTo(new[] { 1, 2, 3 }));
                Assert.That(managedContacts.Mesh2dIndices, Is.EqualTo(new[] { 4, 5, 6 }));
                Assert.That(managedContacts.NumContacts, Is.EqualTo(3));
            }
        }
    }
}