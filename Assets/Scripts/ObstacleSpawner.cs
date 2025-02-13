using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    public int obstacleCount = 20;
    public Vector3 spawnAreaMin = new Vector3(-8, 0, -8);
    public Vector3 spawnAreaMax = new Vector3(8, 0, 8);
    public Transform playerTransform;
    public float minDistanceFromPlayer = 3f;

    void Start()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        Vector3 randomPosition;
        int maxAttempts = 10;

        do
        {
            randomPosition = new Vector3(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                0, 
                Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            );
            maxAttempts--;
        }
        while (Vector3.Distance(randomPosition, playerTransform.position) < minDistanceFromPlayer && maxAttempts > 0);

        GameObject selectedObstacle = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
        Instantiate(selectedObstacle, randomPosition, Quaternion.identity);
    }
}
