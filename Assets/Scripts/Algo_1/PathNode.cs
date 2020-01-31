using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoaDev.Utils;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          PathNode class
 * ------------------------------------------------
 */

namespace Algo_1
{
    public class PathNode
    {
        private int G; // Walking cost from the start node
        private int H; // Heuristic cost to reach the end node
        private int F; // G + H

        private PathNode parent;

        private int x;
        private int y;
        private Vector3 worldPosition;
        private Grid grid;
        private bool isWalkable;

        public PathNode(Vector3 worldPosition, Grid grid, bool isWalkable = true)
        {
            this.worldPosition = worldPosition;
            this.grid = grid;
            this.isWalkable = isWalkable;
        }

        /**
         * Compute world position based on x and y indices
         */
        private void ComputeWorldPosition()
        {
            this.worldPosition = new Vector3(this.x, this.y) * this.grid.nodeRadius;
        }

        public override string ToString()
        {
            // return this.G + "\n" + this.H + "\n" + this.F;
            return x + "," + y;
        }

        /**
         * Set current node's parent
         */
        public void SetParent(PathNode parent)
        {
            this.parent = parent;
        }

        /**
         * Set G cost
         */
        public void SetG(int cost)
        {
            this.G = cost;
        }

        /**
         * Set H cost
         */
        public void SetH(int cost)
        {
            this.H = cost;
        }

        /**
         * Calculate F cost
         */
        public void ComputeF()
        {
            this.F = this.G + this.H;
        }

        /**
         * Return node's x position
         */
        public int GetX()
        {
            return this.x;
        }

        /**
         * Return node's y position
         */
        public int GetY()
        {
            return this.y;
        }

        /**
         * Return node's world position
         */
        public Vector3 GetWorldPosition()
        {
            return this.worldPosition;
        }

        /**
         * Return noes's G cost
         */
        public int GetG()
        {
            return this.G;
        }

        /*
         * Return node's F cost
         */
        public int GetF()
        {
            return this.F;
        }

        /**
         * Return node's parent
         */
        public PathNode GetParent()
        {
            return this.parent;
        }

        /**
         * Set the node as walkable or not
         */
        public void SetWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
        }

        /**
         * Return if the node is walkable
         */
        public bool IsWalkable()
        {
            return this.isWalkable;
        }
    }
}