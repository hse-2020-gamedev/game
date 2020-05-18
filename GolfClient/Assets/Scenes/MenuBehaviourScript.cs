using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehaviourScript : MonoBehaviour
{
    public Button StartButon;
    public Button QuitButton;
    public string GameSceneName;

    // Start is called before the first frame update
    void Start()
    {
        StartButon.onClick.AddListener(OnStartButtonClick);
    }

    void OnStartButtonClick()
    {
        Debug.Log("start clicked");
        SceneManager.LoadScene(GameSceneName);
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
