using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Text))]
public class ZScoreDisplay : MonoBehaviour
{
    [Inject]
    ZSignals.ScoreUpdated onScoreUpdatedSignal; // we want this signal (note the inject header)

    private Text myText;
    // Use this for initialization
    void Start()
    {
        myText = GetComponent<Text>();
        // call the update fn to set the inital score to zero
        // if we had a ref to the ScoreManager we could instead have got it from that
        // or made the score manager emit an event at startup
        OnScoreUpdate(0);

        // when we get the signal, call this fn
        onScoreUpdatedSignal.Listen(OnScoreUpdate);
    }

    private void OnDestroy()
    {
        onScoreUpdatedSignal.Unlisten(OnScoreUpdate);
    }

    void OnScoreUpdate(int value)
    {
        Debug.LogFormat("ZScoreDisplay.OnScoreUpdate {0}",value);
        myText.text = "Score: "+value;
    }
}
