using MeshKernelNET.Api;
using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class PropertyTypeTest
    {
        [Test]
        public void EnsureEnumValuesAreConsistent()
        {
            int enumValue = -1;

            Assert.Multiple(() =>
            {
                Assert.That(MeshKernelDll.Mesh2dGetOrthogonalityPropertyType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(PropertyType.Orthogonality.ToPropertyId()));

                Assert.That(MeshKernelDll.Mesh2dGetEdgeLengthPropertyType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(PropertyType.EdgeLength.ToPropertyId()));

                Assert.That(MeshKernelDll.Mesh2dGetFaceCircumcenterPropertyType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(PropertyType.FaceCircumcenter.ToPropertyId()));
                
                Assert.That(MeshKernelDll.Mesh2dGetNetLinkContourPolygonPropertyType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(PropertyType.NetlinkContourPolygon.ToPropertyId()));

                Assert.That(MeshKernelDll.Mesh2dGetFaceBoundsPropertyType(ref enumValue), Is.EqualTo(0));
                Assert.That(enumValue, Is.EqualTo(PropertyType.FaceBounds.ToPropertyId()));
            });
        }
    }
}