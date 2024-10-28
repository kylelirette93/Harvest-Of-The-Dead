using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ZombieController : MonoBehaviour
{
    // Variables.
    float chaseSpeed = 2f;

    // References.
    Rigidbody2D rb;
    public ParticleSystem bloodParticles;
    public Transform playerTransform;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bloodParticles = GetComponent<ParticleSystem>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Update()
    { 
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector2 facingDirection = playerTransform.position - transform.position;
        float facingAngle = Mathf.Atan2(playerTransform.position.x, -playerTransform.position.y) * Mathf.Rad2Deg;
        rb.rotation = facingAngle;


        transform.position += directionToPlayer * chaseSpeed * Time.smoothDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            bloodParticles.Play();
        }
    }
}
