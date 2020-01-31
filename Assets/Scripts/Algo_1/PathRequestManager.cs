using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * ------------------------------------------------
 *          Author: Joachim Laviolette
 *          PathRequestManager class
 * ------------------------------------------------
 */

namespace Algo_1
{
    public class PathRequestManager : MonoBehaviour
    {
        private Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
        private PathRequest currentPathRequest;
        private Pathfinding pathfinding;
        private bool isProcessingPath;

        private static PathRequestManager instance;

        private void Awake()
        {
            instance = this;
            this.pathfinding = this.GetComponent<Pathfinding>();
        }

        struct PathRequest
        {
            public Vector3 pathStart, pathEnd;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
            }
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newPathRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newPathRequest);
            instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!isProcessingPath && this.pathRequestQueue.Count > 0)
            {
                this.currentPathRequest = this.pathRequestQueue.Dequeue();
                this.isProcessingPath = true;
                this.pathfinding.StartFindPath(this.currentPathRequest.pathStart, this.currentPathRequest.pathEnd);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            this.currentPathRequest.callback(path, success);
            this.isProcessingPath = false;
            this.TryProcessNext();
        }
    }
}