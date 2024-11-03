using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
    int bulletDamage = 5;
    void OnCollisionEnter2D(Collision2D collision)
    {
        Actor actor = collision.gameObject.GetComponent<Actor>();
        if (actor != null)
        {
            actor.healthSystem.TakeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }
}
