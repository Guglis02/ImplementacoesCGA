using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Point,
        PowerUp
    }

    public PickupType pickupType;
}
