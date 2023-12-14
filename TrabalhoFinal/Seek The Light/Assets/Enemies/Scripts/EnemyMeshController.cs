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

    private void OnValidate()
    {
        m_HeadMeshRenderer.materials = m_HeadMaterials;
        m_BodyMeshRenderer.materials = m_BodyMaterials;
    }

    private void Start()
    {
        m_HeadMeshFilter.mesh = m_HeadMesh;        
    }
}
