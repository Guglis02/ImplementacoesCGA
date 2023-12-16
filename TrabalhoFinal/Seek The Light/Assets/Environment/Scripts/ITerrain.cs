using UnityEngine;

public interface ITerrain
{
    public void Initialize(Vector3 levelSize, Vector3 levelCenter);

    // Metodo utilitario para multiplicacao membro a membro de dois vetores
    Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
    {
        float x = v1.x * v2.x;
        float y = v1.y * v2.y;
        float z = v1.z * v2.z;

        return new Vector3(x, y, z);
    }
}
