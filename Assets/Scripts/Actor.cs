using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Actor : MonoBehaviour
{
    public HealthSystem healthSystem;
    public GameObject healthBarPrefab;
    int lastCheckingHealth = 0;

    public virtual void Start()
    {
        healthSystem = new HealthSystem(100);
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
