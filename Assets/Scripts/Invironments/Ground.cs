using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Texture2D mapTexture;
    public Texture2D newTexture;
    public SpriteRenderer sr;
    public PolygonCollider2D polygonCollider;
    public float worldWidth, worldHeight;
    public int pixelWidth, pixelHeight;
    public int minDestroyHeight;
    public CompositeCollider2D compositeCollider;
    public Dictionary<int, int> horizontalDivisions;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        newTexture = new Texture2D(mapTexture.width, mapTexture.height);
        Color[] colors = mapTexture.GetPixels();
        newTexture.SetPixels(colors);
        // newTexture = new Texture2D(100, 20);
        newTexture.Apply();
        MakeSprite();

        worldWidth = sr.bounds.size.x;
        worldHeight = sr.bounds.size.y;
        pixelWidth = sr.sprite.texture.width;
        pixelHeight = sr.sprite.texture.height;
        CameraController.Instance.mapLeftMax = -worldWidth / 2;
        CameraController.Instance.mapRightMax = worldWidth / 2;
        // Debug.Log(worldWidth + "," + worldHeight);
        // Debug.Log(pixelWidth + "," + pixelHeight);

        polygonCollider = GetComponent<PolygonCollider2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();
    }
    public void MakeHole(Collider2D _other)
    {
        CapsuleCollider2D c2d = _other.GetComponent<CapsuleCollider2D>();
        Vector2Int colliderCenter = WorldToPixel(c2d.bounds.center);
        int radius = Mathf.RoundToInt(c2d.bounds.size.x / 2 * pixelWidth / worldWidth);
        int horizontalDivision = _other.GetComponentInParent<Weapons>().horizontalDivision;
        int verticalDivision = _other.GetComponentInParent<Weapons>().verticalDivision;

        int px, nx, py, ny, distance;
        if (colliderCenter.y > minDestroyHeight)
        {
            for (int i = 0; i < radius; ++i)
            {
                distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
                for (int j = 0; j < distance; ++j)
                {
                    px = colliderCenter.x + i / horizontalDivision;
                    nx = colliderCenter.x - i / horizontalDivision;
                    py = colliderCenter.y + j / verticalDivision;
                    ny = colliderCenter.y - j / verticalDivision;

                    if (py > minDestroyHeight)
                    {
                        newTexture.SetPixel(px, py, Color.clear);
                        newTexture.SetPixel(nx, py, Color.clear);
                    }
                    if (ny > minDestroyHeight)
                    {
                        newTexture.SetPixel(px, ny, Color.clear);
                        newTexture.SetPixel(nx, ny, Color.clear);
                    }
                }
            }
        }

        #region 폐기
        // // c2d.bounds.size.x  가 홀수 면 +1
        // int widthRadius = 23;//Mathf.RoundToInt(17 * pixelWidth / worldWidth); //n*2+3
        // int heightRadius = 10;//Mathf.RoundToInt(7 * pixelWidth / worldWidth); //n

        // // Debug.Log(colliderCenter);
        // int[] x = new int[5];
        // int[] y = new int[heightRadius];
        // int xone = 1;
        // int xtwo = 2;
        // int num = 1;
        // int numCopy;
        // for (int i = 0; i < heightRadius / 2; ++i)
        // {
        //     x[0] = colliderCenter.x + 0;
        //     x[1] = colliderCenter.x + xone;
        //     x[2] = colliderCenter.x - xone;
        //     x[3] = colliderCenter.x + xtwo;
        //     x[4] = colliderCenter.x - xtwo;
        //     numCopy = num;
        //     for (int j = 0; j < heightRadius - 1;)
        //     {
        //         if (j == 0)
        //         {
        //             y[j] = colliderCenter.y + 0;
        //             ++j;
        //         }
        //         else if (numCopy > 0)
        //         {
        //             Debug.Log(numCopy);
        //             y[j] = colliderCenter.y + numCopy;
        //             y[j + 1] = colliderCenter.y - numCopy;
        //             ++numCopy;
        //             j += 2;
        //         }
        //         else
        //         {
        //             Debug.Log(numCopy);
        //             ++numCopy;
        //             j += 2;
        //         }
        //     }
        //     --num;
        //     xone += 2;
        //     xtwo += 2;
        //     for (int j = 0; j < 5; ++j)
        //     {
        //         for (int k = 0; k < heightRadius; ++k)
        //         {
        //             newTexture.SetPixel(x[j], y[k], Color.clear);
        //             Debug.Log((x[j] - colliderCenter.x) + "," + (y[k] - colliderCenter.y));
        //         }
        //     }
        // }
        #endregion
        newTexture.Apply();
        MakeSprite();

        Destroy(polygonCollider);
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        polygonCollider.usedByComposite = true;
        compositeCollider.GenerateGeometry();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Explosion"))
        {
            MakeHole(other);
        }
    }
    public void MakeDot(Vector3 pos)
    {
        Vector2Int pixelPosition = WorldToPixel(pos);
        Debug.Log(pos + "/" + pixelPosition);

        newTexture.SetPixel(pixelPosition.x, pixelPosition.y, Color.clear);
        newTexture.SetPixel(pixelPosition.x + 1, pixelPosition.y, Color.clear);
        newTexture.SetPixel(pixelPosition.x - 1, pixelPosition.y, Color.clear);
        newTexture.SetPixel(pixelPosition.x, pixelPosition.y + 1, Color.clear);
        newTexture.SetPixel(pixelPosition.x, pixelPosition.y - 1, Color.clear);

        newTexture.Apply();
        MakeSprite();
    }
    void MakeSprite()
    {
        sr.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
    }
    public Vector2Int WorldToPixel(Vector3 pos)
    {
        Vector2Int pixelPosition = Vector2Int.zero;
        var dx = pos.x - transform.position.x;
        var dy = pos.y - transform.position.y;
        // Debug.Log(dx + "," + dy);
        pixelPosition.x = Mathf.RoundToInt(0.5f * pixelWidth + dx * (pixelWidth / worldWidth));
        pixelPosition.y = Mathf.RoundToInt(0.5f * pixelHeight + dy * (pixelHeight / worldHeight));
        // Debug.Log(pixelPosition);

        return pixelPosition;
    }
}
