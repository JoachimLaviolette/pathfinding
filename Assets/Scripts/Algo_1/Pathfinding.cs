using System;
using System.Collections;
using System.Diagnostics;
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
    public class Pathfinding : MonoBehaviour
    {
        public bool simplifyPath;

        private Grid grid;
        private PathRequestManager pathRequestManager;
        private Vector3[] waypoints = new Vector3[0];
        private bool isPathFindingSuccessful = false;

        private const int STRAIGHT_COST = 10; // 1 * 10
        private const int DIAGONAL_COST = 14; // sqrt(2) * 10

        private void Awake()
        {
            this.grid = this.GetComponent<Grid>();
            this.pathRequestManager = this.GetComponent<PathRequestManager>();
        }

        /**
         * Start the FindPath coroutine with the given positions
         */
        public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
        {
            this.StartCoroutine(FindPath(pathStart, pathEnd));
        }

        /**
         * Try to find a path to the end node
         */
        IEnumerator FindPath(Vector3 pathStart, Vector3 pathEnd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            this.GetNodes(pathStart, pathEnd, out PathNode startNode, out PathNode endNode);
            this.InitializeNodes();
            this.InitializeStartNode(startNode, endNode);

            this.SearchPath(startNode, endNode, sw);
            // this.SearchPathBinaryHeaps(startNode, endNode, sw);

            yield return null;
        }

        /**
         * Get the start and end nodes at the given positions
         */
        private void GetNodes(Vector3 pathStart, Vector3 pathEnd, out PathNode startNode, out PathNode endNode)
        {
            startNode = this.grid.GetNode(pathStart);
            endNode = this.grid.GetNode(pathEnd);
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
        private void SearchPath(PathNode startNode, PathNode endNode, Stopwatch sw)
        {
            if (!startNode.IsWalkable() || !endNode.IsWalkable()) return;

            List<PathNode> openList = new List<PathNode> { startNode }; // Nodes queued up for searching
            HashSet<PathNode> closedList = new HashSet<PathNode>(); // Nodes already searched

            while (openList.Count > 0)
            {
                PathNode currentNode = this.GetLowestFCostNode(openList);

                if (currentNode == endNode)
                {
                    this.isPathFindingSuccessful = true;
                    sw.Stop();
                    UnityEngine.Debug.Log("Path found after: " + sw.ElapsedMilliseconds + " ms.");
                    
                    break;
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode);

                foreach (PathNode neighborNode in this.grid.GetNeighbors(currentNode))
                {
                    if (closedList.Contains(neighborNode)) continue;
                    if (!neighborNode.IsWalkable())
                    {
                        closedList.Add(neighborNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.GetG() + this.ComputeHDistance(currentNode, neighborNode) + neighborNode.GetMovementPenalty();

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

            if (!this.isPathFindingSuccessful)
            {
                // Out of nodes on the open list, no path found
                sw.Stop();
                UnityEngine.Debug.Log("No path found after: " + sw.ElapsedMilliseconds + " ms.");

                return;
            }

            this.ComputePath(endNode);
            this.pathRequestManager.FinishedProcessingPath(this.waypoints, this.isPathFindingSuccessful);
        }

        /**
         * A* algorithm using binary heaps
         */
        private List<PathNode> SearchPathBinaryHeaps(PathNode startNode, PathNode endNode, Stopwatch sw)
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
         * Compute path from the given end node parent to parent
         */
        private void ComputePath(PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();
            PathNode currentNode = endNode;

            while (currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.GetParent();
            }

            this.waypoints = this.SimplifyPath(path);
        }

        /**
         * Simpligy the given path to only display direction changes
         */
        private Vector3[] SimplifyPath(List<PathNode> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 oldDir = Vector2.zero;

            for (int x = 1; x < path.Count; ++x)
            {
                if (!this.simplifyPath)
                {
                    waypoints.Add(path[x].GetWorldPosition());
                    continue;
                }

                Vector2 newDir = new Vector2(
                    path[x - 1].GetX() - path[x].GetX(), 
                    path[x - 1].GetY() - path[x].GetY()
                );

                if (newDir != oldDir) waypoints.Add(path[x].GetWorldPosition());

                oldDir = newDir;
            }

            waypoints.Reverse();
            waypoints.Add(path[0].GetWorldPosition());

            return waypoints.ToArray();
        }
    }
}
