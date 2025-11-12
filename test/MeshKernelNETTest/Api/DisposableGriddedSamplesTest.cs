using System;
using MeshKernelNET.Api;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableGriddedSamplesTests
    {
        [Test]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var samples = new DisposableGriddedSamples(10, 20, 1.0, 2.0, 0.5, InterpolationType.Float);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.NumX, Is.EqualTo(10));
                Assert.That(samples.NumY, Is.EqualTo(20));
                Assert.That(samples.OriginX, Is.EqualTo(1.0));
                Assert.That(samples.OriginY, Is.EqualTo(2.0));
                Assert.That(samples.CellSize, Is.EqualTo(0.5));
                Assert.That(samples.ValueType, Is.EqualTo(InterpolationType.Float));
            }
        }

        [Test]
        public void Constructor_WithShortType_InitializesShortArray()
        {
            var samples = new DisposableGriddedSamples(5, 4, 0.0, 0.0, 1.0, InterpolationType.Short);

            Assert.That(samples.ShortValues, Is.Not.Null);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.ShortValues, Has.Length.EqualTo(20));
                Assert.That(samples.FloatValues, Is.Null);
                Assert.That(samples.IntValues, Is.Null);
                Assert.That(samples.DoubleValues, Is.Null);
            }
        }

        [Test]
        public void Constructor_WithFloatType_InitializesFloatArray()
        {
            var samples = new DisposableGriddedSamples(5, 4, 0.0, 0.0, 1.0, InterpolationType.Float);

            Assert.That(samples.FloatValues, Is.Not.Null);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.FloatValues, Has.Length.EqualTo(20));
                Assert.That(samples.ShortValues, Is.Null);
                Assert.That(samples.IntValues, Is.Null);
                Assert.That(samples.DoubleValues, Is.Null);
            }
        }

        [Test]
        public void Constructor_WithIntType_InitializesIntArray()
        {
            var samples = new DisposableGriddedSamples(5, 4, 0.0, 0.0, 1.0, InterpolationType.Int);

            Assert.That(samples.IntValues, Is.Not.Null);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.IntValues, Has.Length.EqualTo(20));
                Assert.That(samples.ShortValues, Is.Null);
                Assert.That(samples.FloatValues, Is.Null);
                Assert.That(samples.DoubleValues, Is.Null);
            }
        }

        [Test]
        public void Constructor_WithDoubleType_InitializesDoubleArray()
        {
            var samples = new DisposableGriddedSamples(5, 4, 0.0, 0.0, 1.0, InterpolationType.Double);

            Assert.That(samples.DoubleValues, Is.Not.Null);
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.DoubleValues, Has.Length.EqualTo(20));
                Assert.That(samples.ShortValues, Is.Null);
                Assert.That(samples.FloatValues, Is.Null);
                Assert.That(samples.IntValues, Is.Null);
            }
        }

        [Test]
        public void Constructor_WithInvalidType_ThrowsArgumentException()
        {
            Assert.That(() => new DisposableGriddedSamples(5, 4, 0.0, 0.0, 1.0, (InterpolationType)999),
                        Throws.TypeOf<ArgumentException>()
                              .With.Message.Contains("Unsupported interpolation type"));
        }

        [Test]
        public void Constructor_InitializesCoordinateArrays()
        {
            var samples = new DisposableGriddedSamples(3, 5, 0.0, 0.0, 1.0, InterpolationType.Float);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.CoordinatesX, Is.Not.Null);
                Assert.That(samples.CoordinatesY, Is.Not.Null);
            }

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.CoordinatesX, Has.Length.EqualTo(3));
                Assert.That(samples.CoordinatesY, Has.Length.EqualTo(5));
            }
        }

        [Test]
        public void NumX_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.NumX = 15;

            Assert.That(samples.NumX, Is.EqualTo(15));
        }

        [Test]
        public void NumY_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.NumY = 25;

            Assert.That(samples.NumY, Is.EqualTo(25));
        }

        [Test]
        public void OriginX_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.OriginX = 5.5;

            Assert.That(samples.OriginX, Is.EqualTo(5.5));
        }

        [Test]
        public void OriginY_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.OriginY = 7.3;

            Assert.That(samples.OriginY, Is.EqualTo(7.3));
        }

        [Test]
        public void CellSize_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.CellSize = 2.5;

            Assert.That(samples.CellSize, Is.EqualTo(2.5));
        }

        [Test]
        public void ValueType_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            samples.ValueType = InterpolationType.Double;

            Assert.That(samples.ValueType, Is.EqualTo(InterpolationType.Double));
        }

        [Test]
        public void CoordinatesX_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);
            var newCoords = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };

            samples.CoordinatesX = newCoords;

            Assert.That(samples.CoordinatesX, Is.EqualTo(newCoords));
        }

        [Test]
        public void CoordinatesY_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(10, 10, 0.0, 0.0, 1.0, InterpolationType.Float);
            var newCoords = new[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0 };

            samples.CoordinatesY = newCoords;

            Assert.That(samples.CoordinatesY, Is.EqualTo(newCoords));
        }

        [Test]
        public void ShortValues_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(2, 3, 0.0, 0.0, 1.0, InterpolationType.Short);
            var values = new short[] { 1, 2, 3, 4, 5, 6 };

            samples.ShortValues = values;

            Assert.That(samples.ShortValues, Is.EqualTo(values));
        }

        [Test]
        public void FloatValues_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(2, 3, 0.0, 0.0, 1.0, InterpolationType.Float);
            var values = new[] { 1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f };

            samples.FloatValues = values;

            Assert.That(samples.FloatValues, Is.EqualTo(values));
        }

        [Test]
        public void IntValues_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(2, 3, 0.0, 0.0, 1.0, InterpolationType.Int);
            var values = new[] { 10, 20, 30, 40, 50, 60 };

            samples.IntValues = values;

            Assert.That(samples.IntValues, Is.EqualTo(values));
        }

        [Test]
        public void DoubleValues_SetAndGet_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(2, 3, 0.0, 0.0, 1.0, InterpolationType.Double);
            var values = new[] { 1.1, 2.2, 3.3, 4.4, 5.5, 6.6 };

            samples.DoubleValues = values;

            Assert.That(samples.DoubleValues, Is.EqualTo(values));
        }

        [Test]
        public void Constructor_WithZeroNumX_CreatesZeroLengthArrays()
        {
            var samples = new DisposableGriddedSamples(0, 10, 0.0, 0.0, 1.0, InterpolationType.Float);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.FloatValues, Is.Empty);
                Assert.That(samples.CoordinatesX, Is.Empty);
            }
        }

        [Test]
        public void Constructor_WithZeroNumY_CreatesZeroLengthArrays()
        {
            var samples = new DisposableGriddedSamples(10, 0, 0.0, 0.0, 1.0, InterpolationType.Float);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.FloatValues, Is.Empty);
                Assert.That(samples.CoordinatesY, Is.Empty);
            }
        }

        [Test]
        public void Constructor_WithNegativeOrigins_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(5, 5, -10.5, -20.3, 1.0, InterpolationType.Float);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(samples.OriginX, Is.EqualTo(-10.5));
                Assert.That(samples.OriginY, Is.EqualTo(-20.3));
            }
        }

        [Test]
        public void Constructor_WithNegativeCellSize_WorksCorrectly()
        {
            var samples = new DisposableGriddedSamples(5, 5, 0.0, 0.0, -1.5, InterpolationType.Float);

            Assert.That(samples.CellSize, Is.EqualTo(-1.5));
        }
    }
}