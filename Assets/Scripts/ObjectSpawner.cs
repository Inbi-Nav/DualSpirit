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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var pos in validSpawnPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.15f);
        }
    }
   private void GatherValidPositions()
    {
        validSpawnPositions.Clear();

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                int tileIndex = x + y * bounds.size.x;
                TileBase currentTile = allTiles[tileIndex];

                if (currentTile != null)
                {
                    Vector3Int cell = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                    Vector3Int cellAbove = new Vector3Int(cell.x, cell.y + 1, 0);
                    Vector3Int cellBelow = new Vector3Int(cell.x, cell.y - 1, 0);

                    bool isAirAbove = !tilemap.HasTile(cellAbove);
                    bool isAirBelow = !tilemap.HasTile(cellBelow);

                    if (isAirAbove && isAirBelow)
                    {
                        Vector3 cellWorldPos = tilemap.CellToWorld(cell);
                        float offsetY = tilemap.cellSize.y * 0.5f + 0.5f; 

                        Vector3 spawnPos = cellWorldPos + new Vector3(0.5f, offsetY, 0);
                        validSpawnPositions.Add(spawnPos);
                    }
                }
            }
        }
    }
}