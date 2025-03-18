using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
  public HealthUI healthUI;
private int currentHealth;
private SpriteRenderer spriteRenderer;

public static event Action OnPlayedDied;

// Start is called before the first frame update
void Start()
{
    ResetHealth();
    spriteRenderer = GetComponent<SpriteRenderer>();
    GameController.OnReset += ResetHealth;
}

private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemy = collision.GetComponent<Enemy>();
    if (enemy)
    {
        TakeDamage(enemy.damage);
    }
}
    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
{
    currentHealth -= damage;
    healthUI.UpdateHearts(currentHealth);
    StartCoroutine(FlashRed());

    if (currentHealth <= 0)
    {
        // player dead! -- call game over, animation, etc
        OnPlayedDied.Invoke();
    }
}

private IEnumerator FlashRed()
{
    spriteRenderer.color = Color.red;
    yield return new WaitForSeconds(0.2f);
    spriteRenderer.color = Color.white;
}
}