using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : MonoBehaviour, PlayerI
{
    private StateI gameState;
    private PlayerState state = PlayerState.WAIT;
    Vector3 result;
    private readonly GameObject go;
    public UnityEngine.UI.Slider slider;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            state = PlayerState.READY;
            result = Vector3.Normalize(gameState.GatePosition() - gameState.MyBall()) * slider.value;
            gameObject.SetActive(false);
        }
    }
}
