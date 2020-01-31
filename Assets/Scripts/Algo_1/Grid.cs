using UnityEngine;
using System;
using JoaDev.Utils;

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
        public LayerMask unwalkableMask;
        public Vector2 gridWorldSize;
        public float nodeRadius;

        private int width, height;
        private float nodeDiameter;
        private PathNode[,] nodes;

        private void Start()
        {
            this.nodeDiameter = this.nodeRadius * 2;
            this.width = Mathf.RoundToInt(this.gridWorldSize.x / this.nodeDiameter);
            this.height = Mathf.RoundToInt(this.gridWorldSize.y / this.nodeDiameter);

            this.InstantiateNodes();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(this.transform.position, new Vector3(this.gridWorldSize.x, 1, this.gridWorldSize.y));

            if (this.nodes != null)
            {
                foreach (PathNode node in this.nodes)
                {
                    Gizmos.color = (node.IsWalkable() ? Color.white : Color.red);
                    Gizmos.DrawCube(node.GetWorldPosition(), Vector3.one * (this.nodeDiameter - .1f));
                }
            }
        }

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
                    this.nodes[x, y] = new PathNode(worldPosition, this, isWalkable);
                }
            }
        }

        public PathNode GetNodeFromWorldPosition(Vector3 worldPosition)
        {
            float percentX = Mathf.Clamp01((worldPosition.x + this.gridWorldSize.x / 2) / this.gridWorldSize.x);
            float percentY = Mathf.Clamp01((worldPosition.z + this.gridWorldSize.y / 2) / this.gridWorldSize.y);

            int x = Mathf.RoundToInt((this.gridWorldSize.x - 1) * percentX);
            int y = Mathf.RoundToInt((this.gridWorldSize.y - 1) * percentY);

            return this.nodes[x, y];
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

        /**
         * Return the node at the specified indices
         */
        public PathNode GetNode(int x, int y)
        {
            return this.nodes[x, y];
        }
    }
}