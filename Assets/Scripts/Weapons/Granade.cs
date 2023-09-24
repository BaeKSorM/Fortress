using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : Weapons
{
    public float explosionDelay;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(ExplosionDelay(explosionDelay));
        }
    }
    IEnumerator ExplosionDelay(float _explosionDelay)
    {
        yield return new WaitForSeconds(_explosionDelay);
        StartCoroutine(Explosion());
    }
}