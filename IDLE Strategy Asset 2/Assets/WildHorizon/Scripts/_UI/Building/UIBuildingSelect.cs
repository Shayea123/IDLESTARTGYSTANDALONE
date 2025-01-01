using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIBuildingSelect : MonoBehaviour
    {
        [Header("Components")]
        public UIAudio _audio;
        public UIBuild _build;

        private BuildingsType operatingMode;

        public GameObject panel;
        public Transform content;
        public UIBuildingSlot prefab;
        public Text textBuildingsType;

        public Button buttonSocial;
        public Button buttonMining;
        public Button buttonStorages;
        public Button buttonProduction;
        public Button buttonMilitary;

        [Header("Building Info")]
        public Text textBuildingDescription;
        public Image imageInfo;

        public GameObject imageTime;
        public TextMeshProUGUI textTimeValue;

        public Button buttonManage;
        public Button buttonBuild;
        public Button buttonADS;

        [Header("Components")]
        public Color colorNormal;
        public Color colorSelected;

        private List<ScriptableBuilding> displayedBuildings;
        //int selectedBuildingIndex = 0;
        public static ScriptableBuilding selectedBuilding;
        public BuildingOnMap[] buildingsOnScene;

        public void Show()
        {
            GenerateBuildingsList();
            panel.SetActive(true);
        }

        private void OnEnable()
        {
            GenerateBuildingsList();
        }

        private void Start()
        {
            buttonSocial.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                operatingMode = BuildingsType.Social;
                GenerateBuildingsList();
            });
            buttonStorages.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                operatingMode = BuildingsType.Storages;
                GenerateBuildingsList();
            });
            buttonMining.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                operatingMode = BuildingsType.Mining;
                GenerateBuildingsList();
            });
            buttonProduction.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                operatingMode = BuildingsType.Factorys;
                GenerateBuildingsList();
            });
            buttonMilitary.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();
                operatingMode = BuildingsType.Military;
                GenerateBuildingsList();
            });

            buttonADS.onClick.AddListener(() =>
            {
                _audio.PlaySoundButtonClick();

                //GameADS.singleton.ShowRewardedAd(AdsType.building);
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (panel.activeSelf)
            {
                Player player = Player.localPlayer;
                if (player != null)
                {
                    buttonSocial.interactable = (operatingMode != BuildingsType.Social);
                    buttonStorages.interactable = (operatingMode != BuildingsType.Storages);
                    buttonMining.interactable = (operatingMode != BuildingsType.Mining);
                    buttonProduction.interactable = (operatingMode != BuildingsType.Factorys);
                    buttonMilitary.interactable = (operatingMode != BuildingsType.Military);

                    //description
                    textBuildingDescription.text = selectedBuilding.GetDescriptionByLanguage(Localization.languageCurrent);

                    //find this building index in Global list
                    int selectedBuildingIndex = player.buildings.FindIndex(selectedBuilding);
                    Building building = player.buildings.buildings[selectedBuildingIndex];

                    //current level
                    if (building.level > 0)
                    {
                        if (building.underConstruction == false) textBuildingsType.text = Localization.Translate(selectedBuilding.name) + " : " + Localization.Translate("lv") + " " + building.level.ToString();
                        else textBuildingsType.text = Localization.Translate(selectedBuilding.name) + " : "+ Localization.Translate("lv") + " " + building.level + " --> " + (building.level + 1);

                        //button build
                        if (building.level < building.data.spritesByLevel.Length - 1)
                        {
                            buttonBuild.gameObject.GetComponentInChildren<Text>().text = Localization.Translate("Upgrade");
                        }
                    }
                    else
                    {
                        //name by language
                        textBuildingsType.text = Localization.Translate(selectedBuilding.name);

                        //button build
                        buttonBuild.gameObject.GetComponentInChildren<Text>().text = Localization.Translate("Build");
                    }

                    //sprite
                    if (building.level == selectedBuilding.maxLevel) imageInfo.sprite = selectedBuilding.spritesByLevel[building.level - 1];
                    else imageInfo.sprite = selectedBuilding.spritesByLevel[building.level];
                    imageInfo.color = Color.white;

                    // instantiate/destroy enough slots
                    UIUtils.BalancePrefabs(prefab.gameObject, displayedBuildings.Count, content);

                    //refresh all slots
                    for (int i = 0; i < displayedBuildings.Count; i++)
                    {
                        UIBuildingSlot slot = content.transform.GetChild(i).GetComponent<UIBuildingSlot>();
                        slot.image.sprite = displayedBuildings[i].spriteForPreview[0];

                        //building name
                        slot.textName.text = Localization.Translate(displayedBuildings[i].name);

                        int buildingIndex = player.buildings.FindIndex(displayedBuildings[i]);
                        Building tempBuilding = player.buildings.buildings[buildingIndex];

                        //time
                        if (tempBuilding.time > 0)
                        {
                            slot.time.SetActive(true);
                            slot.textTime.text = Utils.PrettySeconds((int)tempBuilding.time);
                        }
                        else slot.time.SetActive(false);

                        //workers
                        slot.textWorkers.text = tempBuilding.InhabitantsInBuilding().ToString();

                        //level - amount
                        slot.textAmount.text = tempBuilding.level + "/" + (displayedBuildings[i].maxLevel);

                        int icopy = i;
                        slot.button.onClick.SetListener(() =>
                        {
                            int indexOnScene = BuildingIndexOnScene(selectedBuilding);
                            if (indexOnScene != -1)
                            {
                                Camera.main.orthographicSize = 3.5f;
                                Vector3 pos = new Vector3(buildingsOnScene[indexOnScene].gameObject.transform.position.x,
                                    buildingsOnScene[indexOnScene].gameObject.transform.position.y - 1.5f,
                                    Camera.main.transform.position.z);
                                Camera.main.transform.position = pos;
                            }

                            selectedBuildingIndex = icopy;
                            selectedBuilding = displayedBuildings[icopy];

                        //play sound
                        if (selectedBuilding.sound != null) _audio.PlaySound(selectedBuilding.sound);
                            else _audio.PlaySoundButtonClick();
                        });

                        slot.redPrompt.SetActive(player.buildings.CheckBuildForQuest(tempBuilding));

                        slot.imageSelected.color = selectedBuildingIndex != buildingIndex ? colorNormal : colorSelected;
                    }

                    //build time
                    imageTime.SetActive(Mathf.Round(building.time) > 0);
                    textTimeValue.text = Mathf.Round(building.time) > 0 ? Utils.PrettySeconds((int)building.time) : "";

                    //ads
                    //buttonADS.gameObject.SetActive(Advertisement.  .IsReady(GameADS.singleton.rewardedVideo) && building.time > 1 && building.adsAmount > 0);

                    //button "Build / Upgrade"
                    buttonBuild.gameObject.SetActive(building.level < building.data.maxLevel);

                    //button "Manage"
                    buttonManage.gameObject.SetActive(building.data.IsManaged() && building.level > 0);
                    buttonManage.onClick.SetListener(() =>
                    {
                        _audio.PlaySoundButtonClick();
                        panel.SetActive(false);

                    //if entity is Camp
                    if (building.data.Equals(player.accessToTheTown.item)) UITown.singleton.panel.SetActive(true);
                        else _build.OpenBuildingManager(building.data);
                    });
                }
                else panel.SetActive(false);
            }
        }

        void GenerateBuildingsList()
        {
            //generate new list
            displayedBuildings = new List<ScriptableBuilding>();
            foreach (var item in ScriptableBuilding.dict)
            {
                ScriptableBuilding building = item.Value;

                if (building.buildingType == operatingMode)
                {
                     displayedBuildings.Add(building);
                }
            }

            //sort buildings by
            displayedBuildings.Sort((a, b) =>
            {
                return a.sortValue.CompareTo(b.sortValue);
            });

            if (displayedBuildings.Count > 0)
                selectedBuilding = displayedBuildings[0];
        }

        public int BuildingIndexOnScene(ScriptableBuilding building)
        {
            for (int i = 0; i < buildingsOnScene.Length; i++)
            {
                if (buildingsOnScene[i] != null && buildingsOnScene[i].building.name == building.name) return i;
            }
            return -1;
        }
    }
}


