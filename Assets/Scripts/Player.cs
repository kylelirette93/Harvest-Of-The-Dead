using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Actor
{
    // References.
    Rigidbody2D rb;

    // Variables.
    public float moveSpeed = 5f;
    float timeSinceLastShot = 0;
    float fireCooldown;
    Vector2 moveDirection;
    Vector2 mousePosition;
    int currentHealth;

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        fireCooldown = Weapon.instance.fireSpeed;
        currentHealth = healthSystem.currentHealth;
    }

    public override void Update()
    {
        base.Update();
        timeSinceLastShot += Time.deltaTime;
        // Get player input.
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Fire weapon on mouse click.
        if (Input.GetMouseButtonDown(0) && timeSinceLastShot >= fireCooldown)
        {
            Weapon.instance.Shoot();
            timeSinceLastShot = 0f;
        }

        // Direction is based on input.
        moveDirection = new Vector2(horizontalMovement, verticalMovement).normalized;

        // Get current mouse position and convert to world space.
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        // Move the player using direction.
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);

        // Aim direction is from the mouse's position to the player.
        Vector2 aimDirection = mousePosition - rb.position;

        // Calculate angle between player and mouse position.
        float aimAngle = Mathf.Atan2(aimDirection.x, -aimDirection.y) * Mathf.Rad2Deg;

        // Apply angle to rotate rigidbody.
        rb.rotation = aimAngle;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            healthSystem.TakeDamage(10);
        }
    }

    public override void Die()
    {
        gameObject.SetActive(false);
    }
}
