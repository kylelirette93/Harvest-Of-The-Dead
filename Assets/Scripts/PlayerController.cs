using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // References.
    Rigidbody2D rb;

    // Variables.
    public float moveSpeed = 5f;
    Vector2 moveDirection;
    Vector2 mousePosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get player input.
        float horizontalMovement = Input.GetAxisRaw("Horizontal");
        float verticalMovement = Input.GetAxisRaw("Vertical");

        // Fire weapon on mouse click.
        if (Input.GetMouseButtonDown(0))
        {
            WeaponManager.instance.FireWeapon();
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
}
