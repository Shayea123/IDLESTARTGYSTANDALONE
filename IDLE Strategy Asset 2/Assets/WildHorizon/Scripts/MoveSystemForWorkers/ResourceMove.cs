using System.Collections.Generic;
using UnityEngine;

namespace IdleStrategyKit
{
    public class ResourceMove : MonoBehaviour
    {
        public List<ScriptableItemAndAmount> resources;
        public ScriptableBuilding moversTarget;
        public int resourcesMoveAmount = 100;


        /*void OnTriggerExit2D(Collider2D col)
        {
            Entity entity = col.GetComponentInParent<Entity>();

            if (entity != null && entity is Building building)
            {
                Player player = Player.localPlayer;

                //если караван начинает движение от шахты 
                if (building.indexInGlobalList != -1 && player.buildings.buildings[building.indexInGlobalList].resources.Length > 0)
                {
                    //target
                    moversTarget = building.target;

                    Buildings temp = player.buildings.buildings[building.indexInGlobalList];

                    int value = resourcesMoveAmount+ (int)(resourcesMoveAmount * player.inhabitants.IncreasesMaximumWeight());
                    resources = new List<ScriptableItemAndAmount>();

                    for (int i = temp.resources.Length - 1; i >= 0; i--)
                    {
                        if (temp.resources[i] >= value)
                        {
                            ScriptableItemAndAmount res = new ScriptableItemAndAmount();
                            res.item = player.buildings.buildings[building.indexInGlobalList].data.minedResources[i].item;
                            res.amount = value;
                            resources.Add(res);

                            //
                            temp.resources[i] -= value;

                            Debug.Log("взял товар " + res.item.name + " / " + res.amount);

                            break;
                        }
                        else
                        {
                            ScriptableItemAndAmount res = new ScriptableItemAndAmount();
                            res.item = player.buildings.buildings[building.indexInGlobalList].data.minedResources[i].item;
                            value = value - (int)temp.resources[i];
                            res.amount = (int)temp.resources[i];
                            resources.Add(res);

                            //
                            temp.resources[i] = 0;

                            Debug.Log("взял товар " + res.item.name + " / " + res.amount);
                        }
                    }

                    player.buildings.buildings[building.indexInGlobalList] = temp;
                }
            }


        }*/

        /*private void OnTriggerEnter2D(Collider2D collision)
        {
            Building building = collision.GetComponentInParent<Building>();

            if (resources.Count > 0)
            {
                if (moversTarget != null && moversTarget.name == building.building.name)
                {
                    if (resources.Count > 0)
                    {
                        moversTarget = null;

                        Player player = Player.localPlayer;

                        //add resource
                        for (int i = 0; i < resources.Count; i++)
                        {
                            Debug.Log("принес товар " + resources[i].item + " / " + resources[i].amount);
                            player.items.AddResource(resources[i]);
                        }

                        resources = new List<ScriptableItemAndAmount>();
                    }
                }
            }
        }*/
    }
}


