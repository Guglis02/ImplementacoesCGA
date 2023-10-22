using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField] private Texture2D levelMap;

    [SerializeField] private GameObject floor;

    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject enemiesParent;

    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject enemyPrefab;

    public void BuildLevel()
    {
        floor.transform.localScale = new Vector3(levelMap.width, 0.1f, levelMap.height);
        floor.transform.position = new Vector3(levelMap.width / 2, 0, levelMap.height / 2);

        Color[] pixels = levelMap.GetPixels();

        for (int y = 0; y < levelMap.height; y++)
        {
            for (int x = 0; x < levelMap.width; x++)
            {
                Color32 pixelColor = pixels[y * levelMap.width + x];
                if (pixelColor == Color.green)
                {
                    GameManager.Player.Controller.enabled = false;
                    GameManager.Player.transform.position = new Vector3(x, 1, y);
                    GameManager.Player.Controller.enabled = true;
                }
                else if (pixelColor == Color.red)
                {
                    GameObject enemy = Instantiate(enemyPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    enemy.transform.parent = enemiesParent.transform;
                }
                else if (pixelColor == Color.blue)
                {
                    // Add power-up
                }
                else if (pixelColor == Color.yellow)
                {
                    // Set end of stage
                }
                else if (pixelColor == Color.black)
                {
                    GameObject wall = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                    wall.transform.parent = wallsParent.transform;
                }
                else if (pixelColor == Color.white)
                {
                    // Add walkable path
                }
            }
        }
    }
}
