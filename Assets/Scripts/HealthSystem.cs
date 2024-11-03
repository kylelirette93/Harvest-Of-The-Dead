using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    public int currentHealth;
    int maxHealth = 50;

    public void ResetGame()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg; 
        Mathf.Clamp(currentHealth, 0, maxHealth);
    }

}
