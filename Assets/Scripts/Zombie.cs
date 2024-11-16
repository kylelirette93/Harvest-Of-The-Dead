using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Zombie : Actor
{
    public float chaseSpeed;
    int mapMinX = -15;
    int mapMaxX = 15;
    int mapMinY = -7;
    int mapMaxY = 7;
    int currentHealth;

    Rigidbody2D rb;
    public Image healthBarFill;
    Animator animator;
    public ParticleSystem bloodParticles;
    public Transform playerTransform;
    BoxCollider2D zombieCollider;
    private DropItem dropItem;
    private GameObject healthBarInstance;

    private enum ZombieState
    {
        Healthy,
        Injured,
        Dead
    }

    private ZombieState currentState;

    void OnEnable()
    {
        base.Start();
        dropItem = GetComponent<DropItem>();
        currentHealth = healthSystem.currentHealth;
        zombieCollider = GetComponent<BoxCollider2D>();
        zombieCollider.enabled = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bloodParticles = GetComponent<ParticleSystem>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        if (healthBarInstance == null)
        {
            Debug.Log("Creating health bar");
            healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            Transform healthBarTransform = healthBarInstance.transform;

            healthBarFill = healthBarInstance.transform.Find("Fill").GetComponent<Image>();

            Canvas healthCanvas = FindObjectOfType<Canvas>();
            if (healthCanvas != null)
            {
                healthBarTransform.SetParent(healthCanvas.transform, false);
            }
            healthBarTransform.localScale = Vector3.one;
        }
        else
        {
            Debug.Log("Reactivating health bar");
            healthBarInstance.SetActive(true);
        }

        UpdateZombieState();
    }

   

    public override void Update()
    {
        base.Update();

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        healthBarFill.transform.position = screenPosition;
        Debug.Log("Health bar fill position: " + healthBarFill.transform.position);
        healthBarFill.fillAmount = healthSystem.currentHealth / (float)healthSystem.maxHealth;

        if (healthSystem.currentHealth <= healthSystem.maxHealth * 0.25f)
        {
            healthBarFill.color = Color.red;
        }
        else if (healthSystem.currentHealth <= healthSystem.maxHealth * 0.5f)
        {
            healthBarFill.color = Color.yellow;
        }
        else
        {
            healthBarFill.color = Color.green;
        }

        if (currentState != ZombieState.Dead)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector2 facingDirection = playerTransform.position - transform.position;
            float facingAngle = Mathf.Atan2(facingDirection.x, -facingDirection.y) * Mathf.Rad2Deg;
            rb.rotation = facingAngle;

            Vector3 separationForce = Vector3.zero;
            float separationRadius = 1.5f;
            Collider2D[] nearbyZombies = Physics2D.OverlapCircleAll(transform.position, separationRadius);
            int nearbyCount = 0;
            foreach (Collider2D zombie in nearbyZombies)
            {
                if (zombie.gameObject != this.gameObject && zombie.CompareTag("Zombie"))
                {
                    Vector3 directionToZombie = (transform.position - zombie.transform.position).normalized;
                    separationForce += directionToZombie;
                    nearbyCount++;
                }
            }

            if (nearbyCount > 0)
            {
                separationForce = (separationForce / nearbyCount).normalized;
            }

            Vector3 finalDirection = (directionToPlayer + separationForce).normalized;
            Vector3 newPosition = transform.position + finalDirection * chaseSpeed * Time.smoothDeltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, mapMinX, mapMaxX);
            newPosition.y = Mathf.Clamp(newPosition.y, mapMinY, mapMaxY);
            transform.position = newPosition;
        }

        if (healthSystem.currentHealth != currentHealth)
        {
            currentHealth = healthSystem.currentHealth;
            UpdateZombieState();
        }
    }

    private void UpdateZombieState()
    {
        if (currentHealth > 60)
        {
            SetZombieState(ZombieState.Healthy);
        }
        else if (currentHealth > 20 && currentHealth <= 60)
        {
            SetZombieState(ZombieState.Injured);
        }
        else if (currentHealth <= 0)
        {
            SetZombieState(ZombieState.Dead);
        }
    }

    private void SetZombieState(ZombieState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        UpdateZombieAnimation();
    }

    private void UpdateZombieAnimation()
    {
        switch (currentState)
        {
            case ZombieState.Healthy:
                animator.Play("ZombieIdle");
                break;
            case ZombieState.Injured:
                animator.Play("ZombieInjured");
                break;
            case ZombieState.Dead:
                animator.Play("ZombieDead");
                break;
        }
    }

    public override void Die()
    {
        base.Die();
        SetZombieState(ZombieState.Dead);
        zombieCollider.enabled = false;
        Invoke("DeactivateZombie", 1f);
        if (dropItem != null)
        {
            GameObject droppedItem = dropItem.GetRandomDrop();
            if (droppedItem != null)
            {
                Instantiate(droppedItem, transform.position, Quaternion.identity);
            }
        }
    }

    public void DeactivateZombie()
    {
        healthBarFill.transform.parent.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            bloodParticles.Play();
        }
    }
}