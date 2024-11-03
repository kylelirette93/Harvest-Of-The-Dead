using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    public int currentHealth;
    const int maxHealth = 100;

    public void ResetGame()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg; 
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

}
