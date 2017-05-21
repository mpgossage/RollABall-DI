using UnityEngine;
using Zenject;

public class ZInstaller : MonoInstaller<ZInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<ZSignals.GameWon>();
        Container.DeclareSignal<ZSignals.ScoreUpdated>();
    }
}