using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLineSweets : ClearedSweet
{
    public bool isRow;

    public override void Clear()
    {
        base.Clear();
        if(isRow)
        {
            sweet.gameManager.ClearRow(sweet.y);
        }
        else
        {
            sweet.gameManager.ClearColumn(sweet.x);
        }
    }

}
