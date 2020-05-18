using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Button StartButon;
    public Button QuitButton;
    public string GameSceneName;

    // Start is called before the first frame update
    internal void Start()
    {
        StartButon.onClick.AddListener(OnStartButtonClick);
        QuitButton.onClick.AddListener(OnQuitButtonClick);
    }

    internal void OnStartButtonClick()
    {
        Debug.Log("start clicked");
        SceneManager.LoadScene(GameSceneName);
        // TODO: create GameLoopManager2
    }

    internal void OnQuitButtonClick()
    {
        Debug.Log("quit clicked");
        Application.Quit();
    }

    // Update is called once per frame
    internal void Update()
    {
    }
}
