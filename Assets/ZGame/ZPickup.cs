using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZPickup : MonoBehaviour
{
    [Inject]
    ZScoreManager scoreManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);    // turn me off
            scoreManager.AddScore(1);   // tell the score manager, it does the rest
        }
    }

}
