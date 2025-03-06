using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBase : MonoBehaviour
{
    public ColorInitialize colorInitialize;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball")) {
            colorInitialize.InvokeColor("Red", 0.5f);
            AudioManager.PlaySound("BassHit", 1.0f, 1.3f);
        }
    }
}
