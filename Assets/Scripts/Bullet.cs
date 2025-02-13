using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public float explosionForce = 500f;
    public float explosionRadiusMultiplier = 2f;
    public GameObject explosionEffectPrefab;

    private Vector3 targetDirection;
    private Rigidbody rb;
    private bool hasExploded = false;

    public void SetTarget(Vector3 direction)
    {
        targetDirection = direction == Vector3.zero ? transform.forward : direction.normalized;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
        if (rb == null) return;

        rb.linearVelocity = targetDirection * bulletSpeed;
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = targetDirection * bulletSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {   
        if (collision.gameObject.CompareTag("Player"))
            return;

        if (!hasExploded)
        {
            Explode();
        }

        Destroy(gameObject);
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        float bulletSize = transform.localScale.x;
        float explosionRadius = bulletSize * explosionRadiusMultiplier;

        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * bulletSize; 
            Destroy(explosion, 2f);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        HashSet<Obstacle> damagedObstacles = new HashSet<Obstacle>();

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody objRb = nearbyObject.GetComponent<Rigidbody>();
            if (objRb != null)
            {
                objRb.AddExplosionForce(explosionForce * bulletSize, transform.position, explosionRadius);
            }

            Obstacle obstacle = nearbyObject.GetComponent<Obstacle>();
            if (obstacle != null && !damagedObstacles.Contains(obstacle))
            {
                obstacle.TakeDamage();
                damagedObstacles.Add(obstacle);
            }
        }
    }
}
