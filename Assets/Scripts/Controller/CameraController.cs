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
    public float cameraMoveSpeed;
    void Start()
    {
        Instance = this;
        Transform playerTransform = UIManager.Instance.tank.transform;
        transform.position = new Vector3(playerTransform.position.x + 8 * playerTransform.localScale.x, 0, -10);
    }
    void Update()
    {
        CameraMove();
    }
    void CameraMove()
    {

        if (Input.GetKey(cameraLeftKey) && transform.position.x > mapLeftMax + sideBlock)
        {
            transform.position += new Vector3(-1f * cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(cameraRightKey) && transform.position.x < mapRightMax - sideBlock)
        {
            transform.position += new Vector3(1f * cameraMoveSpeed * Time.deltaTime, 0, 0);
        }
    }
}
