using UnityEngine;

public class TerrainScaler : MonoBehaviour
{
    public void Initialize(Vector3 levelSize, Vector3 levelCenter)
    {
        transform.localScale = new Vector3(transform.localScale.x * levelSize.x,
                                           transform.localScale.y,
                                           transform.localScale.z * levelSize.z);
        transform.position = new Vector3(levelCenter.x, transform.position.y, levelCenter.z);
    }
}
