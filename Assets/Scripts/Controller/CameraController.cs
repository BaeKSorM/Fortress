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
        transform.position = new Vector3(UIManager.Instance.mapSpawnPoints[UIManager.Instance.playerOrder].x / 5,
                                         0,
                                        -10);
    }
    void Update()
    {
        CameraMove();
    }
    void CameraMove()
    {

        if (Input.GetKey(cameraLeftKey))
        {
            if (transform.position.x > mapLeftMax + sideBlock)
            {
                transform.position += new Vector3(-1f * cameraMoveSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.position = new Vector3(mapLeftMax + sideBlock, 0, -10);
            }
        }
        else
        {

        }
        if (Input.GetKey(cameraRightKey))
        {
            if (transform.position.x < mapRightMax - sideBlock)
            {
                transform.position += new Vector3(1f * cameraMoveSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.position = new Vector3(mapRightMax - sideBlock, 0, -10);
            }
        }
    }
}
