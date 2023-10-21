// using System.Collections.Generic;
// using Photon.Pun;
// using UnityEngine;

// public class Ground : MonoBehaviourPun
// {
//     public Texture2D mapTexture;
//     public Texture2D newTexture;
//     public SpriteRenderer sr;
//     public PolygonCollider2D polygonCollider;
//     public float worldWidth, worldHeight;
//     public int pixelWidth, pixelHeight;
//     public int minDestroyHeight;
//     public CompositeCollider2D compositeCollider;
//     public Dictionary<int, int> horizontalDivisions;
//     void Start()
//     {
//         sr = GetComponent<SpriteRenderer>();
//         newTexture = new Texture2D(mapTexture.width, mapTexture.height);
//         Color[] colors = mapTexture.GetPixels();
//         newTexture.SetPixels(colors);
//         // newTexture = new Texture2D(100, 20);
//         newTexture.Apply();
//         MakeSprite();

//         worldWidth = sr.bounds.size.x;
//         worldHeight = sr.bounds.size.y;
//         pixelWidth = sr.sprite.texture.width;
//         pixelHeight = sr.sprite.texture.height;
//         CameraController.Instance.mapLeftMax = -worldWidth / 2;
//         CameraController.Instance.mapRightMax = worldWidth / 2;

//         polygonCollider = GetComponent<PolygonCollider2D>();
//         compositeCollider = GetComponent<CompositeCollider2D>();
//     }
//     // [PunRPC]
//     public void MakeHole(int radius, int horizontalDivision, int verticalDivision, int colliderCenterX, int colliderCenterY)
//     {
//         Vector2Int colliderCenter = new(colliderCenterX, colliderCenterY);
//         int px, nx, py, ny, distance;
//         if (colliderCenter.y > minDestroyHeight)
//         {
//             for (int i = 0; i < radius; ++i)
//             {
//                 distance = Mathf.RoundToInt(Mathf.Sqrt(radius * radius - i * i));
//                 for (int j = 0; j < distance; ++j)
//                 {
//                     px = colliderCenter.x + i / horizontalDivision;
//                     nx = colliderCenter.x - i / horizontalDivision;
//                     py = colliderCenter.y + j / verticalDivision;
//                     ny = colliderCenter.y - j / verticalDivision;

//                     if (py > minDestroyHeight)
//                     {
//                         newTexture.SetPixel(px, py, Color.clear);
//                         newTexture.SetPixel(nx, py, Color.clear);
//                     }
//                     if (ny > minDestroyHeight)
//                     {
//                         newTexture.SetPixel(px, ny, Color.clear);
//                         newTexture.SetPixel(nx, ny, Color.clear);
//                     }
//                 }
//             }
//         }
//         newTexture.Apply();
//         MakeSprite();

//         Destroy(polygonCollider);
//         polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
//         polygonCollider.usedByComposite = true;
//         compositeCollider.GenerateGeometry();
//     }
//     void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Explosion"))
//         {
//             CapsuleCollider2D c2d = other.GetComponent<CapsuleCollider2D>();
//             Vector2Int colliderCenter = WorldToPixel(c2d.bounds.center);
//             int radius = Mathf.RoundToInt(c2d.bounds.size.x / 2 * pixelWidth / worldWidth);
//             int horizontalDivision = other.GetComponentInParent<Weapons>().horizontalDivision;
//             int verticalDivision = other.GetComponentInParent<Weapons>().verticalDivision;
//             int colliderCenterX = colliderCenter.x;
//             int colliderCenterY = colliderCenter.y;
//             MakeHole(radius, horizontalDivision, verticalDivision, colliderCenterX, colliderCenterY);
//         }
//     }
//     public void MakeDot(Vector3 pos)
//     {
//         Vector2Int pixelPosition = WorldToPixel(pos);

//         newTexture.SetPixel(pixelPosition.x, pixelPosition.y, Color.clear);
//         newTexture.SetPixel(pixelPosition.x + 1, pixelPosition.y, Color.clear);
//         newTexture.SetPixel(pixelPosition.x - 1, pixelPosition.y, Color.clear);
//         newTexture.SetPixel(pixelPosition.x, pixelPosition.y + 1, Color.clear);
//         newTexture.SetPixel(pixelPosition.x, pixelPosition.y - 1, Color.clear);

//         newTexture.Apply();
//         MakeSprite();
//     }
//     void MakeSprite()
//     {
//         sr.sprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), Vector2.one * 0.5f);
//     }
//     public Vector2Int WorldToPixel(Vector3 pos)
//     {
//         Vector2Int pixelPosition = Vector2Int.zero;
//         var dx = pos.x - transform.position.x;
//         var dy = pos.y - transform.position.y;
//         pixelPosition.x = Mathf.RoundToInt(0.5f * pixelWidth + dx * (pixelWidth / worldWidth));
//         pixelPosition.y = Mathf.RoundToInt(0.5f * pixelHeight + dy * (pixelHeight / worldHeight));

//         return pixelPosition;
//     }
// }
