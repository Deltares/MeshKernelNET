using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    [Category("MeshKernelNETNetwork1DTests")]
    public class MeshKernelNETNetwork1DTests
    {
        [Test]
        public void Network1dComputeFixedChainagesThroughAPI()
        {
            // Setup
            using (var api = new MeshKernelApi())
            using (var network1d = new DisposableGeometryList())
            {
                var id = 0;
                try
                {
                    id = api.AllocateState(0);

                    var separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0};
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0};
                    network1d.GeometrySeparator =  separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.IsTrue(api.Network1dSet(id, network1d));

                    double [] fixedChainages ={ 5.0, separator, 5.0 };
                    double minFaceSize = 0.01;
                    double fixedChainagesOffset = 10.0;

                    Assert.IsTrue(api.Network1dComputeFixedChainages(id, fixedChainages,  minFaceSize, fixedChainagesOffset));
                    Assert.IsTrue(api.Network1dToMesh1d(id, minFaceSize));

                    var mesh1D = new DisposableMesh1D();
                    Assert.IsTrue(api.Mesh1dGetData(id, out mesh1D));

                    Assert.AreEqual(mesh1D.NumNodes, 6);
                    Assert.AreEqual(mesh1D.NumEdges, 4);
                    mesh1D.Dispose();
                }
                finally
                {
                    api.DeallocateState(id);
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
                 try
                 {
                     id = api.AllocateState(0);
                 
                     var separator = api.GetSeparator();
                     network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                     network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                     network1d.GeometrySeparator = separator;
                     network1d.NumberOfCoordinates = 7;
                 
                     Assert.IsTrue(api.Network1dSet(id, network1d));
                 
                     double minFaceSize = 0.01;
                     double fixedChainagesOffset = 1.0;
                 
                     Assert.IsTrue(api.Network1dComputeOffsettedChainages(id, fixedChainagesOffset));
                     Assert.IsTrue(api.Network1dToMesh1d(id, minFaceSize));
                 
                     var mesh1D = new DisposableMesh1D();
                     Assert.IsTrue(api.Mesh1dGetData(id, out mesh1D));
                 
                     Assert.AreEqual(mesh1D.NumEdges, 40);
                     Assert.AreEqual(mesh1D.NumNodes, 41);
                     
                     mesh1D.Dispose();
                 }
                 finally
                 {
                     api.DeallocateState(id);
                 }
            }
        }

    }

}
