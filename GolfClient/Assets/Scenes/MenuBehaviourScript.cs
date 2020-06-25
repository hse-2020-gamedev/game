using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuBehaviourScript : MonoBehaviour
{
    public string GameSceneLevel0Name;
    public string GameSceneLevel1Name;
    public string GameSceneLevel2Name;

    public GameObject MainPanel;
    public Button StartButon;
    public Button QuitButton;

    public GameObject ChoosePlayerPanel;
    public Button OnePlayerButton;
    public Button TwoPlayersButton;
    public Button NetworkButton;
    public Button NetworkBotsButton;

    public GameObject ChooseLevelPanel;
    public Button Level0Button;
    public Button Level1Button;
    public Button Level2Button;

    public Dropdown PlayerOneSelector;
    public Dropdown PlayerTwoSelector;
    public Dropdown GameTypeSelector;

    public static bool Remote;
    public static GameSettings Settings;

    // Start is called before the first frame update
    void Start()
    {
        StartButon.onClick.AddListener(() =>
        {
            MainPanel.SetActive(false);
            // ChoosePlayerPanel.SetActive(true);
            ChooseLevelPanel.SetActive(true);
        });

        QuitButton.onClick.AddListener(() =>
        {
            Debug.Log("quit clicked");
            Application.Quit();
        });

        // void OpenChooseLevelPanel()
        // {
        //     ChoosePlayerPanel.SetActive(false);
        //     ChooseLevelPanel.SetActive(true);
        // }
        //
        // OnePlayerButton.onClick.AddListener(() =>
        // {
        //     Remote = false;
        //     Settings = new[] {PlayerType.Human, PlayerType.DummyAI};
        //     OpenChooseLevelPanel();
        // });
        //
        // NetworkButton.onClick.AddListener(() =>
        // {
        //     Remote = true;
        //     Settings = new[] {PlayerType.Human, PlayerType.Human};
        //     OpenChooseLevelPanel();
        // });

        Level0Button.onClick.AddListener(OnLevel0ButtonClick);
        Level1Button.onClick.AddListener(OnLevel1ButtonClick);
        Level2Button.onClick.AddListener(OnLevel2ButtonClick);
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

    private void LoadLevel(string sceneName)
    {
        Settings = new GameSettings {SceneName = sceneName, PlayerTypes = new PlayerType[2]};
        Settings.PlayerTypes[0] = (PlayerType) Enum.Parse(
            typeof(PlayerType),
            PlayerOneSelector.options[PlayerOneSelector.value].text);
        // Debug.LogWarning($"Player two selector: '{PlayerTwoSelector.options[PlayerTwoSelector.value].text}' " +
        //                  $"{PlayerTwoSelector.options[PlayerTwoSelector.value].text == "EvilAI"}");
        Settings.PlayerTypes[1] = (PlayerType) Enum.Parse(
            typeof(PlayerType),
            PlayerTwoSelector.options[PlayerTwoSelector.value].text);
        Remote = GameTypeSelector.options[GameTypeSelector.value].text == "Remote";

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
