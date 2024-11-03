using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Actor
{
    // Variables
    float chaseSpeed = 2f;

    // References
    Rigidbody2D rb;
    Animator animator;
    public ParticleSystem bloodParticles;
    public Transform playerTransform;
    int currentHealth;
    BoxCollider2D zombieCollider;

    // Enum for Zombie State
    private enum ZombieState
    {
        Healthy,
        Injured,
        Dead
    }

    // Current State
    private ZombieState currentState;

    void OnEnable()
    {
        base.Start();
        currentHealth = healthSystem.currentHealth;
        zombieCollider = GetComponent<BoxCollider2D>();
        zombieCollider.enabled = true;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bloodParticles = GetComponent<ParticleSystem>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        UpdateZombieState();  // Initialize the zombie's state
    }

    public override void Update()
    {
        base.Update();

        // Chase the player if not dead
        if (currentState != ZombieState.Dead)
        {
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            Vector2 facingDirection = playerTransform.position - transform.position;
            float facingAngle = Mathf.Atan2(facingDirection.x, -facingDirection.y) * Mathf.Rad2Deg;
            rb.rotation = facingAngle;

            transform.position += directionToPlayer * chaseSpeed * Time.smoothDeltaTime;
        }

        // Update zombie state based on health
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
        SetZombieState(ZombieState.Dead);  // Ensure state is set to Dead
        zombieCollider.enabled = false;
        Invoke("DeactivateZombie", 1f);  // Delay deactivation for animation
    }

    void DeactivateZombie()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            bloodParticles.Play();
            healthSystem.TakeDamage(10);
        }
    }
}