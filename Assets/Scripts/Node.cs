using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Node
    {
        /// <summary>
        /// Known cost from starting node
        /// </summary>
        public int GCost { get; set; }
        /// <summary>
        /// (Heuristic) distance from end node (Manhattan distance)
        /// </summary>
        public int HCost { get; set; }
        /// <summary>
        /// G cost + H cost
        /// </summary>
        public int FCost => GCost + HCost;

        /// <summary>
        /// X position in Grid
        /// </summary>
        public int GridX { get; private set; }
        /// <summary>
        /// Y position in Grid
        /// </summary>
        public int GridY { get; private set; }
        /// <summary>
        /// Whether this node represent a wall
        /// </summary>
        public bool IsWall { get; private set; }
        /// <summary>
        /// Node position in scene coordinates (left bottom corner of the node)
        /// </summary>
        public Vector3 Position { get; private set; }
        /// <summary>
        /// Size of the node
        /// </summary>
        public float Size { get; private set; }
        /// <summary>
        /// Node center position in scene coordinates
        /// </summary>
        public Vector3 CenterPosition => Position + new Vector3(Size / 2, Size / 2, 0f);
        /// <summary>
        /// Previous node on the path
        /// </summary>
        public Node NodeParent { get; set; }
        /// <summary>
        /// List of neighbours nodes
        /// </summary>
        public List<Node> Neighbours { get; set; }

        public Node(int gridX, int gridY, bool isWall, Vector3 position, float size)
        {
            GridX = gridX;
            GridY = gridY;
            IsWall = isWall;
            Position = position;
            Size = size;

            // avoid overflow for fCost
            GCost = int.MaxValue / 2;
            HCost = int.MaxValue / 2;
        }

        /// <summary>
        /// Reset costs and node parent
        /// </summary>
        public void Reset()
        {
            GCost = int.MaxValue / 2;
            HCost = int.MaxValue / 2;
            NodeParent = null;
        }

        public override string ToString()
        {
            return $"({GridX}, {GridY})";
        }
    }
}
