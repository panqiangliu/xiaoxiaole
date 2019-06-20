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
            if (CanMove())
            {
                x = value;
            }
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
            if (CanMove())
            {
                y = value;
            }
        }
    }
    private GameManager.SweetType type;
    public GameManager.SweetType Type
    {
        get
        {
            return type;
        }
    }

    [HideInInspector]
    public GameManager gameManager;
    private MovedSweet movedComponent;
    public MovedSweet MovedComponent
    {
        get
        {
            return movedComponent;
        }
    }

    public bool CanMove()
    {
        return movedComponent != null;
    }


    public void Init(int _x, int _y, GameManager _gamemanager, GameManager.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gamemanager;
        type = _type;
    }
}
