using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    public class CurvilinearParametersTest
    {
        [Test]
        public void CreateDefault_ReturnsCorrectCurvilinearParameters()
        {
            // Call
            var parameters = CurvilinearParameters.CreateDefault();

            // Assert
            Assert.That(parameters.MRefinement, Is.EqualTo(2000));
            Assert.That(parameters.NRefinement, Is.EqualTo(40));
            Assert.That(parameters.SmoothingIterations, Is.EqualTo(10));
            Assert.That(parameters.SmoothingParameter, Is.EqualTo(0.5));
            Assert.That(parameters.AttractionParameter, Is.EqualTo(0.0));
        }
    }
}