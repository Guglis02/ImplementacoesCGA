using UnityEngine;

public class Floor : MonoBehaviour, ITerrain
{
    public void Initialize(Vector3 levelSize, Vector3 levelCenter)
    {
        transform.localScale = ((ITerrain)this).MultiplyVectors(transform.localScale, levelSize);
        transform.position = levelCenter;
    }
}