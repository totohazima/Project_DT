using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance;
    public GameObject mainCamera;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName)
    {
        LoadingScene_OnOff(true);
        StartCoroutine(Load(sceneName));
    }

    private IEnumerator Load(string sceneName)
    {
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadAsync.allowSceneActivation = false;

        float timer = 0.0f;
        while (!loadAsync.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
        Debug.Log(sceneName + "SceneProgressTime: " + timer + "√ ");
        loadAsync.allowSceneActivation = true;
        LoadingScene_OnOff(false);

        SceneManager.UnloadSceneAsync("SampleScene");
    }

    private void LoadingScene_OnOff(bool active)
    {
        mainCamera.SetActive(active);
    }
}
