using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private Texture2D levelMap;

    [SerializeField] private GameObject floor;

    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private GameObject enemiesParent;
    [SerializeField] private Enemy enemyPrefab;

    public static int blockSize = 4;
    private float halfBlockSize => blockSize * 0.5f;

    private Vector2[,] cellSizes;
    private Color[] pixels;

    private int levelWidth;
    private int levelHeight;

    private List<Vector2> walkableCells = new List<Vector2>();
    public List<Vector2> WalkableCells => walkableCells;
        
    enum LevelElementID
    {
        Wall,
        PowerUp,
        Enemy,
        Player,
        Path
    }

    Dictionary<Color, LevelElementID> levelElementIDByColor = new()
    {
        { Color.black, LevelElementID.Wall },
        { Color.blue, LevelElementID.PowerUp },
        { Color.red, LevelElementID.Enemy },
        { Color.green, LevelElementID.Player },
        { Color.white, LevelElementID.Path }
    };

    private void OnDrawGizmos()
    {
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector3(x * blockSize + halfBlockSize,
                                                0,
                                                y * blockSize + halfBlockSize),
                                    new Vector3(blockSize,
                                                1,
                                                blockSize));
            }
        }
    }

    public void BuildLevel()
    {
        levelHeight = levelMap.height;
        levelWidth = levelMap.width;

        cellSizes = new Vector2[levelWidth, levelHeight];
        for (int i = 0; i < levelWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                cellSizes[i, j] = Vector2.one;
            }
        }

        floor.transform.localScale = new Vector3(levelWidth * blockSize, 0.1f, levelHeight * blockSize);
        floor.transform.position = new Vector3(levelWidth / 2 * blockSize, 0, levelHeight / 2 * blockSize);

        pixels = levelMap.GetPixels();

        GroupHorizontalCells();
        GroupVerticalCells();
        
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                if (cellSizes[x, y] == Vector2.zero)
                    continue;

                Color32 pixelColor = pixels[y * levelWidth + x];
                LevelElementID elementID = levelElementIDByColor[pixelColor];

                switch (elementID)
                {
                    case LevelElementID.Wall:
                        InitializeWall(y, x); 
                        break;
                    case LevelElementID.PowerUp:
                        // Add power-up
                        break;
                    case LevelElementID.Enemy:
                        InitializeEnemy(y, x);
                        walkableCells.Add(new Vector2(x, y));
                        break;
                    case LevelElementID.Player:
                        Vector3 spawnPoint = new Vector3(x * blockSize,
                                                         1,
                                                         y * blockSize);
                        GameManager.Player.SetPosition(spawnPoint);
                        walkableCells.Add(new Vector2(x, y));
                        break;
                    case LevelElementID.Path:
                        walkableCells.Add(new Vector2(x, y));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    private void InitializeEnemy(int y, int x)
    {
        Vector3 spawnPoint = new Vector3(x * blockSize,
                                         0,
                                         y * blockSize);

        Enemy enemy = Instantiate(enemyPrefab,
                                       spawnPoint,
                                       Quaternion.identity);

        enemy.transform.parent = enemiesParent.transform;

        enemy.SetInitialTarget(new Vector2(levelWidth + 1, levelHeight));
    }

    private void InitializeWall(int y, int x)
    {
        GameObject wall = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
        Vector2 size = cellSizes[x, y];

        wall.transform.parent = wallsParent.transform;
        wall.transform.localScale = new Vector3(size.x * blockSize,
                                                1,
                                                size.y * blockSize);
        wall.transform.position = new Vector3((x - size.x * 0.5f) * blockSize + halfBlockSize,
                                              0,
                                              (y - size.y * 0.5f) * blockSize + halfBlockSize);
    }

    private void GroupHorizontalCells()
    {
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 1; x < levelWidth; x++)
            {
                if (levelElementIDByColor[pixels[y * levelWidth + x]] != LevelElementID.Wall
                    || pixels[y * levelWidth + x] != pixels[y * levelWidth + x - 1])
                    continue;
                cellSizes[x, y].x += cellSizes[x - 1, y].x;
                cellSizes[x - 1, y] = Vector2.zero;
            }
        }
    }

    private void GroupVerticalCells()
    {
        for (int x = 1; x < levelWidth; x++)
        {
            for (int y = 1; y < levelHeight; y++)
            {
                if (levelElementIDByColor[pixels[y * levelWidth + x]] != LevelElementID.Wall
                    || cellSizes[x, y - 1] == Vector2.zero
                    || pixels[y * levelWidth + x] != pixels[(y - 1) * levelWidth + x]
                    || cellSizes[x, y - 1].x != cellSizes[x, y].x)
                {
                    continue;
                }

                cellSizes[x, y].y += cellSizes[x, y - 1].y;
                cellSizes[x, y - 1] = Vector2.zero;
            }
        }
    }
}
