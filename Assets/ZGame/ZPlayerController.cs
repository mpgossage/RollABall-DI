using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ZPlayerController : MonoBehaviour
{
    public float speed;

    private int count;
    private Rigidbody rb;

    [Inject]
    ZSignals.GameWon gameWonSignal;
    [Inject]
    ZSignals.ScoreUpdated scoreUpdatedSignal;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        SetCountText();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            count++;
            SetCountText();
        }
    }
    void SetCountText()
    {
        scoreUpdatedSignal.Fire(count);
        if (count >= 12)
        {
            Debug.Log("PlayerController says GameWon");
            gameWonSignal.Fire();
        }
    }
}
