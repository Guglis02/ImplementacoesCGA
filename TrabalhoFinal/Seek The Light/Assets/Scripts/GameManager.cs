using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelBuilder m_levelBuilder;
    [SerializeField] private Object m_mainMenuScene;
    [SerializeField] private Object m_nextScene;
    [SerializeField] private GameObject m_playerCamera;
    [SerializeField] private GameObject m_topCamera;

    public static GameManager Instance { get; private set; }

    public Grid<LevelBuilder.LevelCell> LevelGrid => m_levelBuilder.levelGrid;
    public Vector3 LevelSize => m_levelBuilder.LevelSize;
    public Vector3 LevelCenter => m_levelBuilder.LevelCenter;

    public int TotalPoints => m_levelBuilder.pointsCounter;

    public enum AiMode
    {
        GridInterpolation,
        SteeringBehaviour
    }

    public AiMode CurrentAiMode = AiMode.GridInterpolation;

    private static Player s_player;
    public static Player Player
    {
        get
        {
            if (s_player == null)
            {
                s_player = FindObjectOfType<Player>();
            }

            return s_player;
        }
    }

    public static Vector3 PlayerPosition => Player.transform.position;

    private void Awake()
    {
        Instance = this;
        m_levelBuilder.BuildLevel();
    }

    private void Start()
    {
        Player.OnPlayerDeath += () => SceneManager.LoadScene(m_mainMenuScene.name, LoadSceneMode.Single);
        Player.OnPlayerGotAllPoints += () => SceneManager.LoadScene(m_nextScene.name, LoadSceneMode.Single);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_playerCamera.SetActive(!m_playerCamera.activeSelf);
            m_topCamera.SetActive(!m_topCamera.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        Shader.SetGlobalVector("_AgentPos", PlayerPosition);
    }
}
