using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/** Injectable Code to handle scoring.
 * I _could_ make and interface which this implments
 * But I'm not going to for simplicity
 * 
 * After it was built, I realised it doesn't have anything to do with a gameobject
 * so doesn't even need to be a monobehaviour
 */
public class ZScoreManager
{
    private int count=0; // the 'score'

    [Inject]
    ZSignals.GameWon gameWonSignal;
    [Inject]
    ZSignals.ScoreUpdated scoreUpdatedSignal;

    // this is only part of the class which is meant to be called
    // if this were an interface this is the only function we would have
    public void AddScore(int amount)
    {
        count += amount;
        scoreUpdatedSignal.Fire(count);
        if (count>=12) // don't like this have coded number, but will leave for now
        {
            gameWonSignal.Fire();
        }
    }
}
