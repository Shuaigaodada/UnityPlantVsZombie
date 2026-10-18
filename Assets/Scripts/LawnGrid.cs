using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LawnGrid : MonoBehaviour
{
    [Header("游戏地图")]
    [SerializeField, Label("行数")]
    private int rows = 5;

    [SerializeField, Label("列数")]
    private int columns = 9;
    private BoxCollider2D[,] grid;
    private GameObject[,] plantedObjects;
    public int Rows => rows;
    public int Columns => columns;
    private void Awake()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new BoxCollider2D[rows, columns];
        plantedObjects = new GameObject[rows, columns];

        if (transform.childCount < rows)
        {
            Debug.LogError(
                $"行数不足，需要 {rows} 行，当前只有 {transform.childCount} 行。",
                gameObject
            );

            return;
        }

        for (int row = 0; row < rows; row++)
        {
            Transform line = transform.GetChild(row);

            if (line.childCount < columns)
            {
                Debug.LogError(
                    $"{line.name} 格子数量不足，需要 {columns} 个格子。",
                    line.gameObject
                );

                continue;
            }

            for (int column = 0; column < columns; column++)
            {
                Transform cell = line.GetChild(column);

                BoxCollider2D box =
                    cell.GetComponent<BoxCollider2D>();

                if (box == null)
                {
                    Debug.LogError(
                        $"{cell.name} 没有 BoxCollider2D。",
                        cell.gameObject
                    );

                    continue;
                }

                grid[row, column] = box;
            }
        }
    }
    /// <summary>
    /// 获取指定行列的碰撞箱
    /// </summary>
    public BoxCollider2D GetCell(int row, int column)
    {
        if (!IsValidCell(row, column))
        {
            Debug.LogWarning(
                $"格子超出范围：行 {row}，列 {column}"
            );

            return null;
        }

        return grid[row, column];
    }

    /// <summary>
    /// 获取指定格子的世界坐标中心
    /// </summary>
    public Vector3 GetCellCenter(int row, int column)
    {
        BoxCollider2D cell = GetCell(row, column);

        if (cell == null)
            return Vector3.zero;

        return cell.bounds.center;
    }

    /// <summary>
    /// 判断行列是否有效
    /// </summary>
    public bool IsValidCell(int row, int column)
    {
        return grid != null &&
               row >= 0 &&
               row < rows &&
               column >= 0 &&
               column < columns;
    }

    public bool TryGetCellFromWorldPosition(
    Vector3 worldPosition,
    out int row,
    out int column,
    out Vector3 center)
    {
        row = -1;
        column = -1;
        center = Vector3.zero;

        if (grid == null)
            return false;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                BoxCollider2D cell = grid[r, c];

                if (cell == null)
                    continue;

                if (cell.bounds.Contains(worldPosition))
                {
                    row = r;
                    column = c;
                    center = cell.bounds.center;
                    return true;
                }
            }
        }

        return false;
    }
    public bool IsCellOccupied(int row, int column)
    {
        if (!IsValidCell(row, column))
            return false;

        return plantedObjects[row, column] != null;
    }

    public bool TrySetPlant(int row, int column, GameObject plant)
    {
        if (!IsValidCell(row, column))
            return false;

        if (plantedObjects[row, column] != null)
            return false;

        plantedObjects[row, column] = plant;
        return true;
    }

    public void RemovePlant(int row, int column)
    {
        if (!IsValidCell(row, column))
            return;

        plantedObjects[row, column] = null;
    }

    public GameObject GetPlant(int row, int column)
    {
        if (!IsValidCell(row, column))
            return null;

        return plantedObjects[row, column];
    }
}
