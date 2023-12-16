using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelBuilder levelBuilder;
    [SerializeField] private Object mainMenuScene;
    [SerializeField] private Object m_NextScene;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject topCamera;

    public static GameManager Instance { get; private set; }

    public Grid<LevelBuilder.LevelCell> LevelGrid => levelBuilder.levelGrid;
    public Vector3 LevelSize => levelBuilder.LevelSize;
    public Vector3 LevelCenter => levelBuilder.LevelCenter;

    public int TotalPoints => levelBuilder.pointsCounter;

    public enum AiMode
    {
        GridInterpolation,
        SteeringBehaviour
    }

    public AiMode aiMode = AiMode.GridInterpolation;

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

    private void Start()
    {
        Player.OnPlayerDeath += () => SceneManager.LoadScene(mainMenuScene.name, LoadSceneMode.Single);
        Player.OnPlayerGotAllPoints += () => SceneManager.LoadScene(m_NextScene.name, LoadSceneMode.Single);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            playerCamera.SetActive(!playerCamera.activeSelf);
            topCamera.SetActive(!topCamera.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Teste();
        }

        Shader.SetGlobalVector("_AgentPos", PlayerPosition);
    }

    public void Teste()
    {
        Debug.Log("Teste");
        Player.OnPlayerHit?.Invoke(3);
    }
}
