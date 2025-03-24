using System;
using System.Data;
using UnityEngine;

public class HealthItem : MonoBehaviour, IItem
{
    public int healthAmount = 1; 
    public static event Action<int> OnHeatlhCollect;
    public void Collect()
    {
        OnHeatlhCollect.Invoke(healthAmount);
        Destroy(gameObject);

    }
}