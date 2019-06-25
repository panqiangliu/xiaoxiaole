using System.Collections;
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
    public Dictionary<SweetType, GameObject> sweetPrefabDic;
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
    //单例
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

    public float fillTime;

    void awake()
    {
        _instance = this;
    }

    public void Start()
    {
        sweetPrefabDic = new Dictionary<SweetType, GameObject>();
        for (int i = 0; i < sweerPrefabs.Length; i++)
        {
            if (!sweetPrefabDic.ContainsKey(sweerPrefabs[i].type))
            {
                sweetPrefabDic.Add(sweerPrefabs[i].type, sweerPrefabs[i].prefab);
            }
        }


        for (int x = 0; x < xColum; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                GameObject go = Instantiate(gridPrefab, CorrectPos(x, y), Quaternion.identity);
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

        StartCoroutine(AllFill());
    }
    public Vector3 CorrectPos(int x, int y)
    {
        //实际需要实例化巧克力块的X位置=GameManager位置的X坐标-大网格长度的一半+行列对应的X坐标
        //实际需要实例化巧克力块的Y位置=GameManager位置的Y坐标+大网格高度的一半-行列对应的Y坐标
        return new Vector3(transform.position.x - xColum / 2f + x, transform.position.y + yRow / 2f - y);
    }

    public GameSweet CreateNewSweet(int x, int y, SweetType type)
    {
        GameObject newSweet = Instantiate(sweetPrefabDic[type], CorrectPos(x, y), Quaternion.identity);
        newSweet.transform.SetParent(transform);

        sweets[x, y] = newSweet.GetComponent<GameSweet>();
        sweets[x, y].Init(x, y, this, type);

        return sweets[x, y];
    }

    //全部填充的方法
    public IEnumerator AllFill()
    {
        bool needRefill = true;
        while (needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while(Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //清除所有我们已经匹配好的甜品
        
        }
    }

    /// <summary>
    /// 分步填充
    /// </summary>
    public bool Fill()
    {
        bool filledNotFinished = false;    //判断本次的填充是否完成；

        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColum; x++)
            {
                GameSweet sweet = sweets[x, y];      //得到当前的元素的位置
                if (sweet.CanMove())      //如果无法移动，则无法向下填充
                {
                    GameSweet sweetBelow = sweets[x, y + 1];

                    if (sweetBelow.Type == SweetType.EMPTY)  //垂直填充
                    {
                        sweet.MovedComponent.Move(x, y + 1,fillTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetType.EMPTY);
                        filledNotFinished = true;
                    }
                }
            }
            //最上排 特殊情况
            for (int x = 0; x < xColum; x++)
            {
                GameSweet sweet = sweets[x, 0];
                if (sweet.Type == SweetType.EMPTY)
                {
                    GameObject newSweet = Instantiate(sweetPrefabDic[SweetType.NORMAL], CorrectPos(x, -1), Quaternion.identity);
                    newSweet.transform.parent = transform;

                    sweets[x, 0] = newSweet.GetComponent<GameSweet>();
                    sweets[x, 0].Init(x, -1, this, SweetType.NORMAL);
                    sweets[x, 0].MovedComponent.Move(x, 0,fillTime);
                    sweets[x, 0].ColoredComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[x, 0].ColoredComponent.NumColors));
                    filledNotFinished = true;
                }
            }
        }
        return filledNotFinished;
    }
}
