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
    [SerializeField] private GameObject enemyPrefab;

    private Vector2[,] cellSizes;
    private Color[] pixels;

    private int levelWidth;
    private int levelHeight;
    
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

        floor.transform.localScale = new Vector3(levelWidth, 0.1f, levelHeight);
        floor.transform.position = new Vector3(levelWidth / 2, 0, levelHeight / 2);

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
                        GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        Vector2 size = cellSizes[x, y];
                        wall.transform.parent = wallsParent.transform;
                        wall.transform.localScale = new Vector3(size.x, 1, size.y);
                        wall.transform.position = new Vector3(x - size.x * 0.5f, 0, y - size.y * 0.5f);
                        break;
                    case LevelElementID.PowerUp:
                        // Add power-up
                        break;
                    case LevelElementID.Enemy:
                        GameObject enemy = Instantiate(enemyPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        enemy.transform.parent = enemiesParent.transform;
                        break;
                    case LevelElementID.Player:
                        GameManager.Player.SetPosition(new Vector3(x, 1, y));
                        break;
                    case LevelElementID.Path:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    private void GroupHorizontalCells()
    {
        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 1; x < levelWidth; x++)
            {
                if (pixels[y * levelWidth + x] != pixels[y * levelWidth + x - 1])
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
                if (cellSizes[x, y - 1] == Vector2.zero
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
