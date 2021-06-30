using MeshKernelNETCore.Api;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Api
{
    [TestFixture]
    public class InterpolationParametersTest
    {
        [Test]
        public void CreateDefault_ExpectedResult()
        {
            // Call
            var parameters = InterpolationParameters.CreateDefault();

            // Assert
            Assert.That(parameters.InterpolationType, Is.EqualTo(1));
            Assert.That(parameters.DisplayInterpolationProcess, Is.EqualTo(0));
            Assert.That(parameters.MaxNumberOfRefinementIterations, Is.EqualTo(3));
            Assert.That(parameters.AveragingMethod, Is.EqualTo(1));
            Assert.That(parameters.MinimumNumberOfPoints, Is.EqualTo(1));
            Assert.That(parameters.RelativeSearchRadius, Is.EqualTo(1.01));
            Assert.That(parameters.InterpolateTo, Is.EqualTo(3));
            Assert.That(parameters.RefineIntersected, Is.False);
            Assert.That(parameters.UseMassCenters, Is.True);
        }
    }
}