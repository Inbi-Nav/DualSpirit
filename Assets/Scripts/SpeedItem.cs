using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpeedItem : MonoBehaviour, IItem
{
    public float speedBoost = 1.5f;
    public static event Action<float> OnSpeedBoost;
    public void Collect()
    {
        OnSpeedBoost.Invoke(speedBoost);
        Destroy(gameObject);
    }

}
