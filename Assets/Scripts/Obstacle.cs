using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    public float baseWiggleStrength = 40f;
    public float baseWiggleSpeed = 20f;
    public float recoverySpeed = 3f; 
    public Material[] materials;
    public Material playerMaterial;
    public GameObject explosionEffectPrefab;
    public int maxHits = 1;

    private int hitCount = 0; 
    private Renderer obstacleRenderer;
    private Rigidbody rb;
    private bool isDestroyed = false;

    void Start()
    {
        gameObject.tag = "Obstacle";
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.mass = 2f;
            rb.linearDamping = 1.2f;
            rb.angularDamping = 1.2f;
        }

        obstacleRenderer = GetComponent<Renderer>();

        if (materials.Length > 0)
        {
            obstacleRenderer.material = materials[0];
        }
    }

    public void TakeDamage()
    {
        if (isDestroyed) return;

        hitCount++;

        if (hitCount < materials.Length)
        {
            obstacleRenderer.material = materials[hitCount];
        }

        if (hitCount >= maxHits) 
        {
            isDestroyed = true;
            StartCoroutine(StartDestructionSequence());
        }
    }

    private IEnumerator StartDestructionSequence()
    {
        if (playerMaterial != null)
        {
            obstacleRenderer.material = playerMaterial;
        }
        
        yield return new WaitForSeconds(1f);

        Explode();
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null) 
        {
            GameObject explosion = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 2f);
        }

        PathGenerator pathGenerator = Object.FindFirstObjectByType<PathGenerator>(); 
        if (pathGenerator != null)
        {
            pathGenerator.RemoveObstacle(gameObject);
        }

        Destroy(gameObject);
    }
}
