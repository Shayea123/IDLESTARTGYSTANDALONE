using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IdleStrategyKit
{
    public class LoaderForFirstScene : MonoBehaviour
    {
        public float delayTimeBeforeLoading = 2;
        public Image imageLoading;

        private void Start()
        {
            //for show loading bar
            StartCoroutine(LoadMainScene());
        }

        private IEnumerator LoadMainScene()
        {
            yield return new WaitForSeconds(delayTimeBeforeLoading);

            AsyncOperation operation = SceneManager.LoadSceneAsync(1);
            while (!operation.isDone)
            {
                float progress = operation.progress / 0.9f;
                imageLoading.fillAmount = progress;
                yield return null;
            }
        }
    }
}