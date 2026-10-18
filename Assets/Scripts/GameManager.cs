using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class GameStatus
{
    public static int GameReady = 0;
    public static int GameStarted = 1;
    public static int GameEnded = 2;
}

public class GameManager : MonoBehaviour
{

    [Header("游戏设置")]
    [SerializeField, Label("当前阳光")] public int currentSunlight = 0;
    [SerializeField, Label("可用植物列表")] private GameObject[] plantsList;
    [Header("阳光设置")]
    [SerializeField, Label("初始阳光数量")] private int initialSunlight = 0;
    [SerializeField, Label("阳光掉落间隔-最低值")] private float sunlightInterval_min = 9.5f;
    [SerializeField, Label("阳光掉落间隔-最高值")] private float sunlightInterval_max = 12.24f;
    [SerializeField, Label("阳光水平随机偏移")] private float sunlightOffsetX = 0.3f;
    [SerializeField, Label("阳光垂直随机偏移")] private float sunlightOffsetY = 0.2f;
    [SerializeField, Label("阳光掉落速度")] private float sunlightDownSpeed = 1.0f;


    [Header("预制件")]
    [SerializeField] private GameObject Sunlight;
    [SerializeField, Label("阳光数量文字")] private TMP_Text sunlightText;

    private int gameStatus = GameStatus.GameStarted;
    private LawnGrid lawnGrid;
    private int cardChooserIndex = 1;
    private bool isDraging = false;
    private GameObject DragingObject;
    
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        lawnGrid = GetComponent<LawnGrid>();

        if (lawnGrid == null)
        {
            Debug.LogError("GameManager 没有找到 LawnGrid 组件。", gameObject);
        }


    }

    public void RefreshSunlightText()
    {
        if (sunlightText == null)
            return;

        sunlightText.text = currentSunlight.ToString();
    }

    private void Start()
    {
        this.StartGame();
    }

    public void StartGame()
    {
        StartCoroutine(SunlightCreator());
        currentSunlight = initialSunlight;
        RefreshSunlightText();

        cardChooserIndex = 1;
        this.SortPlantList();

        for(int i = 0; i < plantsList.Length; i++)
        {
            if(plantsList[i] == null) { continue; }
            Plant plant = plantsList[i].GetComponent<Plant>();
            if(plant == null)
            {
                break;
            }
            DrawPlantCard(plant);
        }
    }

    public void BuyPlant(int cardIndex)
    {
        if (plantsList[cardIndex] == null) { return; }
        if (plantsList[cardIndex].GetComponent<Plant>().GetCost() > currentSunlight) { return; }
        // 扣除阳光
        this.AddSunlight(-plantsList[cardIndex].GetComponent<Plant>().GetCost());
        Vector3 mousePosition = Input.mousePosition;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mouseWorldPosition.z = 0;
        GameObject plant = Instantiate(plantsList[cardIndex], mouseWorldPosition, Quaternion.identity);
        plant.GetComponent<Animator>().speed = 0;
        plant.GetComponent<SpriteRenderer>().sortingLayerName = "didntPlanted";
        plant.GetComponent<Plant>().SetTrigger(true);
        DragingObject = plant;
        StartCoroutine(DragingPlant());
    }

    public void PlantDeath(int row, int column)
    {
        lawnGrid.RemovePlant(row, column);
    }

    private IEnumerator DragingPlant()
    {
        while (Input.GetMouseButton(0))
        {
            if (DragingObject == null)
                yield break;

            Vector3 position =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            position.z = 0f;
            DragingObject.transform.position = position;

            yield return null;
        }

        if (DragingObject == null)
            yield break;

        Vector3 mousePosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition);

        mousePosition.z = 0f;

        Plant plant = DragingObject.GetComponent<Plant>();

        bool foundCell = lawnGrid.TryGetCellFromWorldPosition(
            mousePosition,
            out int row,
            out int column,
            out Vector3 cellCenter
        );

        // 不在草坪范围
        if (!foundCell)
        {
            CancelPlant(plant);
            yield break;
        }

        // 格子已经有植物
        if (lawnGrid.IsCellOccupied(row, column))
        {
            Debug.Log($"第 {row + 1} 行，第 {column + 1} 列已经有植物");

            CancelPlant(plant);
            yield break;
        }

        // 尝试占用格子
        if (!lawnGrid.TrySetPlant(row, column, DragingObject))
        {
            CancelPlant(plant);
            yield break;
        }

        DragingObject.transform.position = cellCenter;

        plant.Planted();

        DragingObject = null;
    }

    private void CancelPlant(Plant plant)
    {
        if (plant != null)
        {
            AddSunlight(plant.GetCost());
        }

        Destroy(DragingObject);
        DragingObject = null;
    }

    void DrawPlantCard(Plant plant)
    {
        GameObject chooserBox = GameObject.Find("ChooserBackground/card" + cardChooserIndex);
        Card card = chooserBox.GetComponent<Card>();
        card.card = plant.Card;
        card.cardDisable = plant.CardDisable;
        card.FitSpriteToCollider();
        card.SetCardIndex(cardChooserIndex);
        cardChooserIndex++;
    }

    void SortPlantList()
    {
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < plantsList.Length; readIndex++)
        {
            if (plantsList[readIndex] == null)
                continue;

            plantsList[writeIndex] = plantsList[readIndex];

            if (writeIndex != readIndex)
            {
                plantsList[readIndex] = null;
            }

            writeIndex++;
        }
    }

    

    public void AddSunlight(int count)
    {
        this.currentSunlight += count;
        for(int i = 0; i < plantsList.Length; i++)
        {
            if(plantsList[i] == null) { continue; }
            GameObject.Find("ChooserBackground/card" + (i + 1)).GetComponent<Card>().SetActive(
                currentSunlight >= plantsList[i].GetComponent<Plant>().GetCost());
        }
        RefreshSunlightText();
    }

    IEnumerator SunlightCreator()
    {
        while(gameStatus == GameStatus.GameStarted)
        {
            yield return StartCoroutine(SunlightDown(UnityEngine.Random.Range(sunlightInterval_min, sunlightInterval_max)));
        }
    }

    IEnumerator SunlightDown(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Vector3 boxPosition = lawnGrid.GetCellCenter(
            UnityEngine.Random.Range(0, lawnGrid.Rows),
            UnityEngine.Random.Range(0, lawnGrid.Columns)
        );

        float randomOffsetX =
            UnityEngine.Random.Range(-sunlightOffsetX, sunlightOffsetX);

        float randomOffsetY =
            UnityEngine.Random.Range(-sunlightOffsetY, sunlightOffsetY);

        Vector3 targetPosition = new Vector3(
            boxPosition.x + randomOffsetX,
            boxPosition.y + randomOffsetY,
            boxPosition.z
        );

        GameObject sunlight = Instantiate(
            Sunlight,
            new Vector3(targetPosition.x, 6f, targetPosition.z),
            Quaternion.identity
        );

        while (
            sunlight != null &&
            Vector3.Distance(
                sunlight.transform.position,
                targetPosition
            ) > 0.01f
        )
        {
            sunlight.transform.position = Vector3.MoveTowards(
                sunlight.transform.position,
                targetPosition,
                sunlightDownSpeed * Time.deltaTime
            );

            yield return null;
        }

        // 阳光可能已经被点击销毁
        if (sunlight == null)
            yield break;

        sunlight.transform.position = targetPosition;

        Sunlight sunlightScript =
            sunlight.GetComponent<Sunlight>();

        if (sunlightScript != null)
        {
            sunlightScript.AllowDestroy(true);
        }
    }

}
