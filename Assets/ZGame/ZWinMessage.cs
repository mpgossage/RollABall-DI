using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// this component will turn on/off the win message based upon an event
[RequireComponent(typeof(Text))]
public class ZWinMessage : MonoBehaviour
{
    [Inject]
    ZSignals.GameWon onGameWonSignal; // we want this signal (note the inject header)

    private Text myText;
	// Use this for initialization
	void Start ()
    {
        myText = GetComponent<Text>();
        myText.enabled = false;

        // when we get the signal, call this fn
        onGameWonSignal.Listen(OnGameWon);
    }

    private void OnDestroy()
    {
        onGameWonSignal.Unlisten(OnGameWon);
    }

    void OnGameWon()
    {
        Debug.Log("ZWinMessage gets GameWon");
        myText.enabled = true;
    }
}
