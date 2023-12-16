using UnityEngine;

public class Water : MonoBehaviour, ITerrain
{
    public void Initialize(Vector3 levelSize, Vector3 levelCenter)
    {
        transform.localScale = ((ITerrain)this).MultiplyVectors(transform.localScale, levelSize);
        transform.position = new Vector3(levelCenter.x, transform.position.y, levelCenter.z);
    }
}
