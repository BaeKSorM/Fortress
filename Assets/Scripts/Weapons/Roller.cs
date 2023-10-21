using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : Weapons
{
    public PolygonCollider2D polygonCollider;
    public int movePoint;
    public int showMovePoint;
    public CapsuleCollider2D capsuleCollider;
    public Vector2 closePosition = new Vector2(100, 100);
    public enum MoveDirection { Forward, Backward };
    public MoveDirection moveDirection;
    public float lookDirection;
    public GameObject player;
    public float explosionDelay;
    public float moveSpeed;
    new void Start()
    {
        base.Start();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Tank");
        polygonCollider = GameObject.Find("Ground").GetComponent<PolygonCollider2D>();
        lookDirection = rigidbody.velocity.x > 0 ? 1 : -1;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            StopAllCoroutines();
            lookDirection = -lookDirection;
            switch (moveDirection)
            {
                case MoveDirection.Forward:
                    StartCoroutine(Rolling(lookDirection > 0 ? --showMovePoint : ++showMovePoint));
                    break;
                case MoveDirection.Backward:
                    StartCoroutine(Rolling(lookDirection > 0 ? ++showMovePoint : --showMovePoint));
                    break;
                default:
                    break;
            }
        }
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(Explosion());
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            capsuleCollider.isTrigger = true;
            rigidbody.gravityScale = 0;
            // rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            rigidbody.velocity = Vector2.zero;
            movePoint = 0;
            int point = 0;
            foreach (var moveStartPosition in polygonCollider.points)
            {
                if (Vector2.Distance(new Vector2(moveStartPosition.x * 1.5f, moveStartPosition.y * 1.5f - 3), transform.position)
                < Vector2.Distance(new Vector2(closePosition.x * 1.5f, closePosition.y * 1.5f - 3), transform.position))
                {
                    switch (moveDirection)
                    {
                        case MoveDirection.Forward:
                            if (lookDirection == 1 ?
                                moveStartPosition.x * 1.5f > transform.position.x :
                                moveStartPosition.x * 1.5f < transform.position.x)
                            {
                                closePosition = moveStartPosition;
                                movePoint = point;
                            }
                            break;
                        case MoveDirection.Backward:
                            if (lookDirection == 1 ?
                                moveStartPosition.x * 1.5f < transform.position.x :
                                moveStartPosition.x * 1.5f > transform.position.x)
                            {
                                closePosition = moveStartPosition;
                                movePoint = point;
                            }
                            break;
                        default:
                            break;
                    }
                }
                ++point;
            }
            StartCoroutine(Rolling(movePoint));
            Debug.Log(movePoint);
            // Debug.Log(other.gameObject.GetComponent<PolygonCollider2D>().points.Length);
            // Debug.Log(polygonCollider.points.Length);
        }
    }
    IEnumerator Rolling(int _movePoint)
    {
        showMovePoint = _movePoint;
        while (explosionDelay > 0
        && (!Mathf.Approximately(transform.position.x, polygonCollider.points[_movePoint].x * 1.5f)
        || !Mathf.Approximately(transform.position.y, polygonCollider.points[_movePoint].y * 1.5f - 3)))
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(polygonCollider.points[_movePoint].x * 1.5f, polygonCollider.points[_movePoint].y * 1.5f - 3), moveSpeed * Time.deltaTime);
            explosionDelay -= Time.deltaTime;
            if (explosionDelay <= 0)
            {
                StartCoroutine(Explosion());
                break;
            }
            yield return null;
        }
        // polygonCollider.point.Length = hub는 0부터 script는 1부터
        if (explosionDelay > 0)
        {
            switch (moveDirection)
            {
                case MoveDirection.Forward:
                    _movePoint = MoveForward(_movePoint);
                    break;
                case MoveDirection.Backward:
                    _movePoint = MoveBackward(_movePoint);
                    break;
                default:
                    break;
            }
            StartCoroutine(Rolling(_movePoint));
        }
        Debug.Log(_movePoint);
    }
    int MoveForward(int _movePoint)
    {
        if (lookDirection == 1)
        {
            if (_movePoint - 1 < 0)
            {
                return polygonCollider.points.Length - 1;
            }
            else
            {
                return --_movePoint;
            }
        }
        else if (lookDirection == -1)
        {
            if (_movePoint + 1 > polygonCollider.points.Length - 1)
            {
                return 0;
            }
            else
            {
                return ++_movePoint;
            }
        }
        return 0;
    }
    int MoveBackward(int _movePoint)
    {
        if (lookDirection == 1)
        {
            if (_movePoint - 1 < 0)
            {
                return polygonCollider.points.Length - 1;
            }
            else
            {
                return ++_movePoint;
            }
        }
        else if (lookDirection == -1)
        {
            if (_movePoint + 1 > polygonCollider.points.Length - 1)
            {
                return 0;
            }
            else
            {
                return --_movePoint;
            }
        }
        return 0;
    }
}
