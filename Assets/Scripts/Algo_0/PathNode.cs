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

namespace Algo_0
{
    public class PathNode : IDrawable
    {
        private int G; // Walking cost from the start node
        private int H; // Heuristic cost to reach the end node
        private int F; // G + H

        private PathNode parent;

        private int x;
        private int y;
        private Grid<PathNode> grid;

        private bool isWalkable;

        private GameObject cellQuad;

        private static Color CELL_COLOR = Utils.GetColorFromString("9c9c9c");
        private static Color FINAL_PATH_NODE_COLOR = Utils.GetColorFromString("549648");
        private static Color START_NODE_COLOR = Utils.GetColorFromString("e6da73");
        private static Color END_NODE_COLOR = Utils.GetColorFromString("86db76");
        private static Color NOT_WALKABLE_NODE_COLOR = Utils.GetColorFromString("AFAFAF");

        public PathNode(int x, int y, Grid<PathNode> grid)
        {
            this.x = x;
            this.y = y;
            this.grid = grid;
            this.isWalkable = true;
        }

        public void Draw()
        {
            // Draw cell's background
            Vector3 worldPosition = this.grid.GetWorldPosition(this.x, this.y);
            Vector3 newWorldPosition = new Vector3(
                worldPosition.x,
                worldPosition.y,
                Grid<PathNode>.CELL_BACKGROUND_DEPTH
            );
            this.cellQuad = MeshUtils.CreateQuad(
                this.grid.GetCellSize() - this.grid.GetBorderSize(),
                this.grid.GetCellSize() - this.grid.GetBorderSize(),
                null,
                newWorldPosition,
                CELL_COLOR,
                "Cell: {" + this.x + "," + this.y + "}"
            );
        }

        /**
         * Undraw the associate quad
         */
        public void UnDraw()
        {
            GameObject.Destroy(this.cellQuad);
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
         * Mark the cell as being from final path
         */
        public void MarkAsPathCell()
        {
            this.cellQuad.GetComponent<MeshRenderer>().sharedMaterial.color = FINAL_PATH_NODE_COLOR;
        }

        /**
         * Mark the cell as start node
         */
        public void MarkAsStart()
        {
            this.cellQuad.GetComponent<MeshRenderer>().sharedMaterial.color = START_NODE_COLOR;
        }

        /**
         * Mark the cell as end node
         */
        public void MarkAsEnd()
        {
            this.cellQuad.GetComponent<MeshRenderer>().sharedMaterial.color = END_NODE_COLOR;
        }

        /**
         * Set the node as walkable or not
         */
        public void SetWalkable(bool isWalkable)
        {
            this.isWalkable = isWalkable;
            this.cellQuad.GetComponent<MeshRenderer>().sharedMaterial.color = this.isWalkable ? CELL_COLOR : NOT_WALKABLE_NODE_COLOR;
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
