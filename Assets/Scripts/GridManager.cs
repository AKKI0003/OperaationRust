using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Grid")]
    public Vector2 gridWorldSize = new Vector2(22, 16);

    public float nodeRadius = 0.15f;

    [Header("Agent")]
    public float agentRadius = 0.35f;

    public LayerMask obstacleMask;

    Node[,] grid;

    float nodeDiameter;

    int gridSizeX;
    int gridSizeY;

    void Awake()
    {
        Instance = this;
        GenerateGrid();
    }

    void OnValidate()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        nodeDiameter = nodeRadius * 2;

        gridSizeX =
            Mathf.RoundToInt(
                gridWorldSize.x / nodeDiameter);

        gridSizeY =
            Mathf.RoundToInt(
                gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        Vector2 worldBottomLeft =
            (Vector2)transform.position -
            new Vector2(
                gridWorldSize.x / 2f,
                gridWorldSize.y / 2f);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint =
                    worldBottomLeft +
                    Vector2.right *
                    (x * nodeDiameter + nodeRadius) +
                    Vector2.up *
                    (y * nodeDiameter + nodeRadius);

                bool walkable =
                    !Physics2D.OverlapCircle(
                        worldPoint,
                        agentRadius,
                        obstacleMask);

                grid[x, y] =
                    new Node(
                        walkable,
                        worldPoint,
                        x,
                        y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX =
            (worldPosition.x -
             (transform.position.x -
              gridWorldSize.x / 2f))
            / gridWorldSize.x;

        float percentY =
            (worldPosition.y -
             (transform.position.y -
              gridWorldSize.y / 2f))
            / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x =
            Mathf.RoundToInt(
                (gridSizeX - 1) * percentX);

        int y =
            Mathf.RoundToInt(
                (gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours =
            new List<Node>();

        Vector2Int[] dirs =
        {
            new Vector2Int(0,1),
            new Vector2Int(0,-1),
            new Vector2Int(1,0),
            new Vector2Int(-1,0)
        };

        foreach (Vector2Int dir in dirs)
        {
            int checkX =
                node.gridX + dir.x;

            int checkY =
                node.gridY + dir.y;

            if (
                checkX >= 0 &&
                checkX < gridSizeX &&
                checkY >= 0 &&
                checkY < gridSizeY
            )
            {
                neighbours.Add(
                    grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        foreach (Node node in grid)
        {
            Gizmos.color =
                node.walkable
                ? Color.green
                : Color.red;

            Gizmos.DrawCube(
                node.worldPosition,
                Vector3.one * 0.08f);
        }
    }
}