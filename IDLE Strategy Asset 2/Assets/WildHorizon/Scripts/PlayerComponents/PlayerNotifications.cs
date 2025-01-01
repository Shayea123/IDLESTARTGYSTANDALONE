using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum NotificationsType { none, quest, resource, building, research, heroes, boosts, storageIsFull, battle }

    public class PlayerNotifications : MonoBehaviour
    {
        [HideInInspector] public List<Notifications> notifications = new List<Notifications>();

        public void RpcAddNotification(string name, NotificationsType type)
        {
            Notifications not = new Notifications(null, name, type);

            if (!notifications.Contains(not)) notifications.Add(not);
        }

        public void ShowInfo(NotificationsType type)
        {
            //if (type == NotificationsType.quest)
            //{
            //    UIMenu.showMenuType = MenuType.Quests;
            //}
            //else if (type == NotificationsType.building || type == NotificationsType.storageIsFull)
            //{
            //    UIMenu.showMenuType = MenuType.Build;
            //}
            //else if (type == NotificationsType.research)
            //{
            //    UIMenu.showMenuType = MenuType.Research;
            //}
        }
    }
}


