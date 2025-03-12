using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using MeshKernelNET.Api;
using MeshKernelNET.Helpers;
using NetTopologySuite.Geometries;
using NUnit.Framework;

// Added alias to still be able to use the classic assert functions
using Assert = NUnit.Framework.Legacy.ClassicAssert;

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
            Assert.AreEqual(parameters.OuterIterations, nativeParameters.OuterIterations);
            Assert.AreEqual(parameters.BoundaryIterations, nativeParameters.BoundaryIterations);
            Assert.AreEqual(parameters.InnerIterations, nativeParameters.InnerIterations);
            Assert.AreEqual(0.975, nativeParameters.OrthogonalizationToSmoothingFactor);
        }

        [Test]
        public void GivenMakeGridParameters_ToMakeGridParametersNative_ShouldCreateValidMakeGridParametersNative()
        {
            //Arrange
            var makeGridParameters = MakeGridParameters.CreateDefault();

            // Act
            var native = makeGridParameters.ToMakeGridParametersNative();

            // Assert
            Assert.AreEqual(native.NumberOfColumns, makeGridParameters.NumberOfColumns);
            Assert.AreEqual(native.NumberOfRows, makeGridParameters.NumberOfRows);
            Assert.AreEqual(native.GridAngle, makeGridParameters.GridAngle);
            Assert.AreEqual(native.OriginXCoordinate, makeGridParameters.OriginXCoordinate);
            Assert.AreEqual(native.OriginYCoordinate, makeGridParameters.OriginYCoordinate);
            Assert.AreEqual(native.XGridBlockSize, makeGridParameters.XGridBlockSize);
            Assert.AreEqual(native.YGridBlockSize, makeGridParameters.YGridBlockSize);
            Assert.AreEqual(native.UpperRightCornerXCoordinate, makeGridParameters.UpperRightCornerXCoordinate);
            Assert.AreEqual(native.UpperRightCornerYCoordinate, makeGridParameters.UpperRightCornerYCoordinate);
        }

        [Test]
        public void GivenCurvilinearParameters_ToCurvilinearParametersNative_ShouldCreateValidCurvilinearParametersNative()
        {
            //Arrange
            var parameters = CurvilinearParameters.CreateDefault();

            // Act
            var curvilinearParameters = parameters.ToCurvilinearParametersNative();

            // Assert
            Assert.AreEqual(parameters.MRefinement, curvilinearParameters.MRefinement);
            Assert.AreEqual(parameters.NRefinement, curvilinearParameters.NRefinement);
            Assert.AreEqual(parameters.SmoothingIterations, curvilinearParameters.SmoothingIterations);
            Assert.AreEqual(parameters.SmoothingParameter, curvilinearParameters.SmoothingParameter);
            Assert.AreEqual(parameters.AttractionParameter, curvilinearParameters.AttractionParameter);
        }

        [Test]
        public void GivenSplinesToCurvilinearParameters_ToSplinesToCurvilinearParametersNative_ShouldCreateValidSplinesToCurvilinearParametersNative()
        {
            //Arrange
            var parameters = SplinesToCurvilinearParameters.CreateDefault();

            // Act
            var splinesToCurvilinearParameters = parameters.ToSplinesToCurvilinearParametersNative();

            // Assert
            Assert.AreEqual(parameters.AspectRatio, splinesToCurvilinearParameters.AspectRatio);
            Assert.AreEqual(parameters.AspectRatioGrowFactor, splinesToCurvilinearParameters.AspectRatioGrowFactor);
            Assert.AreEqual(parameters.AverageWidth, splinesToCurvilinearParameters.AverageWidth);
            Assert.AreEqual(Convert.ToInt32(parameters.CurvatureAdaptedGridSpacing), splinesToCurvilinearParameters.CurvatureAdaptedGridSpacing);
            Assert.AreEqual(Convert.ToInt32(parameters.GrowGridOutside), splinesToCurvilinearParameters.GrowGridOutside);
            Assert.AreEqual(parameters.MaximumNumberOfGridCellsInTheUniformPart, splinesToCurvilinearParameters.MaximumNumberOfGridCellsInTheUniformPart);
            Assert.AreEqual(parameters.GridsOnTopOfEachOtherTolerance, splinesToCurvilinearParameters.GridsOnTopOfEachOtherTolerance);
            Assert.AreEqual(parameters.MinimumCosineOfCrossingAngles, splinesToCurvilinearParameters.MinimumCosineOfCrossingAngles);
            Assert.AreEqual(Convert.ToInt32(parameters.CheckFrontCollisions), splinesToCurvilinearParameters.CheckFrontCollisions);
            Assert.AreEqual(Convert.ToInt32(parameters.RemoveSkinnyTriangles), splinesToCurvilinearParameters.RemoveSkinnyTriangles);
        }

        [Test]
        public void GivenBoundingBox_ToBoundingBoxNative_ShouldCreateValidBoundingBoxNative()
        {
            //Arrange
            var boundingBox = BoundingBox.CreateDefault();

            // Act
            var boundingBoxNative = boundingBox.ToBoundingBoxNative();

            // Assert
            Assert.AreEqual(boundingBox.xLowerLeft, double.MinValue);
            Assert.AreEqual(boundingBox.yLowerLeft, double.MinValue);
            Assert.AreEqual(boundingBox.xUpperRight, double.MaxValue);
            Assert.AreEqual(boundingBox.yUpperRight, double.MaxValue);

            Assert.AreEqual(boundingBoxNative.xLowerLeft, double.MinValue);
            Assert.AreEqual(boundingBoxNative.yLowerLeft, double.MinValue);
            Assert.AreEqual(boundingBoxNative.xUpperRight, double.MaxValue);
            Assert.AreEqual(boundingBoxNative.yUpperRight, double.MaxValue);
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

            Assert.AreEqual(expected + 2, disposableGeometryList.NumberOfCoordinates);

            double[] expectedXCoordinates = new[] { 0, 10, 10, 0, 0, innerouterSeparator, 3, 7, 7, 3, 3, geometrySeparator, 5, 15, 15, 5, 5 };
            double[] expectedYCoordinates = new[] { 0, 0, 10, 10, 0, innerouterSeparator, 3, 3, 7, 7, 3, geometrySeparator, 5, 5, 15, 15, 5 };
            double[] expectedZCoordinates = new[] { 0, 0, 0, 0, 0, innerouterSeparator, 0, 0, 0, 0, 0, geometrySeparator, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expectedXCoordinates, disposableGeometryList.XCoordinates);
            Assert.AreEqual(expectedYCoordinates, disposableGeometryList.YCoordinates);
            Assert.AreEqual(expectedZCoordinates, disposableGeometryList.Values);
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

            Assert.AreEqual(expectedXCoordinates, disposableGeometryList.XCoordinates);
            Assert.AreEqual(expectedYCoordinates, disposableGeometryList.YCoordinates);
            Assert.AreEqual(expectedZCoordinates, disposableGeometryList.Values);
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
                Assert.AreEqual(expectedXCoordinates[i], coordinateArray[i].X);
                Assert.AreEqual(expectedYCoordinates[i], coordinateArray[i].Y);
            }
        }
    }
}