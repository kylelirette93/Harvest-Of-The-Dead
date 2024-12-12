using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
    int bulletDamage;
    LineRenderer lineRenderer;
    List<Vector3> positions = new List<Vector3>();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 10;

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0f;

        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trailMaterial.color = new Color(1f, 1f, 0f, 0.02f);
        lineRenderer.material = trailMaterial;

        bulletDamage = Weapon.instance.damage;

        positions.Add(transform.position);
    }

    private void FixedUpdate()
    {
        // Initialize the current position to list.
        positions.Insert(0, transform.position);

        if (positions.Count > 10)
        {
            positions.RemoveAt(positions.Count - 1);
        }
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
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
