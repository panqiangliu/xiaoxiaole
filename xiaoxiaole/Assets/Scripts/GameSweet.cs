using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSweet : MonoBehaviour
{
    public int x;
    public int y;
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
    private ColorSweet coloredComponent;

    public ColorSweet ColoredComponent
    {
        get
        {
            return coloredComponent;
        }
    }
    
    private ClearedSweet clearedComponent;

    public ClearedSweet ClearedComponent
    {
        get
        {
            return clearedComponent;
        }
    }

    public bool CanMove()
    {
        return movedComponent != null;
    }
    public bool CanColor()
    {
        return coloredComponent != null;
    }

    public bool CanClear()
    {
        return clearedComponent != null;
    }

    private void Awake()
    {
        movedComponent = GetComponent<MovedSweet>();
        coloredComponent = GetComponent<ColorSweet>();
        clearedComponent = GetComponent<ClearedSweet>();
    }
    public void Init(int _x, int _y, GameManager _gamemanager, GameManager.SweetType _type)
    {
        x = _x;
        y = _y;
        gameManager = _gamemanager;
        type = _type;
    }

    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }

    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }
}
