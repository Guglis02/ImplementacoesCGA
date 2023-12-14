using UnityEngine;

public class EnemyMeshController : MonoBehaviour
{
    [SerializeField]
    private SkinnedMeshRenderer m_BodyMeshRenderer;

    [SerializeField]
    private Material[] m_BodyMaterials;

    [SerializeField]
    private MeshRenderer m_HeadMeshRenderer;

    [SerializeField]
    private MeshFilter m_HeadMeshFilter;

    [SerializeField]
    private Mesh m_HeadMesh;

    [SerializeField]
    private Material[] m_HeadMaterials;

    [SerializeField]
    private Material m_scorchedMaterial;

    private void OnValidate()
    {
        SetDefaultMaterials();
    }

    private void Start()
    {
        m_HeadMeshFilter.mesh = m_HeadMesh;        
    }

    public void SetScorchedMaterials()
    {
        m_HeadMeshRenderer.material = m_scorchedMaterial;
        m_BodyMeshRenderer.material = m_scorchedMaterial;
    }

    public void SetDefaultMaterials()
    {
        m_HeadMeshRenderer.materials = m_HeadMaterials;
        m_BodyMeshRenderer.materials = m_BodyMaterials;
    }
}
