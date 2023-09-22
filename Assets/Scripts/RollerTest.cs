using System.Collections;
using UnityEngine;

public class RollerTest : Weapons
{
    public PolygonCollider2D polygonCollider;
    public int movePoint;
    public CapsuleCollider2D capsuleCollider;
    public Vector2 closePosition;
    new void Start()
    {
        base.Start();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            capsuleCollider.isTrigger = true;
            rigidbody.gravityScale = 0;
            movePoint = 0;
            int point = 0;
            foreach (var moveStartPosition in polygonCollider.points)
            {
                if (Vector2.Distance(new Vector2(moveStartPosition.x * 1.5f, moveStartPosition.y * 1.5f - 3), transform.position)
                < Vector2.Distance(new Vector2(closePosition.x * 1.5f, closePosition.y * 1.5f - 3), transform.position))
                {
                    Debug.Log($"{moveStartPosition}, {transform.position}");
                    closePosition = moveStartPosition;
                    movePoint = point;
                }
                ++point;
            }
            StartCoroutine(Explosion(movePoint));
        }
    }
    IEnumerator Explosion(int _movePoint)
    {
        while (!Mathf.Approximately(transform.position.x, polygonCollider.points[_movePoint].x * 1.5f) || !Mathf.Approximately(transform.position.y, polygonCollider.points[_movePoint].y * 1.5f - 3))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(polygonCollider.points[_movePoint].x * 1.5f, (polygonCollider.points[_movePoint].y) * 1.5f - 3), 0.01f);
            yield return null;
        }
        if (_movePoint + 1 > polygonCollider.points.Length - 1)
        {
            _movePoint = 0;
        }
        else
        {
            ++_movePoint;
        }
        StartCoroutine(Explosion(_movePoint));
    }
}
