using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelBuilder levelBuilder;

    public static GameManager Instance { get; private set; }

    public Grid<LevelBuilder.LevelCell> LevelGrid => levelBuilder.levelGrid;

    private static Player m_player;
    public static Player Player
    {
        get
        {
            if (m_player == null)
            {
                m_player = FindObjectOfType<Player>();
            }

            return m_player;
        }
    }

    public static Vector3 PlayerPosition => Player.transform.position;

    private void Awake()
    {
        Instance = this;
        levelBuilder.BuildLevel();
    }
}
