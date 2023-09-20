using System.Collections;
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

        // Debug.Log(worldWidth + "," + worldHeight);
        // Debug.Log(pixelWidth + "," + pixelHeight);

        polygonCollider = GetComponent<PolygonCollider2D>();
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
        Debug.Log(dx + "," + dy);
        pixelPosition.x = Mathf.RoundToInt(0.5f * pixelWidth + dx * (pixelWidth / worldWidth));
        pixelPosition.y = Mathf.RoundToInt(0.5f * pixelHeight + dy * (pixelHeight / worldHeight));
        Debug.Log(pixelPosition);

        return pixelPosition;
    }
}
