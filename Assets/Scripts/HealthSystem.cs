using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem
{
    public int currentHealth;
    public int maxHealth;
    public Image healthBar;

    public HealthSystem(int maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
    }

    public void ResetGame()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= (int)dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    public void Heal(int hp)
    {
        currentHealth += hp;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }
}
