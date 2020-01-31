using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          Pathfinding class
 * ------------------------------------------------
 */

namespace Algo_1
{
    public class Pathfinding
    {
        private Grid grid;

        private static float CELL_SIZE = 10f;
        private static float BORDER_SIZE = 1f;
        private static Vector3 ORIGIN_POSITION = Vector3.zero;

        private const int STRAIGHT_COST = 10; // 1 * 10
        private const int DIAGONAL_COST = 14; // 1.4 * 10

        /**
         * Create a new pathfinding
         */
        public Pathfinding(int width, int height)
        {
            // this.grid = new Grid(width, height, CELL_SIZE, BORDER_SIZE, ORIGIN_POSITION, (int x, int y, Grid grid) => new PathNode(x, y, grid));
        }

        /**
         * Return the associate grid
         */
        public Grid GetGrid()
        {
            return this.grid;
        }

        /**
         * Try to find a path to the end node
         */
        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            PathNode startNode = this.grid.GetNode(startX, startY);
            PathNode endNode = this.grid.GetNode(endX, endY);

            this.InitializeNodes();
            this.InitializeStartNode(startNode, endNode);

            return this.SearchPath(startNode, endNode);
            // return this.SearchPathBinaryHeaps(startNode, endNode);
        }

        /**
         * Initialize all nodes
         */
        private void InitializeNodes()
        {
            for (int x = 0; x < this.grid.GetWidth(); ++x)
            {
                for (int y = 0; y < this.grid.GetHeight(); ++y)
                {
                    PathNode node = this.grid.GetNode(x, y);
                    node.SetG(int.MaxValue);
                    node.ComputeF();
                    node.SetParent(null);
                }
            }
        }

        /**
         * Initialize start node
         */
        private void InitializeStartNode(PathNode startNode, PathNode endNode)
        {
            startNode.SetG(0);
            startNode.SetH(this.ComputeHDistance(startNode, endNode));
            startNode.ComputeF();
        }

        /**
         * Classic A* algorithm to search a path
         */
        private List<PathNode> SearchPath(PathNode startNode, PathNode endNode)
        {
            List<PathNode> openList = new List<PathNode> { startNode }; // Nodes queued up for searching
            List<PathNode> closedList = new List<PathNode>(); // Nodes already searched

            while (openList.Count > 0)
            {
                PathNode currentNode = this.GetLowestFCostNode(openList);

                if (currentNode == endNode) return this.ComputePath(endNode);

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighborNode in this.GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighborNode)) continue;
                    if (!neighborNode.IsWalkable())
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GetG() + this.ComputeHDistance(currentNode, neighborNode);

                    if (tentativeGCost < neighborNode.GetG())
                    {
                        neighborNode.SetParent(currentNode);
                        neighborNode.SetG(tentativeGCost);
                        neighborNode.SetH(this.ComputeHDistance(neighborNode, endNode));
                        neighborNode.ComputeF();

                        if (!openList.Contains(neighborNode)) openList.Add(neighborNode);
                    }
                }
            }

            // Out of nodes on the open list, no path found
            return null;
        }

        /**
         * A* algorithm using binary heaps
         */
        private List<PathNode> SearchPathBinaryHeaps(PathNode startNode, PathNode endNode)
        {
            List<PathNode> nodeBinaryHeap = new List<PathNode> { null };

            // Out of nodes on the open list, no path found
            return null;
        }

        private int ComputeHDistance(PathNode nodeA, PathNode nodeB)
        {
            int xDist = Mathf.Abs(nodeA.GetX() - nodeB.GetX());
            int yDist = Mathf.Abs(nodeA.GetY() - nodeB.GetY());
            int remaining = Mathf.Abs(xDist - yDist);

            return DIAGONAL_COST * Mathf.Min(xDist, yDist) + STRAIGHT_COST * remaining;
        }

        /**
         * Return the lowest f cost node of the given list
         */
        private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
        {
            PathNode lowestFCostNode = pathNodeList[0];

            for (int x = 1; x < pathNodeList.Count; ++x)
            {
                if (pathNodeList[x].GetF() < lowestFCostNode.GetF()) lowestFCostNode = pathNodeList[x];
            }

            return lowestFCostNode;
        }

        /**
         * Compute path from the given node
         */
        private List<PathNode> ComputePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            path.Add(endNode);

            PathNode currentNode = endNode;

            while (currentNode.GetParent() != null)
            {
                path.Add(currentNode.GetParent());
                currentNode = currentNode.GetParent();
            }

            path.Reverse();

            return path;
        }

        /**
         * Get the neighbors nodes of the given node
         */
        private List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();

            if (node.GetX() - 1 >= 0)
            {
                // Left
                neighbors.Add(this.grid.GetNode(node.GetX() - 1, node.GetY()));
                // Left Down
                if (node.GetY() - 1 >= 0) neighbors.Add(this.grid.GetNode(node.GetX() - 1, node.GetY() - 1));
                // Left Up
                if (node.GetY() + 1 < this.grid.GetHeight()) neighbors.Add(this.grid.GetNode(node.GetX() - 1, node.GetY() + 1));
            }
            
            if (node.GetX() + 1 < this.grid.GetWidth())
            {
                // Right
                neighbors.Add(this.grid.GetNode(node.GetX() + 1, node.GetY()));
                // Right Down
                if (node.GetY() - 1 >= 0) neighbors.Add(this.grid.GetNode(node.GetX() + 1, node.GetY() - 1));
                // Right Up
                if (node.GetY() + 1 < this.grid.GetHeight()) neighbors.Add(this.grid.GetNode(node.GetX() + 1, node.GetY() + 1));
            }

            // Down
            if (node.GetY() - 1 >= 0) neighbors.Add(this.grid.GetNode(node.GetX(), node.GetY() - 1));
            // Up
            if (node.GetY() + 1 < this.grid.GetHeight()) neighbors.Add(this.grid.GetNode(node.GetX(), node.GetY() + 1));

            return neighbors;
        }
    }
}
