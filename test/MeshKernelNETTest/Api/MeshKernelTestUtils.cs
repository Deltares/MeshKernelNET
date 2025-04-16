using System;
using System.Diagnostics;
using MeshKernelNET.Api;

namespace MeshKernelNETTest.Api
{
    public static class TestUtilityFunctions
    {
        public static DisposableMesh2D CreateMesh2D(
            int numbOfNodesHorizontal,
            int numbOfNodesVertical,
            double cellWidth,
            double cellHeight,
            double xOffset = 0.0,
            double yOffset = 0.0)
        {
            var result = new DisposableMesh2D();

            var indicesValues = new int[numbOfNodesVertical, numbOfNodesHorizontal];
            result.NodeX = new double[numbOfNodesHorizontal * numbOfNodesVertical];
            result.NodeY = new double[numbOfNodesHorizontal * numbOfNodesVertical];
            result.NumNodes = numbOfNodesHorizontal * numbOfNodesVertical;

            var nodeIndex = 0;
            for (var v = 0; v < numbOfNodesVertical; ++v)
            {
                for (var h = 0; h < numbOfNodesHorizontal; ++h)
                {
                    indicesValues[v, h] = (v * numbOfNodesHorizontal) + h;
                    result.NodeX[nodeIndex] = xOffset + (h * cellWidth);
                    result.NodeY[nodeIndex] = yOffset + (v * cellHeight);
                    nodeIndex++;
                }
            }

            result.NumEdges = ((numbOfNodesHorizontal - 1) * numbOfNodesVertical) +
                              (numbOfNodesHorizontal * (numbOfNodesVertical - 1));
            result.EdgeNodes = new int[result.NumEdges * 2];
            var edgeIndex = 0;
            for (var v = 0; v < numbOfNodesVertical - 1; ++v)
            {
                for (var h = 0; h < numbOfNodesHorizontal; ++h)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[v,h];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[v + 1, h];
                    edgeIndex++;
                }
            }

            for (var v = 0; v < numbOfNodesVertical; ++v)
            {
                for (var h = 0; h < numbOfNodesHorizontal - 1; ++h)
                {
                    result.EdgeNodes[edgeIndex] = indicesValues[v, h];
                    edgeIndex++;
                    result.EdgeNodes[edgeIndex] = indicesValues[v, h + 1];
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
            for (var v = 0; v < result.NumN; ++v)
            {
                for (var h = 0; h < result.NumM; ++h)
                {
                    result.NodeX[nodeIndex] = h * cellWidth;
                    result.NodeY[nodeIndex] = v * cellHeight;
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