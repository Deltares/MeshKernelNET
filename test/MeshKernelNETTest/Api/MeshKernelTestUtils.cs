using System;
using System.Diagnostics;
using MeshKernelNET.Api;

namespace MeshKernelNETTest.Api
{
    public static class TestUtilityFunctions
    {
        public static DisposableMesh2D CreateMesh2D(
            int numbOfCellsHorizontal,
            int numbOfCellsVertical,
            double cellWidth,
            double cellHeight,
            double xOffset = 0.0,
            double yOffset = 0.0)
        {
            var result = new DisposableMesh2D();

            var indicesValues = new int[numbOfCellsHorizontal, numbOfCellsVertical];
            result.NodeX = new double[numbOfCellsHorizontal * numbOfCellsVertical];
            result.NodeY = new double[numbOfCellsHorizontal * numbOfCellsVertical];
            result.NumNodes = numbOfCellsHorizontal * numbOfCellsVertical;

            var nodeIndex = 0;
            for (var i = 0; i < numbOfCellsHorizontal; ++i)
            {
                for (var j = 0; j < numbOfCellsVertical; ++j)
                {
                    indicesValues[i, j] = (i * numbOfCellsVertical) + j;
                    result.NodeX[nodeIndex] = xOffset + (i * cellWidth);
                    result.NodeY[nodeIndex] = yOffset + (j * cellHeight);
                    nodeIndex++;
                }
            }

            result.NumEdges = ((numbOfCellsHorizontal - 1) * numbOfCellsVertical) +
                              (numbOfCellsHorizontal * (numbOfCellsVertical - 1));
            result.EdgeNodes = new int[result.NumEdges * 2];
            var edgeIndex = 0;
            for (var i = 0; i < numbOfCellsHorizontal - 1; ++i)
            {
                for (var j = 0; j < numbOfCellsVertical; ++j)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[i + 1, j];
                    edgeIndex++;
                }
            }

            for (var i = 0; i < numbOfCellsHorizontal; ++i)
            {
                for (var j = 0; j < numbOfCellsVertical - 1; ++j)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j + 1];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[i, j];
                    edgeIndex++;
                }
            }

            return result;
        }

        public static DisposableCurvilinearGrid CreateCurvilinearGrid(
            int numbOfRows = 3,
            int numbOfColumns = 3,
            double cellWidth = 1.0,
            double cellHeight = 1.0)
        {
            var result = new DisposableCurvilinearGrid(numbOfRows, numbOfColumns);

            var nodeIndex = 0;
            for (var i = 0; i < result.NumM; ++i)
            {
                for (var j = 0; j < result.NumN; ++j)
                {
                    result.NodeX[nodeIndex] = i * cellWidth;
                    result.NodeY[nodeIndex] = j * cellHeight;
                    nodeIndex++;
                }
            }

            return result;
        }

        public static void GetTiming(Stopwatch stopwatch, string actionName, Action action)
        {
            stopwatch.Restart();

            action();

            stopwatch.Stop();
            Console.WriteLine($"{stopwatch.Elapsed} -- {actionName}");
        }
    }
}