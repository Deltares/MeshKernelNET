using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using MeshKernelNET.Api;
using MeshKernelNET.Helpers;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace MeshKernelNETTest.Native
{
    [TestFixture]
    public class NativeStructConversionExtensionsTest
    {
        [Test]
        public void GivenOrthogonalizationParameters_ToOrthogonalizationParametersNative_ShouldCreateOrthogonalizationParametersNativeWithValidDefaults()
        {
            //Arrange
            var parameters = OrthogonalizationParameters.CreateDefault();

            // Act
            var nativeParameters = parameters.ToOrthogonalizationParametersNative();

            // Assert
            Assert.That(nativeParameters.OuterIterations, Is.EqualTo(parameters.OuterIterations));
            Assert.That(nativeParameters.BoundaryIterations, Is.EqualTo(parameters.BoundaryIterations));
            Assert.That(nativeParameters.InnerIterations, Is.EqualTo(parameters.InnerIterations));
            Assert.That(nativeParameters.OrthogonalizationToSmoothingFactor, Is.EqualTo(0.975));
        }

        [Test]
        public void GivenMakeGridParameters_ToMakeGridParametersNative_ShouldCreateValidMakeGridParametersNative()
        {
            //Arrange
            var makeGridParameters = MakeGridParameters.CreateDefault();

            // Act
            var native = makeGridParameters.ToMakeGridParametersNative();

            // Assert
            Assert.That(makeGridParameters.NumberOfColumns, Is.EqualTo(native.NumberOfColumns));
            Assert.That(makeGridParameters.NumberOfRows, Is.EqualTo(native.NumberOfRows));
            Assert.That(makeGridParameters.GridAngle, Is.EqualTo(native.GridAngle));
            Assert.That(makeGridParameters.OriginXCoordinate, Is.EqualTo(native.OriginXCoordinate));
            Assert.That(makeGridParameters.OriginYCoordinate, Is.EqualTo(native.OriginYCoordinate));
            Assert.That(makeGridParameters.XGridBlockSize, Is.EqualTo(native.XGridBlockSize));
            Assert.That(makeGridParameters.YGridBlockSize, Is.EqualTo(native.YGridBlockSize));
            Assert.That(makeGridParameters.UpperRightCornerXCoordinate, Is.EqualTo(native.UpperRightCornerXCoordinate));
            Assert.That(makeGridParameters.UpperRightCornerYCoordinate, Is.EqualTo(native.UpperRightCornerYCoordinate));
        }

        [Test]
        public void GivenCurvilinearParameters_ToCurvilinearParametersNative_ShouldCreateValidCurvilinearParametersNative()
        {
            //Arrange
            var parameters = CurvilinearParameters.CreateDefault();

            // Act
            var curvilinearParameters = parameters.ToCurvilinearParametersNative();

            // Assert
            Assert.That(curvilinearParameters.MRefinement, Is.EqualTo(parameters.MRefinement));
            Assert.That(curvilinearParameters.NRefinement, Is.EqualTo(parameters.NRefinement));
            Assert.That(curvilinearParameters.SmoothingIterations, Is.EqualTo(parameters.SmoothingIterations));
            Assert.That(curvilinearParameters.SmoothingParameter, Is.EqualTo(parameters.SmoothingParameter));
            Assert.That(curvilinearParameters.AttractionParameter, Is.EqualTo(parameters.AttractionParameter));
        }

        [Test]
        public void GivenSplinesToCurvilinearParameters_ToSplinesToCurvilinearParametersNative_ShouldCreateValidSplinesToCurvilinearParametersNative()
        {
            //Arrange
            var parameters = SplinesToCurvilinearParameters.CreateDefault();

            // Act
            var splinesToCurvilinearParameters = parameters.ToSplinesToCurvilinearParametersNative();

            // Assert
            Assert.That(splinesToCurvilinearParameters.AspectRatio, Is.EqualTo(parameters.AspectRatio));
            Assert.That(splinesToCurvilinearParameters.AspectRatioGrowFactor, Is.EqualTo(parameters.AspectRatioGrowFactor));
            Assert.That(splinesToCurvilinearParameters.AverageWidth, Is.EqualTo(parameters.AverageWidth));
            Assert.That(splinesToCurvilinearParameters.CurvatureAdaptedGridSpacing, Is.EqualTo(Convert.ToInt32(parameters.CurvatureAdaptedGridSpacing)));
            Assert.That(splinesToCurvilinearParameters.GrowGridOutside, Is.EqualTo(Convert.ToInt32(parameters.GrowGridOutside)));
            Assert.That(splinesToCurvilinearParameters.MaximumNumberOfGridCellsInTheUniformPart, Is.EqualTo(parameters.MaximumNumberOfGridCellsInTheUniformPart));
            Assert.That(splinesToCurvilinearParameters.GridsOnTopOfEachOtherTolerance, Is.EqualTo(parameters.GridsOnTopOfEachOtherTolerance));
            Assert.That(splinesToCurvilinearParameters.MinimumCosineOfCrossingAngles, Is.EqualTo(parameters.MinimumCosineOfCrossingAngles));
            Assert.That(splinesToCurvilinearParameters.CheckFrontCollisions, Is.EqualTo(Convert.ToInt32(parameters.CheckFrontCollisions)));
            Assert.That(splinesToCurvilinearParameters.RemoveSkinnyTriangles, Is.EqualTo(Convert.ToInt32(parameters.RemoveSkinnyTriangles)));
        }

        [Test]
        public void GivenBoundingBox_ToBoundingBoxNative_ShouldCreateValidBoundingBoxNative()
        {
            //Arrange
            var boundingBox = BoundingBox.CreateDefault();

            // Act
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();

            // Assert
            Assert.That(boundingBox.xLowerLeft, Is.EqualTo(double.MinValue));
            Assert.That(boundingBox.yLowerLeft, Is.EqualTo(double.MinValue));
            Assert.That(boundingBox.xUpperRight, Is.EqualTo(double.MaxValue));
            Assert.That(boundingBox.yUpperRight, Is.EqualTo(double.MaxValue));

            Assert.That(boundingBoxNative.xLowerLeft, Is.EqualTo(double.MinValue));
            Assert.That(boundingBoxNative.yLowerLeft, Is.EqualTo(double.MinValue));
            Assert.That(boundingBoxNative.xUpperRight, Is.EqualTo(double.MaxValue));
            Assert.That(boundingBoxNative.yUpperRight, Is.EqualTo(double.MaxValue));
        }

        [Test]
        public void GivenAMultiPolygon_ToDisposableGeometryList_ShouldConvertToValidDisposableGeometryList()
        {
            //Arrange
            var polygonWithHole = new Polygon(new LinearRing(new[] { new Coordinate(0, 0), new Coordinate(10, 0), new Coordinate(10, 10), new Coordinate(0, 10), new Coordinate(0, 0), }), new ILinearRing[]
            {
                // add interior ring (hole)
                new LinearRing(new[] { new Coordinate(3, 3), new Coordinate(7, 3), new Coordinate(7, 7), new Coordinate(3, 7), new Coordinate(3, 3), }),
            });
            var polygon2 = new Polygon(new LinearRing(new[] { new Coordinate(5, 5), new Coordinate(15, 5), new Coordinate(15, 15), new Coordinate(5, 15), new Coordinate(5, 5), }));

            var multiPolygon = new List<Polygon>(new[] { polygonWithHole, polygon2 });

            // Act
            double geometrySeparator = -999.0;
            double innerouterSeparator = -998.0;
            var disposableGeometryList = multiPolygon.ToDisposableGeometryList(geometrySeparator, -998.0);

            // Assert
            int expected = polygonWithHole.ExteriorRing.Coordinates.Length +
                           polygonWithHole.InteriorRings[0].Coordinates.Length +
                           polygon2.Coordinates.Length;

            Assert.That(disposableGeometryList.NumberOfCoordinates, Is.EqualTo(expected + 2));

            double[] expectedXCoordinates = new[] { 0, 10, 10, 0, 0, innerouterSeparator, 3, 7, 7, 3, 3, geometrySeparator, 5, 15, 15, 5, 5 };
            double[] expectedYCoordinates = new[] { 0, 0, 10, 10, 0, innerouterSeparator, 3, 3, 7, 7, 3, geometrySeparator, 5, 5, 15, 15, 5 };
            double[] expectedZCoordinates = new[] { 0, 0, 0, 0, 0, innerouterSeparator, 0, 0, 0, 0, 0, geometrySeparator, 0, 0, 0, 0, 0 };

            Assert.That(disposableGeometryList.XCoordinates, Is.EqualTo(expectedXCoordinates));
            Assert.That(disposableGeometryList.YCoordinates, Is.EqualTo(expectedYCoordinates));
            Assert.That(disposableGeometryList.Values, Is.EqualTo(expectedZCoordinates));
        }

        [Test]
        public void GivenACoordinateArray_ToDisposableGeometryList_ShouldConvertToValidDisposableGeometryList()
        {
            //Arrange
            var coordinates = new[] { new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 2), };

            // Act
            double geometrySeparator = -999.0;
            double innerouterSeparator = -998.0;
            var disposableGeometryList = coordinates.ToDisposableGeometryList(geometrySeparator, innerouterSeparator);

            // Assert
            var expectedXCoordinates = new[] { 0, 1, 2 };
            var expectedYCoordinates = new[] { 0, 1, 2 };
            var expectedZCoordinates = new[] { 0, 0, 0 };

            Assert.That(disposableGeometryList.XCoordinates, Is.EqualTo(expectedXCoordinates));
            Assert.That(disposableGeometryList.YCoordinates, Is.EqualTo(expectedYCoordinates));
            Assert.That(disposableGeometryList.Values, Is.EqualTo(expectedZCoordinates));
        }

        [Test]
        public void GivenACollectionOfFeatures_ToFeatureList_ShouldConvertToValidFeatureList()
        {
            //Arrange
            var coordinates = new[] { new Coordinate(0.0, 0.0), new Coordinate(1.0, 1.0), new Coordinate(2.0, 2.0), new Coordinate(0.0, 0.0), new Coordinate(-999.0, -999.0), new Coordinate(-998.0, -998.0) };
            double geometrySeparator = -999.0;
            double innerouterSeparator = -998.0;
            var disposableGeometryList = coordinates.ToDisposableGeometryList(geometrySeparator, innerouterSeparator);

            // Act
            ICollection<IPolygon> featureList = disposableGeometryList.ToPolygonList();

            // Assert
            var expectedXCoordinates = new[] { 0, 1, 2, 0 };
            var expectedYCoordinates = new[] { 0, 1, 2, 0 };

            Coordinate[] coordinateArray = featureList.ToArray()[0].Coordinates;

            for (var i = 0; i < coordinateArray.Length - 1; i++)
            {
                Assert.That(coordinateArray[i].X, Is.EqualTo(expectedXCoordinates[i]));
                Assert.That(coordinateArray[i].Y, Is.EqualTo(expectedYCoordinates[i]));
            }
        }
    }
}