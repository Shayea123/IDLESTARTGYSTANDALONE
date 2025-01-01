using UnityEngine;

namespace IdleStrategyKit
{
    public class WorkersSystem : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabs;
        [SerializeField] private Path[] path;

        [SerializeField] private uint maxAmountOnMap = 100;
        [SerializeField] private float timeUpdate = 30;

        [Header("Move Speed")]
        [SerializeField] private float speedMin = 0.05f;
        [SerializeField] private float speedMax = 0.13f;

        private uint currentAmountOnMap = 0;

        void Start()
        {
            UISettings.StartNewStoryGame.AddListener(UpdateInhabitantsOnMap);
            InvokeRepeating(nameof(UpdateInhabitantsOnMap), 1, timeUpdate);
        }

        public void UpdateInhabitantsOnMap()
        {
            Player player = Player.localPlayer;
            if (player != null)
            {
                uint inhabitantsFree = player.inhabitants.InhabitantsFree();
                if (inhabitantsFree > maxAmountOnMap) inhabitantsFree = maxAmountOnMap;

                if (currentAmountOnMap < inhabitantsFree)
                {
                    for (uint i = currentAmountOnMap; i < inhabitantsFree; i++)
                    {
                        GameObject worker = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);

                        FollowPath _path = worker.GetComponent<FollowPath>();
                        _path.myPath = path[Random.Range(0, path.Length)];
                        _path.speed = Random.Range(speedMin, speedMax);

                        int rnd = Random.Range(0, _path.myPath.transform.childCount);
                        _path.moveingTo = rnd;
                        worker.transform.position = _path.myPath.transform.GetChild(rnd).position;
                        worker.transform.SetParent(transform);
                    }

                    currentAmountOnMap = inhabitantsFree;
                }
                else if (currentAmountOnMap > inhabitantsFree)
                {
                    for (uint i = currentAmountOnMap; i > inhabitantsFree; i--)
                    {
                        Destroy(transform.GetChild(Random.Range(0, transform.childCount)).gameObject);
                    }

                    currentAmountOnMap = inhabitantsFree;
                }
            }
        }
    }
}


