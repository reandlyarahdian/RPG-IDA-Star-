using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class Grid
    {
        /// <summary>
        /// Number of nodes in the x axis
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Number of nodes in the y axis
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Size of each node
        /// </summary>
        public float NodeSize { get; private set; }
        /// <summary>
        /// Matrix of nodes that represent the grid
        /// </summary>
        public Node[,] Nodes { get; private set; }
        /// <summary>
        /// Scene position of the left bottom corner of the grid
        /// </summary>
        public Vector3 OriginPosition { get; private set; }
        /// <summary>
        /// Layers that represent walls/obstacles
        /// </summary>
        public LayerMask WallLayers { get; private set; }

        public Grid(int width, int height, float nodeSize, Vector3 originPosition, LayerMask wallLayers)
        {
            Width = width;
            Height = height;
            NodeSize = nodeSize;
            OriginPosition = originPosition;
            WallLayers = wallLayers;

            Nodes = new Node[width, height];
            
            // Create nodes
            int wallsCount = 0; 
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    bool isWall = false;
                    Vector3 center = GetNodePosition(i, j) + new Vector3(this.NodeSize / 2, this.NodeSize / 2, 0);

                    if (CheckWall(center, wallLayers))
                    {
                        isWall = true;
                        wallsCount++;
                    }

                    Nodes[i, j] = new Node(i, j, isWall, GetNodePosition(i, j), nodeSize);
                }
            }

            if (wallsCount <= 0)
            {
                Debug.LogWarning("[GRID] No walls detected. Make sure the 'Geometry Type' is 'Polygons'");
            }

            // Set neighbours
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Nodes[i, j].Neighbours = GetNeighbours(Nodes[i, j]);
                }
            }
        }

        /// <summary>
        /// Draw the grid in scene view using Debug.DrawLine
        /// </summary>
        public void DrawGrid()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Node node = GetNode(i, j);
                    
                    // node center cross
                    if (!node.IsWall)
                    {
                        Debug.DrawLine(
                            new Vector3(node.CenterPosition.x - 0.1f, node.CenterPosition.y),
                            new Vector3(node.CenterPosition.x + 0.1f, node.CenterPosition.y),
                            Color.red,
                            Mathf.Infinity);
                        Debug.DrawLine(
                            new Vector3(node.CenterPosition.x, node.CenterPosition.y - 0.1f),
                            new Vector3(node.CenterPosition.x, node.CenterPosition.y + 0.1f),
                            Color.red,
                            Mathf.Infinity);
                    }
                    
                    // grid lines
                    Debug.DrawLine(GetNodePosition(i, j), GetNodePosition(i, j + 1), Color.white, Mathf.Infinity);
                    Debug.DrawLine(GetNodePosition(i, j), GetNodePosition(i + 1, j), Color.white, Mathf.Infinity);
                }
            }

            // up and right grid lines
            Debug.DrawLine(OriginPosition + new Vector3(0, Height, 0), OriginPosition + new Vector3(Width, Height, 0),
                Color.white, Mathf.Infinity);
            Debug.DrawLine(OriginPosition + new Vector3(Width, 0, 0), OriginPosition + new Vector3(Width, Height, 0),
                Color.white, Mathf.Infinity);

        }

        /// <summary>
        /// Gets the world position of the node
        /// </summary>
        /// <param name="node">Node from the grid</param>
        /// <returns></returns>
        public Vector3 GetNodePosition(Node node)
        {
            return GetNodePosition(node.GridX, node.GridY);
        }

        /// <summary>
        /// Gets the world position of the node
        /// </summary>
        /// <param name="x">x position in the grid</param>
        /// <param name="y">y position in the grid</param>
        /// <returns></returns>
        public Vector3 GetNodePosition(int x, int y)
        {
            return new Vector3(x * NodeSize, y * NodeSize, 0) + OriginPosition;
        }

        /// <summary>
        /// Gets node from grid
        /// </summary>
        /// <param name="x">x position in the grid</param>
        /// <param name="y">y position in the grid</param>
        /// <returns></returns>
        public Node GetNode(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < Width && y < Height)
                return Nodes[x, y];
            else
                return null;
        }

        /// <summary>
        /// Gets node from grid
        /// </summary>
        /// <param name="position">Scene world position</param>
        /// <returns></returns>
        public Node GetNode(Vector3 position)
        {
            int x = Mathf.FloorToInt((position.x - OriginPosition.x) / NodeSize);
            int y = Mathf.FloorToInt((position.y - OriginPosition.y) / NodeSize);

            return GetNode(x, y);
        }

        private List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            int nodeX = node.GridX;
            int nodeY = node.GridY;

            for (int i = nodeX - 1; i < nodeX + 2; i++)
            {
                for (int j = nodeY - 1; j < nodeY + 2; j++)
                {
                    if (!(i == nodeX && j == nodeY))
                    {
                        Node n = GetNode(i, j);
                        if (n != null)
                        {
                            if (!n.IsWall && CheckDiagonals(node, n))
                                neighbours.Add(n);
                        }

                    }
                }
            }

            return neighbours;
        }

        private bool CheckDiagonals(Node origin, Node nextNode)
        {
            bool diagonalNotBlocked = true;

            // first quadrant
            if (nextNode.GridX > origin.GridX && nextNode.GridY > origin.GridY)
            {
                if (GetNode(nextNode.GridX - 1, nextNode.GridY).IsWall &&
                    GetNode(nextNode.GridX, nextNode.GridY - 1).IsWall)
                    diagonalNotBlocked = false;
            }
            // second quadrant
            else if (nextNode.GridX < origin.GridX && nextNode.GridY > origin.GridY)
            {
                if (GetNode(nextNode.GridX + 1, nextNode.GridY).IsWall &&
                    GetNode(nextNode.GridX, nextNode.GridY - 1).IsWall)
                    diagonalNotBlocked = false;
            }
            // third quadrant
            else if (nextNode.GridX < origin.GridX && nextNode.GridY < origin.GridY)
            {
                if (GetNode(nextNode.GridX + 1, nextNode.GridY).IsWall &&
                    GetNode(nextNode.GridX, nextNode.GridY + 1).IsWall)
                    diagonalNotBlocked = false;
            }
            // fourth quadrant
            else if (nextNode.GridX > origin.GridX && nextNode.GridY < origin.GridY)
            {
                if (GetNode(nextNode.GridX - 1, nextNode.GridY).IsWall &&
                    GetNode(nextNode.GridX, nextNode.GridY + 1).IsWall)
                    diagonalNotBlocked = false;
            }


            return diagonalNotBlocked;
        }

        private bool CheckWall(Vector3 position, LayerMask wallLayer)
        {
            return Physics2D.OverlapPoint(position, wallLayer);
        }

    }
}
