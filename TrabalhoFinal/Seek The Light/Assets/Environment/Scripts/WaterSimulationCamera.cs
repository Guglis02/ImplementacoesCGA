using UnityEngine;

public class WaterSimulationCamera : MonoBehaviour
{
    [SerializeField]
    Transform m_WaterInteractor;

    void Awake()
    {
        Camera cam = GetComponent<Camera>();

        Shader.SetGlobalTexture("_GlobalEffectRT", cam.activeTexture);
        Shader.SetGlobalFloat("_OrthographicCamSize", cam.orthographicSize);
    }

    private void Update()
    {
        transform.position = new Vector3(m_WaterInteractor.transform.position.x,
                                         transform.position.y,
                                         m_WaterInteractor.transform.position.z);
        Shader.SetGlobalVector("_Position", transform.position);
    }
}
