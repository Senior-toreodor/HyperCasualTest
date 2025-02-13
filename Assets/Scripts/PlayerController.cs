using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float maxChargeTime = 2f;
    public float minPlayerSize = 0.1f;
    public float maxBulletSize = 1f;
    public float firePointOrbitRadius = 0.8f;
    public float moveSpeed = 3f;
    public float stopRadius = 0.5f; 
    public Transform target;

    public float coneAngle = 45f;
    public float coneLength = 3f;

    private float chargeTime = 0f;
    private GameObject chargingBullet;
    private Vector3 initialPlayerScale;
    private bool canMove = false;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private float fixedY;

    void Start()
    {
        initialPlayerScale = transform.localScale;
        fixedY = transform.position.y;
    }

    void Update()
    {
        UpdateFirePointPosition();

        if (chargingBullet != null)
            chargingBullet.transform.position = firePoint.position;

        if (Input.GetMouseButtonDown(0))
            StartChargingBullet();

        if (Input.GetMouseButton(0))
            ChargeBullet();

        if (Input.GetMouseButtonUp(0))
            ShootBullet();

        CheckForObstacleAhead();

        if (canMove)
            MoveAlongPath();
    }

    void StartChargingBullet()
    {
        chargeTime = 0f;
        chargingBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        chargingBullet.transform.localScale = Vector3.zero;
    }

    void ChargeBullet()
    {
        chargeTime += Time.deltaTime;
        float chargeRatio = Mathf.Clamp(chargeTime / maxChargeTime, 0.05f, maxBulletSize);
        
        if (chargingBullet != null)
            chargingBullet.transform.localScale = Vector3.one * chargeRatio;

        float newPlayerScale = Mathf.Max(transform.localScale.x - chargeRatio * Time.deltaTime, minPlayerSize);
        transform.localScale = Vector3.one * newPlayerScale;
    }

    void ShootBullet()
    {
        if (chargingBullet != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPosition = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(10);
            Vector3 shootDirection = (targetPosition - firePoint.position).normalized;
            shootDirection.y = 0;

            Bullet bulletScript = chargingBullet.GetComponent<Bullet>();
            if (bulletScript != null)
                bulletScript.SetTarget(shootDirection);

            Rigidbody rb = chargingBullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = shootDirection * bulletSpeed;
                rb.AddForce(shootDirection * bulletSpeed, ForceMode.Impulse);
            }

            chargingBullet = null;
        }

        if (transform.localScale.x <= minPlayerSize)
        {
            ScoreManager.DecreaseScore(1);
            GameOver();
        }
    }

    void UpdateFirePointPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 direction = new Vector3((worldMousePos - transform.position).x, 0, (worldMousePos - transform.position).z).normalized;
        firePoint.position = transform.position + direction * firePointOrbitRadius;
    }

    void GameOver()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void SetPath(List<GameObject> pathTiles)
    {
        pathPoints.Clear();
        foreach (GameObject tile in pathTiles)
        {
            Vector3 pathPoint = tile.transform.position;
            pathPoint.y = fixedY;
            pathPoints.Enqueue(pathPoint);
        }
    }

    void CheckForObstacleAhead()
    {
        Vector3 forwardDirection = transform.forward;
        Vector3 coneOrigin = transform.position;
        float halfAngle = coneAngle / 2f;

        Collider[] hitColliders = Physics.OverlapSphere(coneOrigin + forwardDirection * (coneLength / 2f), coneLength / 2f);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Obstacle"))
            {
                Vector3 directionToObstacle = hit.transform.position - transform.position;
                float angle = Vector3.Angle(forwardDirection, directionToObstacle);
                if (angle < halfAngle)
                {
                    canMove = false;
                    return;
                }
            }
        }
        canMove = true;
    }

    public void StartMoving()
    {
        canMove = true;
    }

    void MoveAlongPath()
    {
        if (pathPoints.Count > 0)
        {
            Vector3 nextPoint = pathPoints.Peek();
            nextPoint.y = fixedY;

            transform.position = Vector3.MoveTowards(transform.position, nextPoint, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
                pathPoints.Dequeue();
        }
    }
}
