using System.Collections.Generic;
using System.Linq;
using GridSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Pathfinding
{
    public static class IDAStarPathfinding
    {
        private static readonly int STRAIGHT_MOVE_COST = 10;
        private static readonly int DIAGONAL_MOVE_COST = 15;

        private static List<Node> path = new List<Node>();
        private static int threshold;

        public static List<Node> GetPath(Node startNode, Node finalNode, Grid grid)
        {
            ResetNodes(grid);

            threshold = GetHeuristic(startNode.Position, finalNode.Position);
            path = new List<Node>();
            int temp = 0;
            while (temp != int.MaxValue)
            {
                temp = Search(startNode, finalNode, 0);
                if (temp == -1)
                {
                    path.Reverse();
                    return path;
                }

                threshold = temp;

            }

            return new List<Node>();
        }

        private static int Search(Node currentNode, Node goalNode, int gCost)
        {
            int fCost = gCost + GetHeuristic(currentNode.Position, goalNode.Position);

            if (fCost > threshold)
                return fCost;

            if (currentNode == goalNode)
            {
                path.Add(currentNode);
                return -1;
            }

            currentNode.GCost = gCost;
            currentNode.HCost = GetHeuristic(currentNode.Position, goalNode.Position);

            int minCost = int.MaxValue;

            foreach (var nextNode in currentNode.Neighbours)
            {
                if (nextNode.IsWall || path.Contains(nextNode)) continue;

                int tentativeGCost = gCost + GetCost(currentNode, nextNode);
                int temp = Search(nextNode, goalNode, tentativeGCost);

                if (temp == -1)
                {
                    path.Add(currentNode);
                    return -1;
                }
                if (temp < minCost)
                    minCost = temp;
            }
            return minCost;
        }

        private static void ResetNodes(Grid grid)
        {
            foreach (var node in grid.Nodes)
            {
                node.Reset();
            }
        }

        private static int GetHeuristic(Vector3 startPosition, Vector3 finalPosition)
        {
            float xValue = Mathf.Abs(finalPosition.x - startPosition.x);
            float yValue = Mathf.Abs(finalPosition.y - startPosition.y);

            // Manhattan distance
            return Mathf.FloorToInt(xValue + yValue);
        }

        private static int GetCost(Node a, Node b)
        {
            if (a.GridX == b.GridX || a.GridY == b.GridY)
                return STRAIGHT_MOVE_COST;
            
            return DIAGONAL_MOVE_COST;
        }
    }
}