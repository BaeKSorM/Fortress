using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    public KeyCode cameraLeftKey;
    public KeyCode cameraRightKey;
    public float mapLeftMax;
    public float mapRightMax;
    public float sideBlock;
    void Start()
    {
        Instance = this;
    }
    void Update()
    {
        CameraMove();
    }
    void CameraMove()
    {

        if (Input.GetKey(cameraLeftKey) && transform.position.x > mapLeftMax + sideBlock)
        {
            transform.position += new Vector3(-0.01f, 0, 0);
        }
        if (Input.GetKey(cameraRightKey) && transform.position.x < mapRightMax - sideBlock)
        {
            transform.position += new Vector3(0.01f, 0, 0);
        }
    }
}
