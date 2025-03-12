using MeshKernelNET.Api;
using NUnit.Framework;

// Added alias to still be able to use the classic assert functions
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace MeshKernelNETTest.Api
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

                    double separator = api.GetSeparator();
                    network1d.XCoordinates = new[] { 0.0, 10.0, 20.0, separator, 10.0, 10.0, 10.0 };
                    network1d.YCoordinates = new[] { 0.0, 0.0, 0.0, separator, -10.0, 0.0, 10.0 };
                    network1d.GeometrySeparator = separator;
                    network1d.NumberOfCoordinates = 7;

                    Assert.AreEqual(0, api.Network1dSet(id, network1d));

                    double[] fixedChainages = { 5.0, separator, 5.0 };
                    var minFaceSize = 0.01;
                    var fixedChainagesOffset = 10.0;

                    Assert.AreEqual(0, api.Network1dComputeFixedChainages(id, fixedChainages, minFaceSize, fixedChainagesOffset));
                    Assert.AreEqual(0, api.Network1dToMesh1d(id, minFaceSize));

                    Assert.AreEqual(0, api.Mesh1dGetData(id, out mesh1D));

                    Assert.AreEqual(6, mesh1D.NumNodes);
                    Assert.AreEqual(4, mesh1D.NumEdges);
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

                    Assert.AreEqual(0, api.Network1dSet(id, network1d));

                    var minFaceSize = 0.01;
                    var fixedChainagesOffset = 1.0;

                    Assert.AreEqual(0, api.Network1dComputeOffsettedChainages(id, fixedChainagesOffset));
                    Assert.AreEqual(0, api.Network1dToMesh1d(id, minFaceSize));
                    Assert.AreEqual(0, api.Mesh1dGetData(id, out mesh1D));

                    Assert.AreEqual(40, mesh1D.NumEdges);
                    Assert.AreEqual(41, mesh1D.NumValidNodes);
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