using UnityEngine;

public class Spliter : Weapons
{
    public float groundCheckDistance;
    public LayerMask whatIsGround;
    public bool canSplit;
    public bool canCopy;
    public float rotateAngle;
    void Update()
    {
        if (rigidbody.velocity.y < 0)
        {
            Split();
        }
    }
    void Split()
    {
        canSplit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        if (canSplit && canCopy)
        {
            canCopy = false;
            rigidbody.velocity = Vector2.zero;
            GameObject clone = Instantiate(gameObject);
            rigidbody.AddForce(Vector2.left * rotateAngle, ForceMode2D.Impulse);
            clone.GetComponent<Rigidbody2D>().AddForce(Vector2.right * rotateAngle, ForceMode2D.Impulse);
        }
        else if (!canSplit && !canCopy)
        {
            canCopy = true;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(Explosion());
        }
    }
}
