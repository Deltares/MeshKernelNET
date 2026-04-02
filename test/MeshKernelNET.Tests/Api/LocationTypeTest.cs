using MeshKernelNET.Api;
using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNET.Tests.Api
{
    [TestFixture]
    public class LocationTypeTest
    {
        [Test]
        public void EnsureEnumValuesAreConsistent()
        {
            int enumValue = -1;

            Assert.Multiple(() =>
            {
                Assert.That(MeshKernelDll.GetEdgesLocationType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(LocationType.Edges.ToLocationId()));

                Assert.That(MeshKernelDll.GetNodesLocationType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(LocationType.Nodes.ToLocationId()));

                Assert.That(MeshKernelDll.GetFacesLocationType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(LocationType.Faces.ToLocationId()));
            });
        }
    }
}