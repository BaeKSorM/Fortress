using System;
using UnityEngine;

public class Shot : Weapons
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Explosion());
        }
    }
}
