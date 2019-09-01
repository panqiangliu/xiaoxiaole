using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public int xColum;      //x表达的是行数
    public int yRow;        //y表示的是列数

    public float fillTime;

    //显示时间的text
    public Text timeText;

    public float gameTime = 2;

    private bool gameOver;

    public int playerScore = 0;

    public Text scoreText;

    public float addScoreTime;

    public float currentScore;

    public GameObject overPanel;

    public Text finalScoreText;


    private void Awake()
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

        Destroy(sweets[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetType.BARRIER);

        StartCoroutine(AllFill());
    }

    public void Update()
    {

        gameTime -= Time.deltaTime;
        if (gameTime < 0)
        {
            gameTime = 0;
            //x显示失败面板
            //播放失败面板的动画
            gameOver = true;
            overPanel.SetActive(true);
            finalScoreText.text = playerScore.ToString();
        }
        timeText.text = gameTime.ToString("0");

        if (addScoreTime <= 0.05f)
        {
            addScoreTime += Time.deltaTime;
        }
        else
        {
            if (currentScore < playerScore)
            {
                currentScore++;
                scoreText.text = currentScore.ToString();
                addScoreTime = 0;
            }
        }
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
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //清除所有我们已经匹配好的甜品
            needRefill = ClearAllMatchedSweet();
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
                        Destroy(sweetBelow.gameObject);
                        sweet.MovedComponent.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        CreateNewSweet(x, y, SweetType.EMPTY);
                        filledNotFinished = true;
                    }
                    else        //斜向填充
                    {
                        for (int down = -1; down < 1; down++)
                        {
                            if (down != 0)
                            {
                                int downX = down + x;
                                //该处判断列的位置    0 < downX < xColum
                                //   [0,0]     [1,0]   [2,0] ...[x,0]
                                //   [1,0]     [1,1]   [1,2] ...[1,yRow]
                                //   ...        ...
                                //[xColum,0] [xColum] [xColum]...[xColum]
                                if (downX > 0 && downX < xColum)
                                {
                                    GameSweet downSweet = sweets[downX, y + 1];
                                    if (downSweet.Type == SweetType.EMPTY)
                                    {
                                        bool canFill = true;

                                        //判断数值方向上是否符合填充的要求
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GameSweet sweetAbove = sweets[downX, aboveY];

                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (!sweetAbove.CanMove() && sweetAbove.Type != SweetType.EMPTY)
                                            {
                                                canFill = false;
                                                break;
                                            }
                                        }
                                        if (!canFill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.MovedComponent.Move(downX, y + 1, fillTime);
                                            sweets[downX, y + 1] = sweet;
                                            CreateNewSweet(x, y, SweetType.EMPTY);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
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
                    sweets[x, 0].MovedComponent.Move(x, 0, fillTime);
                    sweets[x, 0].ColoredComponent.SetColor((ColorSweet.ColorType)Random.Range(0, sweets[x, 0].ColoredComponent.NumColors));
                    filledNotFinished = true;
                }
            }
        }
        return filledNotFinished;
    }

    /// <summary>
    /// 判断产品是否相邻的方法
    /// </summary>
    public bool IsFriend(GameSweet sweet1, GameSweet sweet2)
    {
        //有两种情况，X相同，y相差1；y相同，x只相差1
        return ((sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) ||
        (sweet1.Y == sweet2.Y && Mathf.Abs(sweet2.X - sweet1.X) == 1));
    }

    /// <summary>
    /// 交换两个甜品的方法
    /// </summary>
    public void ExchangeSweets(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweets[sweet1.X, sweet1.Y] = sweet2;
            sweets[sweet2.X, sweet2.Y] = sweet1;

            if (MatchSweets(sweet1, sweet2.X, sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null)
            {
                int tempX = sweet1.X;
                int tempy = sweet1.Y;
                sweet1.MovedComponent.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.MovedComponent.Move(tempX, tempy, fillTime);



                ClearAllMatchedSweet();
                StartCoroutine(AllFill());

                pressedSweet = null;
                enteredSweet = null;
            }
            else
            {
                sweets[sweet1.X, sweet1.Y] = sweet1;
                sweets[sweet2.X, sweet2.Y] = sweet2;
            }
        }
    }

    /// <summary>
    /// 玩家对我们甜品操作进行拖拽处理的方法
    /// </summary>
#region 
    /// <summary>
    /// 对甜品的操作方法
    /// </summary>
    public void EnterSweet(GameSweet sweet)
    {
        if (gameOver == false)
        {
            enteredSweet = sweet;
        }
    }

    public void PressSweet(GameSweet sweet)
    {
        if (gameOver == false)
        {
            pressedSweet = sweet;
        }
    }

    public void ReleaseSweet()
    {
        if (gameOver == false)
        {
            if (IsFriend(pressedSweet, enteredSweet))
            {
                ExchangeSweets(pressedSweet, enteredSweet);
            }
        }
    }
    #endregion

    /// <summary>
    /// 清除匹配的方法
    /// </summary>
    #region
    //匹配方法
    public List<GameSweet> MatchSweets(GameSweet sweet, int newX, int newY)
    {
        if (sweet.CanMove())
        {
            ColorSweet.ColorType color = sweet.ColoredComponent.Color;
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();

            //行匹配
            //当x=0代表向左，1代表向右遍历sweet所在的行的物体
            matchRowSweets.Add(sweet);
            for (int i = 0; i <= 1; i++)
            {
                for (int xDistance = 1; xDistance < xColum; xDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newX - xDistance;
                    }
                    else
                    {
                        x = newX + xDistance;
                    }
                    if (x < 0 || x >= xColum)
                    {
                        break;
                    }
                    if (sweets[x, newY].CanColor() && sweets[x, newY].ColoredComponent.Color == color)
                    {
                        matchRowSweets.Add(sweets[x, newY]);
                    }
                    else
                        break;
                }
            }

            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }

            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int yDistance = 1; yDistance < yRow; yDistance++)
                        {
                            int y;
                            if (j == 0)
                            {
                                y = newY - yDistance;
                            }
                            else
                            {
                                y = newY + yDistance;
                            }
                            if (y < 0 || y >= yRow)
                            {
                                break;
                            }

                            if (sweets[matchRowSweets[i].X, y].CanColor() && sweets[matchRowSweets[i].X, y].ColoredComponent.Color == color)
                            {
                                matchLineSweets.Add(sweets[matchRowSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (matchLineSweets.Count < 2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchLineSweets[j]);
                        }
                        break;
                    }
                }
            }

            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
            matchRowSweets.Clear();
            matchLineSweets.Clear();

            matchLineSweets.Add(sweet);
            //列匹配

            //i=0代表往左，i=1代表往右
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        y = newY - yDistance;
                    }
                    else
                    {
                        y = newY + yDistance;
                    }
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }

                    if (sweets[newX, y].CanColor() && sweets[newX, y].ColoredComponent.Color == color)
                    {
                        matchLineSweets.Add(sweets[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchLineSweets[i]);
                }
            }


            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < xColum; xDistance++)
                        {
                            int x;
                            if (j == 0)
                            {
                                x = newY - xDistance;
                            }
                            else
                            {
                                x = newY + xDistance;
                            }
                            if (x < 0 || x >= xColum)
                            {
                                break;
                            }

                            if (sweets[x, matchLineSweets[i].Y].CanColor() && sweets[x, matchLineSweets[i].Y].ColoredComponent.Color == color)
                            {
                                matchRowSweets.Add(sweets[x, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchRowSweets[j]);
                        }
                        break;
                    }
                }
            }


            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }
        Debug.Log("没有可以匹配的糖果");
        return null;
    }

    #endregion

    public bool ClearSweet(int x, int y)
    {
        if (sweets[x, y].CanClear() && !sweets[x, y].ClearedComponent.IsClear)
        {
            sweets[x, y].ClearedComponent.Clear();
            CreateNewSweet(x, y, SweetType.EMPTY);

            ClearBarrier(x, y);
            return true;
        }
        return false;
    }

    //饼干的消除
    public void ClearBarrier(int x, int y)
    {
        for (int friendx = x - 1; friendx <= x + 1; friendx++)
        {
            if (friendx != x && friendx >= 0 && friendx < xColum)
            {
                if (sweets[friendx, y].Type == SweetType.BARRIER && sweets[friendx, y].CanClear())
                {
                    sweets[friendx, y].ClearedComponent.Clear();
                    CreateNewSweet(friendx, y, SweetType.EMPTY);
                }
            }
        }
        for (int friendy = y - 1; friendy <= y + 1; friendy++)
        {
            if (friendy != y && friendy >= 0 && friendy < yRow)
            {
                if (sweets[x, friendy].Type == SweetType.BARRIER && sweets[x, friendy].CanClear())
                {
                    sweets[x, friendy].ClearedComponent.Clear();
                    CreateNewSweet(x, friendy, SweetType.EMPTY);
                }
            }
        }
    }

    //清楚全部完成匹配的方法
    private bool ClearAllMatchedSweet()
    {
        bool needRefill = false;
        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xColum; x++)
            {
                if (sweets[x, y].CanClear())
                {
                    List<GameSweet> matchList = MatchSweets(sweets[x, y], x, y);
                    if (matchList != null)
                    {
                        SweetType specialSweetsType = SweetType.COUNT;
                        GameSweet randomSweet = matchList[Random.Range(0, matchList.Count)];
                        int specialSweetX = randomSweet.X;
                        int specialSweetY = randomSweet.Y;
                        if (matchList.Count == 4)
                            specialSweetsType = (SweetType)Random.Range((int)SweetType.ROW_CLEAR, (int)SweetType.ROW_CLEAR);
                        else if (matchList.Count == 5)
                            specialSweetsType = SweetType.RAINBOWCANDY;

                        for (int i = 0; i < matchList.Count; i++)
                        {
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                        if(specialSweetsType!= SweetType.COUNT)
                        {
                            Destroy(sweets[specialSweetX, specialSweetY]);
                            GameSweet newSweet = CreateNewSweet(specialSweetX, specialSweetY, specialSweetsType);
                            if(specialSweetsType == SweetType.ROW_CLEAR || specialSweetsType == SweetType.COLUMN_CLEAR
                                && newSweet.CanColor() && matchList[0].CanColor())
                            {
                                newSweet.ColoredComponent.SetColor(matchList[0].ColoredComponent.Color);
                            }
                            //加上彩虹糖的特殊类型的产生
                            else if (specialSweetsType == SweetType.RAINBOWCANDY && newSweet.CanColor())
                            {
                                newSweet.ColoredComponent.SetColor(ColorSweet.ColorType.ANY);
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }


    public void ReturnToMain()
    {
        SceneManager.LoadScene(0);
    }

    public void Replay()
    {
        SceneManager.LoadScene(1);
    }
}


