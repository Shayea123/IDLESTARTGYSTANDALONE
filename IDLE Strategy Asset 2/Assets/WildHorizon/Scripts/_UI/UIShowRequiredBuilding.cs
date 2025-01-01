using UnityEngine;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class UIShowRequiredBuilding : MonoBehaviour
    {
        public GameObject panel;
        public Image image;
        public Button buttonBuild;
        public Text textInfo;

        [Header("Components")]
        public UIAudio _audio;

        public static UIShowRequiredBuilding singleton;
        public UIShowRequiredBuilding()
        {
            // assign singleton only once (to work with DontDestroyOnLoad when
            // using Zones / switching scenes)
            //if (singleton == null) 
            singleton = this;
        }

        private ScriptableBuilding selectBuilding;

        public void Show(ScriptableBuilding building, string message)
        {
            selectBuilding = building;
            image.sprite = building.spriteForPreview[0];
            textInfo.text = message;    
            panel.SetActive(true);
        }

        private void Start()
        {
            buttonBuild.onClick.SetListener(() =>
            {
                _audio.PlaySoundButtonClick();
                UIBuildingSelect.selectedBuilding = selectBuilding;
                UIBuild.singleton.ShowBuildingFromOtherPanels();
                panel.SetActive(false);
            });
        }
    }
}