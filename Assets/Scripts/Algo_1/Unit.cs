using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Algo_1
{
    public class Unit : MonoBehaviour
    {
        public Transform target;

        private float speed = 3f;
        private Vector3[] path;
        private int targetIndex;

        private void Start()
        {
            PathRequestManager.RequestPath(
                this.transform.position,
                this.target.transform.position,
                OnPathFound
            );
        }

        private void OnDrawGizmos()
        {
            if (this.path != null)
            {
                for (int x = this.targetIndex; x < this.path.Length; ++x) 
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawCube(this.path[x], new Vector3(.2f, .2f, .2f));

                    if (x == this.targetIndex) Gizmos.DrawLine(this.transform.position, this.path[x]);
                    else Gizmos.DrawLine(this.path[x - 1], this.path[x]);
                }
            }
        }

        public void OnPathFound(Vector3[] newPath, bool isPathFindingSuccessful)
        {
            if (!isPathFindingSuccessful) return;

            this.path = newPath;
            this.StopCoroutine("FollowPath");
            this.StartCoroutine("FollowPath");
        }

        IEnumerator FollowPath()
        {
            Vector3 currentWaypoint = this.path[0];

            while(true)
            {
                if (this.transform.position == currentWaypoint)
                {
                    ++this.targetIndex;

                    if (this.targetIndex >= path.Length) yield break;

                    currentWaypoint = path[this.targetIndex];
                }

                this.transform.position = Vector3.MoveTowards(this.transform.position, currentWaypoint, this.speed * Time.deltaTime);

                yield return null;
            }
        }
    }
}