using System;
using UnityEngine;

public class Gem : MonoBehaviour, IItem
{
    public static event Action<int> OnGemCollect;
    public int worth = 0;

   
    public void Collect()
    {
        OnGemCollect.Invoke(worth);
        SoundEffectManager.Play("Gem");
        Destroy(gameObject);
    }

}