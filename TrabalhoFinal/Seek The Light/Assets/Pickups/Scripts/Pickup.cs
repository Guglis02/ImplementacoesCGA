using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupEffect
    {
        Point,
        PowerUp
    }

    public PickupEffect Effect;
}
