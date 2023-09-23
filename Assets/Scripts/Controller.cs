using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Vector3 MousePosition;
    public LayerMask whatIsGround;
    public GameObject booomClone;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePosition.z = 0;
            Instantiate(booomClone, MousePosition, Quaternion.identity);
        }
    }
}
