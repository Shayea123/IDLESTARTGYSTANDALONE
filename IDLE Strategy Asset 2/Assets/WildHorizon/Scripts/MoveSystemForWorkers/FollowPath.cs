using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum MovementType { moveing, lerping }

    public class FollowPath : MonoBehaviour
    {
        public MovementType type = MovementType.moveing;
        public Path myPath;
        public float speed = 1;
        public float maxDistance = .1f;
        [HideInInspector]public int moveingTo = 0;

        private IEnumerator<Transform> pointInPath;

        void Start()
        {
            if (myPath == null) return;

            pointInPath = myPath.GetNextPathPoint(moveingTo);
            pointInPath.MoveNext();

            if (pointInPath.Current == null) return;

            transform.position = pointInPath.Current.position;
        }

        void Update()
        {
            if (pointInPath == null || pointInPath.Current == null) return;

            if (type == MovementType.moveing)
            {
                transform.position = Vector3.MoveTowards(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
            }
            else if (type == MovementType.lerping)
            {
                transform.position = Vector3.Lerp(transform.position, pointInPath.Current.position, Time.deltaTime * speed);
            }

            var distanceSqure = (transform.position - pointInPath.Current.position).sqrMagnitude;

            if (distanceSqure < maxDistance * maxDistance) pointInPath.MoveNext();
        }
    }
}


