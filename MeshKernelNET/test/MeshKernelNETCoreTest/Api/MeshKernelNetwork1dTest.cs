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
                var mesh1D = new DisposableMesh1D();
                try
                {
                    id = api.AllocateState(0);

                    var separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                    network1d.GeometrySeparator = separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.AreEqual(api.Network1dSet(id, network1d), 0);

                    double[] fixedChainages = { 5.0, separator, 5.0 };
                    double minFaceSize = 0.01;
                    double fixedChainagesOffset = 10.0;

                    Assert.AreEqual(api.Network1dComputeFixedChainages(id, fixedChainages, minFaceSize, fixedChainagesOffset), 0);
                    Assert.AreEqual(api.Network1dToMesh1d(id, minFaceSize), 0);

                    Assert.AreEqual(api.Mesh1dGetData(id, out mesh1D), 0);

                    Assert.AreEqual(mesh1D.NumNodes, 6);
                    Assert.AreEqual(mesh1D.NumEdges, 4);
                }
                finally
                {
                    api.DeallocateState(id);
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

                    var separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                    network1d.GeometrySeparator = separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.AreEqual(api.Network1dSet(id, network1d), 0);

                    double minFaceSize = 0.01;
                    double fixedChainagesOffset = 1.0;

                    Assert.AreEqual(api.Network1dComputeOffsettedChainages(id, fixedChainagesOffset), 0);
                    Assert.AreEqual(api.Network1dToMesh1d(id, minFaceSize), 0);
                    Assert.AreEqual(api.Mesh1dGetData(id, out mesh1D), 0);

                    Assert.AreEqual(mesh1D.NumEdges, 40);
                    Assert.AreEqual(mesh1D.NumNodes, 41);
                }
                finally
                {
                    api.DeallocateState(id);
                    mesh1D.Dispose();
                }
            }
        }

    }

}
