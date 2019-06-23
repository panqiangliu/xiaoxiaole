using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSweet : MonoBehaviour
{
    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    }

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    }

    public ColorSprite[] ColorSprites;

    private Dictionary<ColorType, Sprite> colorSpriteDic;

    private SpriteRenderer sprite;

    public int NumColors
    {
        get { return ColorSprites.Length; }
    }

    public ColorType Color
    {
        get
        {
            return color;
        }

        set
        {
            SetColor(value);
        }
    }
    private ColorType color;


    private void Awake()
    {
        sprite = transform.Find("Sweet").GetComponent<SpriteRenderer>();
        colorSpriteDic = new Dictionary<ColorType, Sprite>();

        for (int i = 0; i < ColorSprites.Length; i++)
        {
            if (!colorSpriteDic.ContainsKey(ColorSprites[i].color))
            {
                colorSpriteDic.Add(ColorSprites[i].color, ColorSprites[i].sprite);
            }
        }
    }
    public void SetColor(ColorType newColor)
    {
        color = newColor;
        if (colorSpriteDic.ContainsKey(newColor))
        {
            sprite.sprite = colorSpriteDic[newColor];
        }
    }

}
