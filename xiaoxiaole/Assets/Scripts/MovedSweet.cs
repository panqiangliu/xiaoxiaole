using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovedSweet : MonoBehaviour
{
    public GameSweet sweet;
    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    public void Move(int newX, int newY)
    {
        sweet.X = newX;
        sweet.Y = newY;
        sweet.transform.localPosition = sweet.gameManager.CorrectPos(newX, newY);
    }

}
