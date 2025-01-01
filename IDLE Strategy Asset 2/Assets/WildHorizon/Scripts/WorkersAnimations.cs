using UnityEngine;

namespace IdleStrategyKit
{
    public class WorkersAnimations : MonoBehaviour
    {
        private Vector3 posStart;

        void Start()
        {
            posStart = transform.position;
        }

        void Update()
        {
            //if move
            if (transform.position != posStart)
            {
                //if move from right to left
                if (transform.position.x < posStart.x)
                {
                    if (transform.rotation.y == 0)
                        transform.rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (transform.position.x > posStart.x)
                {
                    if (transform.rotation.y == 1)
                        transform.rotation = Quaternion.Euler(0, 0, 0);
                }

                posStart = transform.position;
            }
        }
    }
}


