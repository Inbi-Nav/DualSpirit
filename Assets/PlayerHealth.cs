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

void Start()
{
    ResetHealth();
    spriteRenderer = GetComponent<SpriteRenderer>();
    GameController.OnReset += ResetHealth;
    HealthItem.OnHeatlhCollect += Heal;
}

private void OnTriggerEnter2D(Collider2D collision)
{
    Enemy enemy = collision.GetComponent<Enemy>();
    if (enemy)
    {
        TakeDamage(enemy.damage);
    }
}

void Heal(int amount)
{
    currentHealth += amount;
    if (currentHealth > maxHealth)
    {
        currentHealth = maxHealth;
    }
    healthUI.UpdateHearts(currentHealth);
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