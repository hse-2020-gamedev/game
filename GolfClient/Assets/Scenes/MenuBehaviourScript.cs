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
    public Button Level0Button;
    public Button Level1Button;
    public GameObject MainPanel;
    public GameObject ChooseLevelPanel;
    // Start is called before the first frame update
    void Start()
    {
        Level0Button.onClick.AddListener(OnLevel0ButtonClick);  
        Level1Button.onClick.AddListener(OnLevel1ButtonClick);
        StartButon.onClick.AddListener(OnStartButtonClick);
    }

    void OnLevel0ButtonClick() {
        SceneManager.LoadScene(GameSceneLevel0Name);      
    }

    void OnLevel1ButtonClick() {
        SceneManager.LoadScene(GameSceneLevel1Name);      
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
}
