using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehaviourScript : MonoBehaviour
{
    public Button StartButon;
    public Button QuitButton;
    public string GameSceneLevel0Name;
    public string GameSceneLevel1Name;
    public string GameSceneLevel2Name;
    public Button Level0Button;
    public Button Level1Button;
    public Button Level2Button;
    public GameObject MainPanel;
    public GameObject ChooseLevelPanel;
    // Start is called before the first frame update
    void Start()
    {
        Level0Button.onClick.AddListener(OnLevel0ButtonClick);  
        Level1Button.onClick.AddListener(OnLevel1ButtonClick);
        Level2Button.onClick.AddListener(OnLevel2ButtonClick);
        StartButon.onClick.AddListener(OnStartButtonClick);
    }

    void OnLevel0ButtonClick() {
        LoadLevel(GameSceneLevel0Name);
    }

    void OnLevel1ButtonClick()
    {
        LoadLevel(GameSceneLevel1Name);
    }

    void OnLevel2ButtonClick() {
        LoadLevel(GameSceneLevel2Name);
    }

    void OnStartButtonClick() {
        MainPanel.SetActive(false);
        ChooseLevelPanel.SetActive(true);
        
        //Debug.Log("start clicked");
        //SceneManager.LoadScene(GameSceneName);
    }

    void OnQuitButotnClick()
    {
        Debug.Log("quit clicked");
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadLevel(string sceneName)
    {
        var sceneAsync = SceneManager.LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single));
        var scene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        sceneAsync.completed += _ =>
        {
            // GetComponentInChildren doesn't work for inactive objects.
            // Hence, we have to traverse the children manually to enable GameLoopManager and UI Canvas.
            var rootTransform = scene.GetRootGameObjects()[0].GetComponent<Transform>();
            for (int i = 0; i < rootTransform.childCount; i++)
            {
                var child = rootTransform.GetChild(i).gameObject;
                if (child.GetComponent<GameLoopManager>() != null || child.GetComponent<Canvas>() != null)
                {
                    child.SetActive(true);
                }
            }
        };
    }
}
