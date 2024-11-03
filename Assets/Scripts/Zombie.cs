using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Zombie : Actor
{
    // Variables.
    float chaseSpeed = 2f;

    // References.
    Rigidbody2D rb;
    public ParticleSystem bloodParticles;
    public Transform playerTransform;
    

    public override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        bloodParticles = GetComponent<ParticleSystem>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public override void Update()
    {
        base.Update();
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 facingDirection = playerTransform.position - transform.position;
        float facingAngle = Mathf.Atan2(playerTransform.position.x, -playerTransform.position.y) * Mathf.Rad2Deg;
        rb.rotation = facingAngle;


        transform.position += directionToPlayer * chaseSpeed * Time.smoothDeltaTime;
    }

    public override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            bloodParticles.Play();
        }
    }
}
