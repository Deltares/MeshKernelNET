using System;
using System.Runtime.InteropServices;
using MeshKernelNET.Api;
using NUnit.Framework;
using static MeshKernelNETTest.Api.TestUtilityFunctions;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETContactsTests")]
    public class MeshKernelContactsTest
    {
        [Test]
        public void ContactsComputeSingleContactsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    var projectionFactor = 0.0;
                    Assert.That(api.ContactsComputeSingle(id, onedNodeMaskPinnedAddress, geometryListIn, projectionFactor), Is.EqualTo(0));

                    var contacts = new DisposableContacts();
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    Assert.That(contacts.NumContacts, Is.GreaterThan(0));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                }
            }
        }

        [Test]
        public void ContactsComputeMultipleThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeMultiple(id, onedNodeMaskPinnedAddress), Is.EqualTo(0));

                    var contacts = new DisposableContacts();
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));
                    Assert.That(contacts.NumContacts, Is.GreaterThan(0));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                }
            }
        }

        [Test]
        public void ContactsComputeWithPolygonsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));

                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeWithPolygons(id, onedNodeMaskPinnedAddress, geometryListIn), Is.EqualTo(0));

                    var contacts = new DisposableContacts();
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));

                    // Only one contact is generated, because only one polygon is present 
                    Assert.That(contacts.NumContacts, Is.EqualTo(1));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                }
            }
        }

        [Test]
        public void ContactsComputeWithPointsThroughAPI()
        {
            //Setup
            using (DisposableMesh2D mesh = CreateMesh2D(4, 4, 10, 10))
            using (var mesh1d = new DisposableMesh1D())
            using (var api = new MeshKernelApi())
            using (var geometryListIn = new DisposableGeometryList())
            {
                var id = 0;
                var onedNodeMask = new[] { 1, 1, 1, 1, 1, 1, 1 };
                GCHandle onedNodeMaskPinned = GCHandle.Alloc(onedNodeMask, GCHandleType.Pinned);
                try
                {
                    id = api.AllocateState(0);

                    Assert.That(api.Mesh2dSet(id, mesh), Is.EqualTo(0));

                    mesh1d.NodeX = new[] { 1.73493900000000, 2.35659313023165, 5.38347452702839, 14.2980910429074, 22.9324017677239, 25.3723169493137, 25.8072280000000 };
                    mesh1d.NodeY = new[] { -7.6626510000000, 1.67281447902331, 10.3513746546384, 12.4797224193970, 15.3007317677239, 24.1623588554512, 33.5111870000000 };
                    mesh1d.NumNodes = 7;

                    mesh1d.EdgeNodes = new[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6 };
                    mesh1d.NumEdges = 6;
                    Assert.That(api.Mesh1dSet(id, mesh1d), Is.EqualTo(0));

                    geometryListIn.GeometrySeparator = api.GetSeparator();
                    geometryListIn.XCoordinates = new[] { 5.0, 25.0, 25.0, 5.0 };
                    geometryListIn.YCoordinates = new[] { 5.0, 5.0, 25.0, 25.0 };
                    geometryListIn.Values = new[] { 0.0, 0.0, 0.0, 0.0 };
                    geometryListIn.NumberOfCoordinates = geometryListIn.XCoordinates.Length;

                    IntPtr onedNodeMaskPinnedAddress = onedNodeMaskPinned.AddrOfPinnedObject();
                    Assert.That(api.ContactsComputeWithPoints(id, onedNodeMaskPinnedAddress, geometryListIn), Is.EqualTo(0));

                    var contacts = new DisposableContacts();
                    Assert.That(api.ContactsGetData(id, out contacts), Is.EqualTo(0));

                    // Four contacts are generated, one for each point
                    Assert.That(contacts.NumContacts, Is.EqualTo(4));
                }
                finally
                {
                    onedNodeMaskPinned.Free();
                    api.ClearState();
                }
            }
        }
    }
}