using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWinner : MonoBehaviour
{
    public Text text;

    public void SetWinner(string name)
    {
        text.text = $"{name} win";
    }
}
