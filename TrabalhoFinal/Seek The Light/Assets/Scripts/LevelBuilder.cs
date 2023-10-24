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
    [SerializeField] private Material wallMaterial;

    [SerializeField] private GameObject enemiesParent;

    [SerializeField] private GameObject enemyPrefab;
    
    enum LevelElementID
    {
        Wall,
        PowerUp,
        Enemy,
        Player
    }

    Dictionary<Color, LevelElementID> levelElementIDByColor = new()
    {
        { Color.black, LevelElementID.Wall },
        { Color.blue, LevelElementID.PowerUp },
        { Color.red, LevelElementID.Enemy },
        { Color.green, LevelElementID.Player }
    };

    public void BuildLevel()
    {
        int height = levelMap.height;
        int width = levelMap.width;

        floor.transform.localScale = new Vector3(width, 0.1f, height);
        floor.transform.position = new Vector3(width / 2, 0, height / 2);

        Color[] pixels = levelMap.GetPixels();

        bool[,] visited = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (visited[x, y])
                {
                    continue;
                }

                Color32 pixelColor = pixels[y * width + x];
                LevelElementID elementID = levelElementIDByColor[pixelColor];

                switch (elementID)
                {
                    case LevelElementID.Wall:
                        CreateCustomWallMesh(x, y, visited);
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
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }

    private void CreateCustomWallMesh(int startX, int startY, bool[,] visited)
    {


        for (int u = startX; u < levelMap.width; u++)
        {
            for (int v = startY; v < levelMap.height; v++)
            {
                
            }
        }
    }
}
