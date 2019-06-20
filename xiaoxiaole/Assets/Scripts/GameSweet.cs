using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
    private int x;
    private int y;

    public int X
    {
        get
        {
            return x;
        }

        set
        {
            x = value;
        }
    }

    public int Y
    {
        get
        {
            return y;
        }

        set
        {
            y = value;
        }
    }


    public GameManager.SweetType Type
    {
        get
        {
            return type;
        }
    }

    private GameManager.SweetType type;

    [HideInInspector]
    public GameManager gameManager;

    public void Init(int _x, int _y, GameManager _gamemanager, GameManager.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gamemanager;
        type = _type;
    }
}
