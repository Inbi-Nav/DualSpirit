using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy }

    public GameObject[] objectPrefabs;
    public float bigGemProbability = 0.2f;
    public float enemyProbability = 0.1f;
    public int maxObjects = 5;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    [Header("Spawn Area")]
    public Vector2 spawnAreaSize = new Vector2(2987f, 1080f);
    public Vector2 spawnOrigin = new Vector2(0f, 0f); // esquina inferior izquierda del mundo
    private Vector2 spawnAreaCenter;

    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;
    private float minDistanceBetweenGems;

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
        return spawnObjects.Any(checkObj => Vector3.Distance(checkObj.transform.position, positionToCheck) < 1.0f);
    }

    private ObjectType RandomObjectType()
    {
        float randomChoice = Random.value;

        if (randomChoice <= enemyProbability)
        {
            return ObjectType.Enemy;
        }
        else if (randomChoice <= (enemyProbability + bigGemProbability))
        {
            return ObjectType.BigGem;
        }
        else
        {
            return ObjectType.SmallGem;
        }
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;
        int attempts = 0;
        int maxAttempts = 10;

        ObjectType objectType = RandomObjectType();

        while (!validPositionFound && attempts < maxAttempts)
        {
            float randomX = Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2);
            float randomY = Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2);
            Vector3 potentialPosition = new Vector3(randomX, randomY, 0);

            if (!PositionHasObject(potentialPosition) && !TooCloseToOtherGems(potentialPosition, objectType))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            attempts++;
        }

        if (validPositionFound)
        {
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(gameObject);

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectsAfterTime(gameObject, gemLifeTime));
            }
        }
    }

    private bool TooCloseToOtherGems(Vector3 position, ObjectType type)
    {
        if (type == ObjectType.Enemy) return false;

        foreach (var obj in spawnObjects)
        {
            if (obj == null) continue;

            ObjectType objType;
            if (obj.name.Contains("BigGem"))
                objType = ObjectType.BigGem;
            else if (obj.name.Contains("SmallGem"))
                objType = ObjectType.SmallGem;
            else
                continue;

            float distance = Vector3.Distance(obj.transform.position, position);
            if (distance < minDistanceBetweenGems)
                return true;
        }

        return false;
    }

    private IEnumerator DestroyObjectsAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject != null)
        {
            spawnObjects.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 center = spawnOrigin + (spawnAreaSize / 2f);
        Gizmos.DrawWireCube(center, spawnAreaSize);
    }
}
