using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Solo debe tener Enemy (índice 0)
    public int maxObjects = 5;
    public float spawnInterval = 0.5f;

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(2987f, 1080f);
    public Vector2 spawnOrigin = new Vector2(0f, 0f);
    private Vector2 spawnAreaCenter;

    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        spawnAreaCenter = spawnOrigin + (spawnAreaSize / 2f);
        StartCoroutine(SpawningObjectsIfNeeded());
    }

    void Update()
    {
        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawningObjectsIfNeeded());
        }
    }

    private int ActiveObjectCount()
    {
        spawnObjects.RemoveAll(item => item == null);
        return spawnObjects.Count;
    }

    private IEnumerator SpawningObjectsIfNeeded()
    {
        isSpawning = true;

        while (ActiveObjectCount() < maxObjects)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private bool PositionHasObject(Vector3 positionToCheck)
    {
        return spawnObjects.Exists(obj => obj != null && Vector3.Distance(obj.transform.position, positionToCheck) < 1f);
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;
        int maxAttempts = 10;

        while (!validPositionFound && attempts < maxAttempts)
        {
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
            float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
            Vector3 potentialPosition = new Vector3(randomX, randomY, 0);

            if (!PositionHasObject(potentialPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            attempts++;
        }

        if (validPositionFound)
        {
            GameObject enemy = Instantiate(objectPrefabs[0], spawnPosition, Quaternion.identity);
            spawnObjects.Add(enemy);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 center = spawnOrigin + (spawnAreaSize / 2f);
        Gizmos.DrawWireCube(center, spawnAreaSize);
    }
}
