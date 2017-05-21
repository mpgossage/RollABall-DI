using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// not a monobehaviour, just a holder for all the events
public static class ZSignals
{
    // the won game message
    public class GameWon : Signal<GameWon> { }

    // score has been updated message
    public class ScoreUpdated : Signal<int, ScoreUpdated> { }

}