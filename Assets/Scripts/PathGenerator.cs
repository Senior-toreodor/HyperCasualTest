using UnityEngine;
using System.Collections.Generic;

public class PathGenerator : MonoBehaviour
{
    public Transform player;
    public Transform target;
    public GameObject pathPrefab;
    public float pathSpacing = 1f;
    public float groundY = -1f;
    public float overlapHeight = 2f;

    private List<GameObject> pathTiles = new List<GameObject>();
    private HashSet<GameObject> blockedTiles = new HashSet<GameObject>();

    void Start()
    {
        GeneratePath();
    }

    void GeneratePath()
    {
        foreach (GameObject tile in pathTiles)
        {
            Destroy(tile);
        }
        pathTiles.Clear();
        blockedTiles.Clear();

        Vector3 startPos = new Vector3(player.position.x, groundY, player.position.z);
        Vector3 targetPos = new Vector3(target.position.x, groundY, target.position.z);

        Vector3 direction = (targetPos - startPos).normalized;
        direction.y = 0;

        float distance = Vector3.Distance(startPos, targetPos);

        for (float d = 0; d <= distance; d += pathSpacing)
        {
            Vector3 pathPoint = startPos + direction * d;
            pathPoint.y = groundY;

            Quaternion tileRotation = Quaternion.Euler(90, 0, 0);
            GameObject tile = Instantiate(pathPrefab, pathPoint, tileRotation);
            pathTiles.Add(tile);

            Collider[] hitColliders = Physics.OverlapBox(
                pathPoint + Vector3.up * (overlapHeight / 2),
                new Vector3(pathSpacing / 2, overlapHeight / 2, pathSpacing / 2),
                Quaternion.identity
            );

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Obstacle"))
                {
                    blockedTiles.Add(tile);
                    tile.GetComponent<Renderer>().material.color = Color.red;
                }
            }
        }

        player.GetComponent<PlayerController>().SetPath(pathTiles);
    }

    public void RemoveObstacle(GameObject obstacle)
    {
        foreach (GameObject tile in blockedTiles)
        {
            Collider[] hitColliders = Physics.OverlapBox(
                tile.transform.position + Vector3.up * (overlapHeight / 2),
                new Vector3(pathSpacing / 2, overlapHeight / 2, pathSpacing / 2),
                Quaternion.identity
            );

            bool stillBlocked = false;

            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Obstacle"))
                {
                    stillBlocked = true;
                    break;
                }
            }

            if (!stillBlocked)
            {
                blockedTiles.Remove(tile);
                tile.GetComponent<Renderer>().material.color = Color.green;
                break;
            }
        }

        if (blockedTiles.Count == 0)
        {
            player.GetComponent<PlayerController>().StartMoving();
        }
    }
}
