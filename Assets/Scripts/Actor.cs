using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public HealthSystem healthSystem = new HealthSystem();
    int lastCheckingHealth = 0;

    public virtual void Start()
    {
        healthSystem.ResetGame();
    }
    
    public virtual void Die()
    {
        
    }


    public virtual void Update()
    {
        if (healthSystem.currentHealth != lastCheckingHealth)
        {
            lastCheckingHealth = healthSystem.currentHealth;
            if (healthSystem.currentHealth <= 0)
            {
                Die();
            }
        }
    }
}
