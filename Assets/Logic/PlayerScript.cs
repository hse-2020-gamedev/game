using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour, PlayerI
{
    private StateI gameState;
    private PlayerState state = PlayerState.WAIT;
    Vector3 result;

    public Slider slider;
    public DirectionSelectorScript DirectionSelector;

    public Vector3 GetResult()
    {
        state = PlayerState.WAIT;
        return result;
    }

    public PlayerState GetState() => state;

    public void Turn(StateI gameState)
    {
        this.gameState = gameState;
        state = PlayerState.ACTIVE;
        gameObject.SetActive(true);
        DirectionSelector.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            state = PlayerState.READY;
            result = slider.value * DirectionSelector.GetDirection();
            gameObject.SetActive(false);
        }
    }
}
