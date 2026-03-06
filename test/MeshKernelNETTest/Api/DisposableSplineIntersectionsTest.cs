using System;
using MeshKernelNET.Api;
using MeshKernelNET.Native;
using NUnit.Framework;

namespace MeshKernelNETTest.Api
{
    [TestFixture]
    public class DisposableSplineIntersectionsTests
    {
        [Test]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            var intersections = new DisposableSplineIntersections(5);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(intersections.NumIntersections, Is.EqualTo(5));
                Assert.That(intersections.SplineIndex, Is.Not.Null);
                Assert.That(intersections.IntersectionAngle, Is.Not.Null);
                Assert.That(intersections.IntersectionX, Is.Not.Null);
                Assert.That(intersections.IntersectionY, Is.Not.Null);
            }
        }

        [Test]
        public void Constructor_InitializesArraysWithCorrectLength()
        {
            var intersections = new DisposableSplineIntersections(10);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(intersections.SplineIndex, Has.Length.EqualTo(10));
                Assert.That(intersections.IntersectionAngle, Has.Length.EqualTo(10));
                Assert.That(intersections.IntersectionX, Has.Length.EqualTo(10));
                Assert.That(intersections.IntersectionY, Has.Length.EqualTo(10));
            }
        }

        [Test]
        public void NumIntersections_SetAndGet_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(5);

            intersections.NumIntersections = 10;

            Assert.That(intersections.NumIntersections, Is.EqualTo(10));
        }

        [Test]
        public void SplineIndex_SetAndGet_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(3);
            var indices = new[] { 0, 2, 5 };

            intersections.SplineIndex = indices;

            Assert.That(intersections.SplineIndex, Is.EqualTo(indices));
        }

        [Test]
        public void IntersectionAngle_SetAndGet_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(3);
            var angles = new[] { 0.5, 1.2, 2.8 };

            intersections.IntersectionAngle = angles;

            Assert.That(intersections.IntersectionAngle, Is.EqualTo(angles));
        }

        [Test]
        public void IntersectionX_SetAndGet_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(3);
            var xCoords = new[] { 10.5, 20.3, 30.7 };

            intersections.IntersectionX = xCoords;

            Assert.That(intersections.IntersectionX, Is.EqualTo(xCoords));
        }

        [Test]
        public void IntersectionY_SetAndGet_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(3);
            var yCoords = new[] { 15.2, 25.6, 35.9 };

            intersections.IntersectionY = yCoords;

            Assert.That(intersections.IntersectionY, Is.EqualTo(yCoords));
        }

        [Test]
        public void Constructor_WithZeroIntersections_CreatesEmptyArrays()
        {
            var intersections = new DisposableSplineIntersections(0);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(intersections.SplineIndex, Is.Empty);
                Assert.That(intersections.IntersectionAngle, Is.Empty);
                Assert.That(intersections.IntersectionX, Is.Empty);
                Assert.That(intersections.IntersectionY, Is.Empty);
            }
        }

        [Test]
        public void Constructor_WithNegativeAngles_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(2)
            {
                IntersectionAngle = new[] { -1.5, -2.3 }
            };

            Assert.That(intersections.IntersectionAngle, Is.EqualTo(new[] { -1.5, -2.3 }));
        }

        [Test]
        public void Constructor_WithNegativeCoordinates_WorksCorrectly()
        {
            var intersections = new DisposableSplineIntersections(2)
            {
                IntersectionX = new[] { -10.5, -20.3 },
                IntersectionY = new[] { -15.2, -25.6 }
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(intersections.IntersectionX, Is.EqualTo(new[] { -10.5, -20.3 }));
                Assert.That(intersections.IntersectionY, Is.EqualTo(new[] { -15.2, -25.6 }));
            }
        }

        [Test]
        public void AllProperties_CanBeSetAndRetrieved()
        {
            var intersections = new DisposableSplineIntersections(2)
            {
                NumIntersections = 2,
                SplineIndex = new[] { 1, 3 },
                IntersectionAngle = new[] { 1.57, 3.14 },
                IntersectionX = new[] { 100.0, 200.0 },
                IntersectionY = new[] { 150.0, 250.0 }
            };

            using (Assert.EnterMultipleScope())
            {
                Assert.That(intersections.NumIntersections, Is.EqualTo(2));
                Assert.That(intersections.SplineIndex, Is.EqualTo(new[] { 1, 3 }));
                Assert.That(intersections.IntersectionAngle, Is.EqualTo(new[] { 1.57, 3.14 }));
                Assert.That(intersections.IntersectionX, Is.EqualTo(new[] { 100.0, 200.0 }));
                Assert.That(intersections.IntersectionY, Is.EqualTo(new[] { 150.0, 250.0 }));
            }
        }

        [Test]
        public void CreateNativeObject_CreatesValidNativeStructure()
        {
            var intersections = new DisposableSplineIntersections(3)
            {
                NumIntersections = 3,
                SplineIndex = new[] { 0, 1, 2 },
                IntersectionAngle = new[] { 0.5, 1.0, 1.5 },
                IntersectionX = new[] { 10.0, 20.0, 30.0 },
                IntersectionY = new[] { 15.0, 25.0, 35.0 }
            };

            using (intersections)
            {
                SplineIntersectionsNative nativeIntersections = intersections.CreateNativeObject();

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(nativeIntersections.NumIntersections, Is.EqualTo(3));
                    Assert.That(nativeIntersections.SplineIndex, Is.Not.EqualTo(IntPtr.Zero));
                    Assert.That(nativeIntersections.IntersectionAngle, Is.Not.EqualTo(IntPtr.Zero));
                    Assert.That(nativeIntersections.IntersectionX, Is.Not.EqualTo(IntPtr.Zero));
                    Assert.That(nativeIntersections.IntersectionY, Is.Not.EqualTo(IntPtr.Zero));
                }
            }
        }
    }
}