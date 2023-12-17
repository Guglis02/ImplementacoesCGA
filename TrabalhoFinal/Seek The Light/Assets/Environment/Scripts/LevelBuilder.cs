using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{

    [SerializeField] private Texture2D levelMap;

    [SerializeField] private GameObject wallsParent;
    [SerializeField] private GameObject wallPrefab;

    [SerializeField] private TerrainScaler floor;
    [SerializeField] private TerrainScaler terrainPrefab;

    [SerializeField] private GameObject enemiesParent;
    [SerializeField] private List<Enemy> enemiesPrefabs;

    [SerializeField] private GameObject pickupsParent;
    [SerializeField] private GameObject pickupPrefab;
    [SerializeField] private GameObject powerUpPrefab;

    private int enemyCounter = 0;

    public int pointsCounter = 0;

    public static int s_CellSize = 4;

    private LevelCell[,] m_LevelGrid;
    public Grid<LevelCell> levelGrid;

    private Color[] pixels;

    private int levelWidth;
    private int levelHeight;

    public Vector3 LevelSize => levelGrid.IsUnityNull() ? Vector3.one : levelGrid.GridTotalSize();
    public Vector3 LevelCenter => new Vector3(levelWidth / 2 * s_CellSize, 0, levelHeight / 2 * s_CellSize);

    public enum LevelElementID
    {
        Wall,
        PowerUp,
        Enemy,
        Player,
        Path
    }

    public Dictionary<Color, LevelElementID> levelElementIDByColor = new()
    {
        { Color.black, LevelElementID.Wall },
        { Color.blue, LevelElementID.PowerUp },
        { Color.red, LevelElementID.Enemy },
        { Color.green, LevelElementID.Player },
        { Color.white, LevelElementID.Path }
    };

    public struct LevelCell
    {
        public LevelElementID levelElementID;
        public Vector2Int stackCount;
        public bool isWalkable;

        public LevelCell(LevelElementID levelElementID)
        {
            this.levelElementID = levelElementID;
            stackCount = Vector2Int.one;
            isWalkable = levelElementID != LevelElementID.Wall;
        }
    }

    public void BuildLevel()
    {
        levelHeight = levelMap.height;
        levelWidth = levelMap.width;
        pixels = levelMap.GetPixels();

        PopulateLocalGrid();

        levelGrid = new Grid<LevelCell>(m_LevelGrid, LevelCenter, new Vector2(s_CellSize, s_CellSize));

        floor.Initialize(LevelSize, LevelCenter);
        TerrainScaler terrain = Instantiate(terrainPrefab, transform);
        terrain.Initialize(LevelSize, LevelCenter);

        GroupHorizontalCells();
        GroupVerticalCells();

        for (int y = 0; y < levelHeight; y++)
        {
            for (int x = 0; x < levelWidth; x++)
            {
                if (levelGrid[x, y].stackCount == Vector2.zero)
                    continue;

                Color32 pixelColor = pixels[y * levelWidth + x];
                LevelElementID elementID = levelElementIDByColor[pixelColor];

                switch (elementID)
                {
                    case LevelElementID.Wall:
                        InitializeWall(y, x);
                        break;
                    case LevelElementID.PowerUp:
                        InitializeEntity(x, y, powerUpPrefab, pickupsParent);
                        break;
                    case LevelElementID.Enemy:
                        GameObject enemy = enemiesPrefabs[enemyCounter % enemiesPrefabs.Count].gameObject;
                        InitializeEntity(x, y, enemy, enemiesParent);
                        enemyCounter++;
                        break;
                    case LevelElementID.Player:
                        Vector3 spawnPoint = levelGrid.CoordToPosition(x, y);
                        GameManager.Player.SetPosition(spawnPoint);
                        break;
                    case LevelElementID.Path:
                        InitializeEntity(x, y, pickupPrefab, pickupsParent);
                        pointsCounter++;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void PopulateLocalGrid()
    {
        m_LevelGrid = new LevelCell[levelWidth, levelHeight];
        for (int x = 0; x < levelWidth; x++)
        {
            for (int y = 0; y < levelHeight; y++)
            {
                Color32 pixelColor = pixels[y * levelWidth + x];
                m_LevelGrid[x, y] = new LevelCell(levelElementIDByColor[pixelColor]);
            }
        }
    }

    private void InitializeEntity(int x, int y, GameObject prefab, GameObject parent)
    {
        Vector3 spawnPoint = levelGrid.CoordToPosition(x, y);

        GameObject entity = Instantiate(prefab,
                                        spawnPoint,
                                        Quaternion.identity);

        entity.transform.parent = parent.transform;
    }

    private void InitializeWall(int y, int x)
    {
        GameObject wall = Instantiate(wallPrefab, Vector3.zero, Quaternion.identity);
        Vector2Int size = levelGrid[x, y].stackCount;

        wall.transform.parent = wallsParent.transform;
        Vector3 scale = new Vector3(size.x * s_CellSize, 1, size.y * s_CellSize);
        wall.transform.localScale = scale;
        Vector3 position = levelGrid.CoordToPosition(x - size.x / 2f + 0.5f, y - size.y / 2f + 0.5f);
        wall.transform.position = position;
    }

    private void GroupHorizontalCells()
    {
        for (int y = 0; y < levelGrid.Height; y++)
        {
            for (int x = 1; x < levelGrid.Width; x++)
            {
                ref LevelCell current = ref levelGrid[x, y];
                ref LevelCell previous = ref levelGrid[x - 1, y];

                if (current.isWalkable
                    || current.levelElementID != previous.levelElementID)
                    continue;

                current.stackCount.x += previous.stackCount.x;
                previous.stackCount = Vector2Int.zero;
            }
        }
    }

    private void GroupVerticalCells()
    {
        for (int x = 1; x < levelGrid.Width; x++)
        {
            for (int y = 1; y < levelGrid.Height; y++)
            {
                ref LevelCell current = ref levelGrid[x, y];
                ref LevelCell previous = ref levelGrid[x, y - 1];

                if (current.isWalkable
                    || previous.stackCount == Vector2.zero
                    || current.levelElementID != previous.levelElementID
                    || current.stackCount.x != previous.stackCount.x)
                {
                    continue;
                }

                current.stackCount.y += previous.stackCount.y;
                previous.stackCount = Vector2Int.zero;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (levelGrid == null)
            return;

        for (int y = 0; y < levelGrid.Height; y++)
        {
            for (int x = 0; x < levelGrid.Width; x++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(x * s_CellSize + s_CellSize * 0.5f,
                                                0,
                                                y * s_CellSize + s_CellSize * 0.5f),
                                    new Vector3(s_CellSize,
                                                0,
                                                s_CellSize));
            }
        }
    }
}
