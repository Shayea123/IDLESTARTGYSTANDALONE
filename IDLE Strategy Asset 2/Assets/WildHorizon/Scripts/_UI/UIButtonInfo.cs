using UnityEngine;
using UnityEngine.EventSystems;

namespace IdleStrategyKit
{
    public class UIButtonInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isDown { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
        }

    }
}


