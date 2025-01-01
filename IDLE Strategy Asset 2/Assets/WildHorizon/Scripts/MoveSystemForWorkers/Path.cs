using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum PathTypes { lener, loop }
    public enum MovementDirection { clockwise, counterclockwise }

    public class Path : MonoBehaviour
    {
        public PathTypes pathType;
        public MovementDirection movementDirection = MovementDirection.clockwise;

        public void OnDrawGizmos()
        {
            if (transform.childCount < 2) return;

            for (int i = 1; i < transform.childCount; i++)
                Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);

            if (pathType == PathTypes.loop)
                Gizmos.DrawLine(transform.GetChild(0).position, transform.GetChild(transform.childCount - 1).position);
        }

        public IEnumerator<Transform> GetNextPathPoint(int moveingTo)
        {
            if (transform.childCount < 2) yield break;

            while (true)
            {
                yield return transform.GetChild(moveingTo);

                if (transform.childCount == 1) continue;

                if (pathType == PathTypes.lener)
                {
                    if (moveingTo <= 0) movementDirection = MovementDirection.clockwise;
                    else if (moveingTo >= transform.childCount - 1) movementDirection = MovementDirection.counterclockwise;
                }

                moveingTo += movementDirection == MovementDirection.clockwise ? 1 : -1;

                if (pathType == PathTypes.loop)
                {
                    if (moveingTo >= transform.childCount) moveingTo = 0;
                    else if (moveingTo < 0) moveingTo = transform.childCount - 1;
                }
            }
        }
    }
}


