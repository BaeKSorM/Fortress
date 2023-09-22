using System;
using UnityEngine;

public class Shot : Weapons
{
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Explosion());
            // #if UNITY_EDITOR
            //             UnityEditor.EditorApplication.isPaused = true;
            // #endif
        }
        // Debug.Log(other.gameObject.tag);
    }
}
