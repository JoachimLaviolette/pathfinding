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
        private int movementPenalty;
        private bool isWalkable;

        public PathNode(Vector3 worldPosition, int x, int y, int movementPenalty, bool isWalkable = true)
        {
            this.worldPosition = worldPosition;
            this.x = x;
            this.y = y;
            this.movementPenalty = movementPenalty;
            this.isWalkable = isWalkable;
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
         * Return node's movement penalty
         */
        public int GetMovementPenalty()
        {
            return this.movementPenalty;
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

        public override string ToString()
        {
            return "(" + this.x + "," + this.y + ")";
        }
    }
}