﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 甜品的相关成员变量
    /// </summary>
#region
        //甜品种类
    public enum SweetType
    {
        EMPTY,
        NORMAL,
        BARRIER,   //障碍
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOWCANDY,//彩虹糖
        COUNT//标记类型
    }

    //甜品预制体的字典，我们可以通过甜品的种类来得到对应的甜品游戏物体
    public Dictionary<SweetType,GameObject> sweetPrefabDic;
    [System.Serializable]
    public struct SweetPrefab
    {
        public SweetType type;
        public GameObject prefab;
    }
    public SweetPrefab[] sweerPrefabs;

    public GameObject gridPrefab;

    //甜品数组()生成的甜品的是数组
    private GameSweet[,] sweets;

    //要交换的两个甜品对象
    private GameSweet pressedSweet;
    private GameSweet enteredSweet;


#endregion
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    public int xColum;
    public int yRow;


    void awake()
    {
        _instance = this;
    }

    public void Start()
    {
        sweetPrefabDic = new Dictionary<SweetType, GameObject>();
        for(int i=0;i<sweerPrefabs.Length; i++)
        {
            if(!sweetPrefabDic.ContainsKey(sweerPrefabs[i].type))
            {
                sweetPrefabDic.Add(sweerPrefabs[i].type, sweerPrefabs[i].prefab);
            }
        }


        for (int x = 0; x < xColum; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject go = Instantiate(gridPrefab, CorrectPos(x,y), Quaternion.identity);
                go.transform.SetParent(transform);
            }
        }

        sweets = new GameSweet[xColum, yRow];
        for (int x = 0; x < xColum; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                CreateNewSweet(x, y, SweetType.EMPTY);
            }
        }


    }
    public Vector3 CorrectPos(int x, int y)
    {
        //实际需要实例化巧克力块的X位置=GameManager位置的X坐标-大网格长度的一半+行列对应的X坐标
        //实际需要实例化巧克力块的Y位置=GameManager位置的Y坐标+大网格高度的一半-行列对应的Y坐标
        return new Vector3(transform.position.x - xColum / 2f + x, transform.position.y + yRow / 2f - y);
    }

    public void CreateNewSweet(int x,int y,SweetType type)
    {

    }

}