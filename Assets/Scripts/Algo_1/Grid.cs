using UnityEngine;
using System.Collections.Generic;
using System;


/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          Grid class
 * ------------------------------------------------
 */

namespace Algo_1
{
    public class Grid : MonoBehaviour
    {
        public bool displayGridGizmos;
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        private int width, height;
        private float nodeDiameter;
        private PathNode[,] nodes;

        private void Awake()
        {
            this.nodeDiameter = this.nodeRadius * 2;
            this.width = Mathf.RoundToInt(this.gridWorldSize.x / this.nodeDiameter);
            this.height = Mathf.RoundToInt(this.gridWorldSize.y / this.nodeDiameter);

            this.InstantiateNodes();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(this.transform.position, new Vector3(this.gridWorldSize.x, 1, this.gridWorldSize.y));

            if (this.nodes != null && this.displayGridGizmos)
            {
                foreach (PathNode node in this.nodes)
                {
                    Gizmos.color = (node.IsWalkable() ? Color.white : Color.red);
                    Gizmos.DrawCube(node.GetWorldPosition(), Vector3.one * (this.nodeDiameter - .1f));
                }
            }
        }

        /**
         * Instantiate the nodes
         */
        private void InstantiateNodes()
        {
            this.nodes = new PathNode[this.width, this.height];
            Vector3 worldBottomLeftPos =
                this.transform.position - Vector3.right * this.gridWorldSize.x / 2
                - Vector3.forward * this.gridWorldSize.y / 2;

            for (int x = 0; x < this.gridWorldSize.x; ++x)
            {
                for (int y = 0; y < this.gridWorldSize.y; ++y)
                {
                    Vector3 worldPosition =
                        worldBottomLeftPos + Vector3.right * (x * this.nodeDiameter + this.nodeRadius)
                        + Vector3.forward * (y * this.nodeDiameter + nodeRadius);
                    bool isWalkable = !(Physics.CheckSphere(worldPosition, this.nodeRadius, this.unwalkableMask));
                    this.nodes[x, y] = new PathNode(worldPosition, x, y, this, isWalkable);
                }
            }
        }

        /**
         * Return the node at the specified world position
         */
        public PathNode GetNode(Vector3 worldPosition)
        {
            this.GetXY(worldPosition, out int x, out int y);

            return this.nodes[x, y];
        }

        /**
         * Get the x and y from the given world position
         */
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            float percentX = Mathf.Clamp01((worldPosition.x + this.gridWorldSize.x / 2) / this.gridWorldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.z + this.gridWorldSize.y / 2) / this.gridWorldSize.y);

            x = Mathf.RoundToInt((this.gridWorldSize.x - 1) * percentX);
            y = Mathf.RoundToInt((this.gridWorldSize.y - 1) * percentY);
        }

        /**
         * Return the node at the specified indices
         */
        public PathNode GetNode(int x, int y)
        {
            return this.nodes[x, y];
        }

        /**
         * Get the neighbors nodes of the given node
         */
        public List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();

            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if (x == 0 && y == 0) continue;

                    int checkX = node.GetX() + x;
                    int checkY = node.GetY() + y;

                    if (checkX >= 0 && checkX < this.width
                        && checkY >= 0 && checkY < height)
                    {
                        neighbors.Add(this.nodes[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        /**
         * Return grid's width
         */
        public int GetWidth()
        {
            return this.width;
        }

        /**
         * Return grid's height
         */
        public int GetHeight()
        {
            return this.height;
        }
    }
}