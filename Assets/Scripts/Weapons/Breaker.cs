using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Breaker : Weapons
{
    public float moveX;
    public float moveY;
    public bool isDroped;
    public bool isBroken;
    public GameObject clone;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (!isDroped)
            {
                isDroped = true;
                clone = Instantiate(gameObject);
                rigidbody.velocity = Vector2.zero;
                rigidbody.AddForce(new Vector2(moveX, moveY), ForceMode2D.Impulse);
                clone.GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveX, moveY), ForceMode2D.Impulse);
            }
            else if (isBroken)
            {
                StartCoroutine(Explosion());
            }
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isBroken = true;
        }
    }

}
