using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Vector3 MousePosition;
    public LayerMask whatIsGround;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePosition.z = 0;
            Collider2D overCollider2d = Physics2D.OverlapCircle(MousePosition, 0.01f, whatIsGround);
            if (overCollider2d != null)
            {
                overCollider2d.transform.GetComponent<Ground>().MakeDot(MousePosition);
            }
        }
    }
}
