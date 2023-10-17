using Unity.VisualScripting;
using UnityEngine;

public class Breaker : Weapons
{
    public float moveX;
    public float moveY;
    public bool isDroped;
    public bool isBroken;
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            if (!isDroped)
            {
                isDroped = true;
                GameObject leftClone = Instantiate(gameObject);
                GameObject rightClone = Instantiate(gameObject);
                leftClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(moveX, moveY), ForceMode2D.Impulse);
                rightClone.GetComponent<Rigidbody2D>().AddForce(new Vector2(-moveX, moveY), ForceMode2D.Impulse);
                Destroy(gameObject);
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
