using System;
using UnityEngine;

namespace IdleStrategyKit
{
    public enum QuestTaskType { none, predecessor, resources, buildings, reserches, sendInhabitans, townRename }

    [Serializable]
    public struct QuestTask
    {
        public Sprite sprite;
        public string name;
        public bool completed;
        public QuestTaskType type;
        public ScriptableObject scObject;

        // constructors
        public QuestTask(Sprite sprite, string name, bool completed, QuestTaskType type, ScriptableObject scObject)
        {
            this.sprite = sprite;
            this.name = name;
            this.completed = completed;
            this.type = type;
            this.scObject = scObject;
        }
    }
}


