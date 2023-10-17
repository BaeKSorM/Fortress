using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public static Wind Instance;
    public float windPower;
    private void Start()
    {
        Instance = this;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Weapon"))
        {
            Rigidbody2D weaponRigidbody = other.GetComponent<Rigidbody2D>();
            weaponRigidbody.AddForce(Vector2.right * windPower, ForceMode2D.Impulse);
        }
    }
}
