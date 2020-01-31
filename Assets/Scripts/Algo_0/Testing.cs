using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JoaDev.Utils;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          Testing class
 * ------------------------------------------------
 */

namespace Algo_0
{
    public class Testing : MonoBehaviour
    {
        private Pathfinding pathfinding;

        private void Start()
        {
            this.pathfinding = new Pathfinding(20, 10);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !this.pathfinding.GetGrid().IsDebugOn())
            {
                Vector3 mouseWP = Utils.GetMouseWorldPosition();
                this.pathfinding.GetGrid().GetXY(mouseWP, out int x, out int y);
                this.pathfinding.FindPath(0, 0, x, y);
            }

            if (Input.GetMouseButtonDown(1) && !this.pathfinding.GetGrid().IsDebugOn())
            {
                Vector3 mouseWP = Utils.GetMouseWorldPosition();
                PathNode node = this.pathfinding.GetGrid().GetElement(mouseWP);
                node.SetWalkable(!node.IsWalkable());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.pathfinding.Debug();
            }
        }
    }
}