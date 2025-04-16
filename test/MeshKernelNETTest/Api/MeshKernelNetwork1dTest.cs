using MeshKernelNET.Api;
using NUnit.Framework;

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