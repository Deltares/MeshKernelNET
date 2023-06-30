using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using MeshKernelNETCore.Api;
using MeshKernelNETCore.Helpers;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace MeshKernelNETCoreTest.Native
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
            Assert.AreEqual(parameters.CurvatureAdaptedGridSpacing, splinesToCurvilinearParameters.CurvatureAdaptedGridSpacing);
            Assert.AreEqual(parameters.GrowGridOutside, splinesToCurvilinearParameters.GrowGridOutside);
            Assert.AreEqual(parameters.MaximumNumberOfGridCellsInTheUniformPart, splinesToCurvilinearParameters.MaximumNumberOfGridCellsInTheUniformPart);
            Assert.AreEqual(parameters.GridsOnTopOfEachOtherTolerance, splinesToCurvilinearParameters.GridsOnTopOfEachOtherTolerance);
            Assert.AreEqual(parameters.MinimumCosineOfCrossingAngles, splinesToCurvilinearParameters.MinimumCosineOfCrossingAngles);
            Assert.AreEqual(parameters.CheckFrontCollisions, splinesToCurvilinearParameters.CheckFrontCollisions);
            Assert.AreEqual(parameters.RemoveSkinnyTriangles, splinesToCurvilinearParameters.RemoveSkinnyTriangles);

        }

        [Test]
        public void GivenAMultiPolygon_ToDisposableGeometryList_ShouldConvertToValidDisposableGeometryList()
        {
            //Arrange
            var polygonWithHole = new Polygon(new LinearRing(new[]
            {
                new Coordinate(0,0),
                new Coordinate(10,0),
                new Coordinate(10,10),
                new Coordinate(0,10),
                new Coordinate(0,0),
            }), new ILinearRing[]
            {
                // add interior ring (hole)
                new LinearRing(new []
                {
                    new Coordinate(3,3),
                    new Coordinate(7,3),
                    new Coordinate(7,7),
                    new Coordinate(3,7),
                    new Coordinate(3,3),
                }),
            });
            var polygon2 = new Polygon(new LinearRing(new[]
            {
                new Coordinate(5,5),
                new Coordinate(15,5),
                new Coordinate(15,15),
                new Coordinate(5,15),
                new Coordinate(5,5),
            }));

            var multiPolygon = new List<Polygon>(new[]
            {
                polygonWithHole,
                polygon2
            });

            // Act
            var geometrySeparator = -999.0;
            var innerouterSeparator = -998.0;
            var disposableGeometryList = multiPolygon.ToDisposableGeometryList(geometrySeparator, -998.0);

            // Assert
            var expected = polygonWithHole.ExteriorRing.Coordinates.Length +
                           polygonWithHole.InteriorRings[0].Coordinates.Length +
                           polygon2.Coordinates.Length;

            Assert.AreEqual(expected + 2, disposableGeometryList.NumberOfCoordinates);

            var expectedXCoordinates = new[] { 0, 10, 10, 0, 0, innerouterSeparator, 3, 7, 7, 3, 3, geometrySeparator, 5, 15, 15, 5, 5 };
            var expectedYCoordinates = new[] { 0, 0, 10, 10, 0, innerouterSeparator, 3, 3, 7, 7, 3, geometrySeparator, 5, 5, 15, 15, 5 };
            var expectedZCoordinates = new[] { 0, 0, 0, 0, 0, innerouterSeparator, 0, 0, 0, 0, 0, geometrySeparator, 0, 0, 0, 0, 0 };

            Assert.AreEqual(expectedXCoordinates, disposableGeometryList.XCoordinates);
            Assert.AreEqual(expectedYCoordinates, disposableGeometryList.YCoordinates);
            Assert.AreEqual(expectedZCoordinates, disposableGeometryList.Values);
        }

        [Test]
        public void GivenACoordinateArray_ToDisposableGeometryList_ShouldConvertToValidDisposableGeometryList()
        {
            //Arrange
            var coordinates = new[]
            {
                new Coordinate(0,0),
                new Coordinate(1,1),
                new Coordinate(2,2),
            };

            // Act
            var geometrySeparator = -999.0;
            var innerouterSeparator = -998.0;
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
            var coordinates = new[]
            {
                new Coordinate(0.0,0.0),
                new Coordinate(1.0,1.0),
                new Coordinate(2.0,2.0),
                new Coordinate(0.0,0.0),
                new Coordinate(-999.0,-999.0),
                new Coordinate(-998.0,-998.0)
            };
            var geometrySeparator = -999.0;
            var innerouterSeparator = -998.0;
            var disposableGeometryList = coordinates.ToDisposableGeometryList(geometrySeparator, innerouterSeparator);

            // Act
            var featureList = disposableGeometryList.ToPolygonList();

            // Assert
            var expectedXCoordinates = new[] { 0, 1, 2, 0 };
            var expectedYCoordinates = new[] { 0, 1, 2, 0 };

            var coordinateArray = featureList.ToArray()[0].Coordinates;

            for (int i = 0; i < coordinateArray.Length - 1; i++)
            {
                Assert.AreEqual(expectedXCoordinates[i], coordinateArray[i].X);
                Assert.AreEqual(expectedYCoordinates[i], coordinateArray[i].Y);
            }
        }
    }
}