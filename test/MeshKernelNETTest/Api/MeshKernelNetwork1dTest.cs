using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNET.Api;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETNetwork1DTests")]
    public class MeshKernelNETNetwork1DTests
    {
        
        [Test]
        public void FailingTestIn1D2D()
        {
            var unstructuredGrid = TestUtilityFunctions.CreateMesh2D(11, 3, 10, 10, -5, -5);
            
            // Create mask 
            bool[] mask1DMesh = new bool[6]{false,false,true,true,true,true};
            var intMask1dMesh = mask1DMesh.Select(m => m ? 1 : 0).ToArray();
            var maskHandle = GCHandle.Alloc(intMask1dMesh, GCHandleType.Pinned);
            var mask1DMeshPtr = maskHandle.AddrOfPinnedObject();

            DisposableGeometryList gullyData = new DisposableGeometryList();
            gullyData.XCoordinates = new double[2];
            gullyData.YCoordinates = new double[2];
            gullyData.Values = new double[2];

            gullyData.XCoordinates[0] = 10;
            gullyData.YCoordinates[0] = 5;
            gullyData.Values[0] = double.NaN;
            gullyData.XCoordinates[1] = -999;
            gullyData.YCoordinates[1] = -999;
            gullyData.Values[1] = -999;
            gullyData.NumberOfCoordinates = 2;

            var mesh1D = new DisposableMesh1D(6, 3);
            mesh1D.NodeX[0] = 0;
            mesh1D.NodeX[1] = 100;
            mesh1D.NodeX[2] = 0;
            mesh1D.NodeX[3] = 100;
            mesh1D.NodeX[4] = 0;
            mesh1D.NodeX[5] = 100;
            
            mesh1D.NodeY[0] = 0;
            mesh1D.NodeY[1] = 0;
            mesh1D.NodeY[2] = 10;
            mesh1D.NodeY[3] = 10;
            mesh1D.NodeY[4] = 20;
            mesh1D.NodeY[5] = 20;

            mesh1D.EdgeNodes[0] = 0;
            mesh1D.EdgeNodes[0] = 1;
            mesh1D.EdgeNodes[0] = 2;
            mesh1D.EdgeNodes[0] = 3;
            mesh1D.EdgeNodes[0] = 4;
            mesh1D.EdgeNodes[0] = 5;
            

            using (var api = new MeshKernelApi())
            {
                var id = api.AllocateState(0);

                api.Mesh1dSet(id, mesh1D);
                api.Mesh2dSet(id, unstructuredGrid);
                
                var res = api.ContactsComputeWithPoints(id,mask1DMeshPtr,gullyData);
                
                // 0 means success
                Assert.That(res, Is.EqualTo(0));
                
                DisposableContacts contacts;
                var bla = api.ContactsGetData(id, out DisposableContacts disposableContacts);
                Assert.That(bla, Is.EqualTo(0));
                
                Assert.That(disposableContacts, Is.Not.Null);
                Assert.That(disposableContacts.NumContacts, Is.EqualTo(1));
            }
        }
        
        
        [Test]
        public void Network1dComputeFixedChainagesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var network1d = new DisposableGeometryList())
            {
                var id = 0;
                var mesh1D = new DisposableMesh1D();
                try
                {
                    id = api.AllocateState(0);

                    double separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                    network1d.GeometrySeparator = separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.That(api.Network1dSet(id, network1d), Is.EqualTo(0));

                    double[] fixedChainages = { 5.0, separator, 5.0 };
                    var minFaceSize = 0.01;
                    var fixedChainagesOffset = 10.0;

                    Assert.That(api.Network1dComputeFixedChainages(id, fixedChainages, minFaceSize, fixedChainagesOffset), Is.EqualTo(0));
                    Assert.That(api.Network1dToMesh1d(id, minFaceSize), Is.EqualTo(0));

                    Assert.That(api.Mesh1dGetData(id, out mesh1D), Is.EqualTo(0));

                    Assert.That(mesh1D.NumNodes, Is.EqualTo(6));
                    Assert.That(mesh1D.NumEdges, Is.EqualTo(4));
                }
                finally
                {
                    api.ClearState();
                    mesh1D.Dispose();
                }
            }
        }

        [Test]
        public void Network1dComputeOffsettedChainagesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var network1d = new DisposableGeometryList())
            {
                var id = 0;
                var mesh1D = new DisposableMesh1D();
                try
                {
                    id = api.AllocateState(0);

                    double separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                    network1d.GeometrySeparator = separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.That(api.Network1dSet(id, network1d), Is.EqualTo(0));

                    var minFaceSize = 0.01;
                    var fixedChainagesOffset = 1.0;

                    Assert.That(api.Network1dComputeOffsettedChainages(id, fixedChainagesOffset), Is.EqualTo(0));
                    Assert.That(api.Network1dToMesh1d(id, minFaceSize), Is.EqualTo(0));
                    Assert.That(api.Mesh1dGetData(id, out mesh1D), Is.EqualTo(0));

                    Assert.That(mesh1D.NumEdges, Is.EqualTo(40));
                    Assert.That(mesh1D.NumValidNodes, Is.EqualTo(41));
                }
                finally
                {
                    api.ClearState();
                    mesh1D.Dispose();
                }
            }
        }
    }
}