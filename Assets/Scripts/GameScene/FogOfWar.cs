using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{
    private Texture2D fogOfWarTex;
    private int fogOfWarSizeWidth;
    private int fogOfWarSizeHeight;
    private int radius;
    public SpriteRenderer _fogOfWarSpriteRenderer;
    public Color _fogOfWarColor;

    private void Start()
    {
        fogOfWarSizeWidth = 1000;
        fogOfWarSizeHeight = 1000;
        radius = 100;
        fogOfWarTex = new Texture2D(fogOfWarSizeWidth, fogOfWarSizeHeight);
        CreateFogOfWar();
        DrawCircle(fogOfWarTex, 0, 0, radius, Color.clear);
    }
    public void CreateFogOfWar()
    {
        for (int x = 0; x < fogOfWarSizeWidth; x++)
        {
            for (int y = 0; y < fogOfWarSizeHeight; y++)
            {
                fogOfWarTex.SetPixel(x, y, _fogOfWarColor);
            }
        }
        fogOfWarTex.filterMode = FilterMode.Point;
        fogOfWarTex.Apply();
        _fogOfWarSpriteRenderer.sprite = Sprite.Create(fogOfWarTex,
                                             new Rect(0.0f, 0.0f, fogOfWarTex.width, fogOfWarTex.height),
                                             new Vector2(0.5f, 0.5f),
                                             100.0f);

        //  Right

        GameObject g = new GameObject();
        g.name = "RightBorder";
        g.transform.localPosition = new Vector3(155,0,0);
        g.transform.localScale = Vector3.one * fogOfWarSizeWidth;
        g.transform.SetParent(_fogOfWarSpriteRenderer.transform);

        SpriteRenderer rend = g.AddComponent<SpriteRenderer>();
        rend.sprite = Resources.Load("Art/Blank Square", typeof (Sprite)) as Sprite;
        rend.color = _fogOfWarColor;
        rend.sortingLayerName = "FogOfWar";

        //  Left

        g = new GameObject();
        g.name = "LeftBorder";
        g.transform.localPosition = new Vector3(-155, 0, 0);
        g.transform.localScale = Vector3.one * fogOfWarSizeWidth;
        g.transform.SetParent(_fogOfWarSpriteRenderer.transform);

        rend = g.AddComponent<SpriteRenderer>();
        rend.sprite = Resources.Load("Art/Blank Square", typeof(Sprite)) as Sprite;
        rend.color = _fogOfWarColor;
        rend.sortingLayerName = "FogOfWar";

        //  Up

        g = new GameObject();
        g.name = "UpBorder";
        g.transform.localPosition = new Vector3(0, 155, 0);
        g.transform.localScale = Vector3.one * fogOfWarSizeWidth;
        g.transform.SetParent(_fogOfWarSpriteRenderer.transform);

        rend = g.AddComponent<SpriteRenderer>();
        rend.sprite = Resources.Load("Art/Blank Square", typeof(Sprite)) as Sprite;
        rend.color = _fogOfWarColor;
        rend.sortingLayerName = "FogOfWar";

        //  Down

        g = new GameObject();
        g.name = "DownBorder";
        g.transform.localPosition = new Vector3(0, -155, 0);
        g.transform.localScale = Vector3.one * fogOfWarSizeWidth;
        g.transform.SetParent(_fogOfWarSpriteRenderer.transform);

        rend = g.AddComponent<SpriteRenderer>();
        rend.sprite = Resources.Load("Art/Blank Square", typeof(Sprite)) as Sprite;
        rend.color = _fogOfWarColor;
        rend.sortingLayerName = "FogOfWar";

        _fogOfWarSpriteRenderer.transform.localScale = new Vector2(25, 25);
    }
    private void DrawCircle(Texture2D tex, int x, int y, int radius, Color color)
    {
        float rSquared = radius * radius;
        x += tex.width / 2;
        y += tex.height / 2;

        for (int u = x - radius; u < x + radius + 1; u++)
            for (int v = y - radius; v < y + radius + 1; v++)
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                    tex.SetPixel(u, v, color);

        tex.Apply();
    }
}
