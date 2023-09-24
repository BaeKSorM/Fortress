using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class One_Bounce : Weapons
{
    public bool isBounce;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (isBounce)
            {
                StartCoroutine(Explosion());
            }
            else
            {
                isBounce = true;
            }
        }
    }
}
