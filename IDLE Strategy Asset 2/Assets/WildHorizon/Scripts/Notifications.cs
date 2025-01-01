using System;
using UnityEngine;

namespace IdleStrategyKit
{
    [Serializable]public struct Notifications
    {
        public Sprite image;
        public string name;
        public NotificationsType type;

        // constructors
        public Notifications(Sprite image, string name, NotificationsType type)
        {
            this.image = image;
            this.name = name;
            this.type = type;
        }
    }
}


