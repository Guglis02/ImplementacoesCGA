using UnityEngine;

public class RippleTest : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material material;
    [SerializeField] private Camera camera;

    private Vector3 playerPos;
    private RenderParams renderParams;

    private void Start()
    {
        playerPos = GameManager.PlayerPosition;
        camera.transform.position = GameManager.Instance.LevelCenter;
        camera.orthographicSize = GameManager.Instance.LevelSize.x;

        renderParams = new RenderParams();
        renderParams.material = material;
        renderParams.layer = LayerMask.NameToLayer("WaterSimulation");
        renderParams.camera = camera;
    }

    private void Update()
    {
        material.SetColor(0, Color.red);

        Graphics.RenderMesh(renderParams, mesh);
    }
}
