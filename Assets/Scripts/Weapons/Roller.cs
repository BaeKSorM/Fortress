using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Roller : Weapons
{
    public PolygonCollider2D polygonCollider;
    public int movePoint;
    public int showMovePoint;
    public CapsuleCollider2D capsuleCollider;
    public Vector2 closePosition;
    public enum MoveDirection { Forward, Backward };
    public MoveDirection moveDirection;
    public int lookDirection;
    public GameObject player;
    public float explosionDelay;
    new void Start()
    {
        base.Start();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Tank");
        polygonCollider = GameObject.Find("Ground").GetComponent<PolygonCollider2D>();
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
            StartCoroutine(Rolling(movePoint));
        }
    }
    IEnumerator Rolling(int _movePoint)
    {
        showMovePoint = _movePoint;
        while (explosionDelay > 0 && (!Mathf.Approximately(transform.position.x, polygonCollider.points[_movePoint].x * 1.5f) || !Mathf.Approximately(transform.position.y, polygonCollider.points[_movePoint].y * 1.5f - 3)))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(polygonCollider.points[_movePoint].x * 1.5f, (polygonCollider.points[_movePoint].y) * 1.5f - 3), 0.01f);
            explosionDelay -= Time.deltaTime;
            if (explosionDelay <= 0)
            {
                StartCoroutine(Explosion());

                break;
            }
            yield return null;
        }
        if (explosionDelay > 0)
        {
            if (_movePoint + 1 < polygonCollider.points.Length - 1 && _movePoint - 1 >= 0)
            {
                // ++_movePoint;
                _movePoint = polygonCollider.points[_movePoint].x < transform.position.x ? --_movePoint : ++_movePoint;
            }

            StartCoroutine(Rolling(_movePoint));
        }
    }

}
