using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class ItemsPrefabForAnimation : MonoBehaviour
    {
        public Image sprite;
        public Image rarity;
        public TextMeshProUGUI textAmount;

        public Transform target;
        public Vector2 startPos;
        public Vector2 endPos;
        public float speed;
        private float progress;

        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
            endPos = new Vector2(target.position.x, target.position.y);
        }

        private void FixedUpdate()
        {
            transform.position = Vector2.Lerp(startPos, endPos, progress);
            progress += speed;

            if ((target.position - transform.position).magnitude < 0.1f) Destroy(this.gameObject);
        }
    }
}


