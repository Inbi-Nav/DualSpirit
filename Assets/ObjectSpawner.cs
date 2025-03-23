using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy }

    public Tilemap tilemap;
    public GameObject[] objectPrefabs;
    public float bigGemProbability = 0.2f;
    public float enemyProbability = 0.1f;
    public int maxObjects = 5;
    public float gemLifeTime = 10f;
    public float spawnInterval = 0.5f;

    private List<Vector3> validSpawnPositions = new List<Vector3>();
    private List<GameObject> spawnObjects = new List<GameObject>();
    private bool isSpawning = false;

    void Start()
    {
        GatherValidPositions();
        StartCoroutine(SpawningObjectsIfNeeded());
        GameController.OnReset += LevelChange;
    }
    void Update()
    {
        if(!tilemap.gameObject.activeInHierarchy)
        {
            LevelChange();
        }
        if (!isSpawning && ActiveObjectCount() < maxObjects)
        {
            StartCoroutine(SpawningObjectsIfNeeded());
        }
    }
   private void LevelChange()
{
    GameObject ground = GameObject.Find("Ground");

    if (ground != null && ground.GetComponent<Tilemap>() != null)
    {
        tilemap = ground.GetComponent<Tilemap>();
        GatherValidPositions();
        DestroyAllSpawnedObjects();
    }
    else
    {
        Debug.LogWarning("No se encontró el GameObject 'Ground' o no tiene Tilemap.");
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
        if (validSpawnPositions.Count == 0) return;

        Vector3 spawnPosition = Vector3.zero;
        bool validPositionFound = false;

        while (!validPositionFound && validSpawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPositions.Count);
            Vector3 potentialPosition = validSpawnPositions[randomIndex];
            Vector3 leftPosition = potentialPosition + Vector3.left;
            Vector3 rightPosition = potentialPosition + Vector3.right;

            if (!PositionHasObject(leftPosition) && !PositionHasObject(rightPosition))
            {
                spawnPosition = potentialPosition;
                validPositionFound = true;
            }

            validSpawnPositions.RemoveAt(randomIndex);
        }

        if (validPositionFound)
        {
            ObjectType objectType = RandomObjectType();
            GameObject gameObject = Instantiate(objectPrefabs[(int)objectType], spawnPosition, Quaternion.identity);
            spawnObjects.Add(gameObject);

            if (objectType != ObjectType.Enemy)
            {
                StartCoroutine(DestroyObjectsAfterTime(gameObject, gemLifeTime));
            }
        }
    }
    
    private IEnumerator DestroyObjectsAfterTime (GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        if (gameObject != null)
    {
        spawnObjects.Remove(gameObject); 
        validSpawnPositions.Add(gameObject.transform.position);
        Destroy(gameObject);
    }
}
    private void DestroyAllSpawnedObjects()
    {
        foreach(GameObject obj in spawnObjects)
        {
            if (obj != null)
            {
              Destroy(obj);
            }
        }
        spawnObjects.Clear();
    }

    private void GatherValidPositions()
    {
        validSpawnPositions.Clear();
        BoundsInt boundsInt = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(boundsInt);
        Vector3 start = tilemap.CellToWorld(new Vector3Int(boundsInt.xMin, boundsInt.yMin, 0));
        for (int x = 0; x < boundsInt.size.x; x++)
        {
            for (int y = 0; y < boundsInt.size.y; y++)
            {
                TileBase tile = allTiles[x + y * boundsInt.size.x];
                if (tile != null)
                {
                    Vector3 place = start + new Vector3(x + 7f, y + 9f, 0);
                    validSpawnPositions.Add(place);
                }
            }
        }
    }
}