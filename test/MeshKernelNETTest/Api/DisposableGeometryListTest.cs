using System;
using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableGeometryListTest
    {
        [Test]
        public void WhenCallingCreateGeometryListNative_AGeometryListNativeShouldBeCreated()
        {
            //Arrange
            var list = new DisposableGeometryList
            {
                XCoordinates = new[] { 0.0, 1.0, 2.0, -999.0, 0.0, 1.0, 2.0 },
                YCoordinates = new[] { 0.0, 1.0, 2.0, -999.0, 0.0, 1.0, 2.0 },
                Values = new[] { 0.0, 1.0, 2.0, -999.0, 0.0, 1.0, 2.0 },
                NumberOfCoordinates = 7,
                GeometrySeparator = -999.0
            };

            using (list)
            {
                // Act
                GeometryListNative nativeGeometryList = list.CreateNativeObject();

                // Assert

                Assert.That(nativeGeometryList.numberOfCoordinates, Is.EqualTo(list.NumberOfCoordinates));
                Assert.That(nativeGeometryList.xCoordinates, Is.Not.EqualTo(IntPtr.Zero));
                Assert.That(nativeGeometryList.yCoordinates, Is.Not.EqualTo(IntPtr.Zero));
                Assert.That(nativeGeometryList.zCoordinates, Is.Not.EqualTo(IntPtr.Zero));
            }
        }

        [Test]
        public void UpdateFromNativeObjectShouldPopulateGeometryList()
        {
            // Arrange
            var managedList = new DisposableGeometryList();
            var nativeList = new DisposableGeometryList
            {
                XCoordinates = new[] { 1.0, 2.0, 3.0 },
                YCoordinates = new[] { 4.0, 5.0, 6.0 },
                Values = new[] { 7.0, 8.0, 9.0 },
                NumberOfCoordinates = 3,
                GeometrySeparator = -999.0,
                InnerOuterSeparator = -998.0
            };

            using (nativeList)
            {
                GeometryListNative nativeObject = nativeList.CreateNativeObject();

                // Act
                managedList.UpdateFromNativeObject(ref nativeObject);

                // Assert
                Assert.That(managedList.XCoordinates, Is.EqualTo(new[] { 1.0, 2.0, 3.0 }));
                Assert.That(managedList.YCoordinates, Is.EqualTo(new[] { 4.0, 5.0, 6.0 }));
                Assert.That(managedList.Values, Is.EqualTo(new[] { 7.0, 8.0, 9.0 }));
                Assert.That(managedList.NumberOfCoordinates, Is.EqualTo(3));
                Assert.That(managedList.GeometrySeparator, Is.EqualTo(-999.0));
                Assert.That(managedList.InnerOuterSeparator, Is.EqualTo(-998.0));
            }
        }
    }
}