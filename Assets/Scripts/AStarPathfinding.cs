using System.Collections.Generic;
using System.Linq;
using GridSystem;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Pathfinding
{
    public static class AStarPathfinding
    {
        private static readonly int STRAIGHT_MOVE_COST = 10;
        private static readonly int DIAGONAL_MOVE_COST = 15;

        private static List<Node> _openList;
        private static List<Node> _closedList;

        public static List<Node> GetPath(Node startNode, Node finalNode, Grid grid)
        {
            // reset nodes costs
            ResetNodes(grid);

            _openList = new List<Node>() { startNode };
            _closedList = new List<Node>();

            startNode.GCost = 0;
            startNode.HCost = GetHeuristic(startNode.Position, finalNode.Position);

            while (_openList.Count > 0)
            {
                Node currentNode = _openList.OrderBy(node => node.FCost).FirstOrDefault();
                _closedList.Add(currentNode);
                _openList.Remove(currentNode);
                
                // stop condition
                if (currentNode == finalNode) break;

                foreach (var nextNode in currentNode.Neighbours)
                {
                    int actionCost = GetCost(currentNode, nextNode);
                    
                    if (_openList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                    {
                        nextNode.NodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);
                    }
                    else if (_closedList.Contains(nextNode) && (currentNode.GCost + actionCost) < nextNode.GCost)
                    {
                        nextNode.NodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);

                        _closedList.Remove(nextNode);
                        _openList.Add(nextNode);
                    }
                    else if (!_openList.Contains(nextNode) && !_closedList.Contains(nextNode))
                    {
                        nextNode.NodeParent = currentNode;
                        nextNode.GCost = currentNode.GCost + actionCost;
                        nextNode.HCost = GetHeuristic(nextNode.Position, finalNode.Position);
                        
                        _openList.Add(nextNode);
                    }
                }
            }

            List<Node> path = new List<Node>();
            if (finalNode.NodeParent != null) // path founded
            {
                Node nodeToAdd = finalNode;
                path.Add(nodeToAdd);
                while (nodeToAdd.NodeParent != null)
                {
                    nodeToAdd = nodeToAdd.NodeParent;
                    path.Add(nodeToAdd);
                }
            }

            path.Reverse();
            return path;
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
            if (a == b)
                return 0;

            if (a.GridX == b.GridX || a.GridY == b.GridY)
                return STRAIGHT_MOVE_COST;
            
            return DIAGONAL_MOVE_COST;
        }
    }
}