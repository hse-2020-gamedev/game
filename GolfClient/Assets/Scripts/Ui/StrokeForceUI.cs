using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrokeForceUI : MonoBehaviour
{
    void Start()
    {
        _gameManager = GameObject.FindObjectOfType<GameLoopManager>();
        image = GetComponent<Image>();
    }

    GameLoopManager _gameManager;
    Image image;

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = _gameManager.ForceImageFillAmount;
    }
}
