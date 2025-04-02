using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public enum ObjectType { SmallGem, BigGem, Enemy }

    [Header("Prefabs")]
    public GameObject[] objectPrefabs;

    [Header("Probabilidades de Gemas")]
    [Range(0f, 1f)] public float bigGemChance = 0.2f;

    [Header("Configuración")]
    public int maxGems = 15;
    public int maxEnemies = 3;
    public float gemSpawnInterval = 0.2f;
    public float enemySpawnInterval = 5f;
    public float gemLifetime = 10f;

    [Header("Área de Spawn")]
    public Vector2 spawnOrigin = new Vector2(-10, 0);
    public Vector2 spawnAreaSize = new Vector2(20, 5);

    private List<GameObject> gemObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < maxGems; i++)
        {
            SpawnGemInstant();
        }

        StartCoroutine(GemSpawner());
        StartCoroutine(EnemySpawner());
    }


    void Update()
    {
        gemObjects.RemoveAll(item => item == null);
        enemyObjects.RemoveAll(item => item == null);
    }

    private IEnumerator GemSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(gemSpawnInterval);

            if (gemObjects.Count >= maxGems) continue;

            SpawnGemInstant();
        }
    }

    private void SpawnGemInstant()
    {
        Vector3 spawnPos = GetRandomSpawnPosition();
        if (PositionHasObject(spawnPos)) return;

        ObjectType gemType = (Random.value <= bigGemChance) ? ObjectType.BigGem : ObjectType.SmallGem;
        GameObject gem = Instantiate(objectPrefabs[(int)gemType], spawnPos, Quaternion.identity);
        gemObjects.Add(gem);

        StartCoroutine(DestroyAfterTime(gem, gemLifetime));
    }

    private IEnumerator EnemySpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnInterval);

            if (enemyObjects.Count >= maxEnemies) continue;
            SpawnEnemy();
            Vector3 spawnPos = GetRandomSpawnPosition();
            if (PositionHasObject(spawnPos)) continue;

            GameObject enemy = Instantiate(objectPrefabs[(int)ObjectType.Enemy], spawnPos, Quaternion.identity);
            enemyObjects.Add(enemy);
        }
    }
    private void SpawnEnemy()
{
    Vector3 spawnPos = GetRandomSpawnPosition();
    Debug.Log("Intentando spawnear enemigo en: " + spawnPos);

    GameObject enemy = Instantiate(objectPrefabs[(int)ObjectType.Enemy], spawnPos, Quaternion.identity);
    enemyObjects.Add(enemy);
    Debug.Log("¡Enemigo instanciado!");
}


    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(spawnOrigin.x, spawnOrigin.x + spawnAreaSize.x);
        float y = Random.Range(spawnOrigin.y, spawnOrigin.y + spawnAreaSize.y);
        return new Vector3(x, y, 0);
    }

    private bool PositionHasObject(Vector3 position)
    {
        foreach (GameObject obj in gemObjects)
        {
            if (obj != null && Vector3.Distance(obj.transform.position, position) < 1f)
                return true;
        }
        foreach (GameObject obj in enemyObjects)
        {
            if (obj != null && Vector3.Distance(obj.transform.position, position) < 1f)
                return true;
        }
        return false;
    }

    private IEnumerator DestroyAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        if (obj != null)
        {
            gemObjects.Remove(obj);
            Destroy(obj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(spawnOrigin + spawnAreaSize / 2f, spawnAreaSize);
    }
}
