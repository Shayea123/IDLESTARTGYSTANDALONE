using System.Collections;
using UnityEngine;

namespace IdleStrategyKit
{
    public class CloudManager : MonoBehaviour
    {
        public GameObject[] prefabs;

        [Header("Scale")]
        public float scaleMin = 1;
        public float scaleMax = 5;

        //Set this variable to how often you want the Cloud Manager to make clouds in seconds.
        [Header("Delay")]
        public float delayMin = 1;
        public float delayMax = 10;

        [Header("Colors")]
        public Color colorgoodWeather = Color.white;
        public Color colorRain = Color.gray;
        public Color colorStorm = Color.black;

        //If you ever need the clouds to stop spawning, set this variable to false, by doing: CloudManagerScript.spawnClouds = false;
        public static bool spawnClouds = true;

        void Start()
        {
            StartCoroutine(SpawnClouds());
        }

        IEnumerator SpawnClouds()
        {
            //This will always run
            while (true)
            {
                //Only spawn clouds if the boolean spawnClouds is true
                while (spawnClouds)
                {
                    GameObject cloud = prefabs[Random.Range(0, prefabs.Length)];

                    float scale = Random.Range(scaleMin, scaleMax);
                    cloud.transform.localScale = new Vector3(scale, scale, 1);
                    cloud.transform.position = transform.GetChild(Random.Range(0, transform.childCount)).transform.position;

                    Instantiate(cloud);

                    yield return new WaitForSeconds(Random.Range(delayMin, delayMax));
                }
            }
        }
    }
}

