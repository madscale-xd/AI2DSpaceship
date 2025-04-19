using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner2D : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefabToSpawn;
    public int numberOfPrefabs = 10;
    public float minDistanceBetweenPrefabs = 1.5f;

    [Header("Scale Settings")]
    public Vector2 scaleRange = new Vector2(0.5f, 1.5f); 

    [Header("Purinrin Safety Settings")]
    public string purinrinTag = "Purinrin";
    public float safeRadiusAroundPurinrin = 2.0f;

    private float spawnAreaWidth;
    private float spawnAreaHeight;

    private List<Vector2> spawnPositions = new List<Vector2>();
    private Transform[] purinrinObjects;

    void Start()
    {
        purinrinObjects = GetPurinrinObjects();
        CalculateBounds();
        SpawnPrefabs();
    }

    Transform[] GetPurinrinObjects()
    {
        GameObject[] found = GameObject.FindGameObjectsWithTag(purinrinTag);
        List<Transform> transforms = new List<Transform>();
        foreach (GameObject obj in found)
        {
            transforms.Add(obj.transform);
        }
        return transforms.ToArray();
    }

    void CalculateBounds()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            spawnAreaWidth = sr.bounds.size.x;
            spawnAreaHeight = sr.bounds.size.y;
            return;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            spawnAreaWidth = col.bounds.size.x;
            spawnAreaHeight = col.bounds.size.y;
            return;
        }

        spawnAreaWidth = 5f;
        spawnAreaHeight = 5f;
        Debug.LogWarning("No SpriteRenderer or Collider2D found, using default spawn area.");
    }

    void SpawnPrefabs()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogWarning("No prefab assigned to spawner!");
            return;
        }

        int spawnedCount = 0;
        int attempts = 0;
        int maxAttempts = 1000;

        while (spawnedCount < numberOfPrefabs && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomOffset = new Vector2(
                Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f),
                Random.Range(-spawnAreaHeight / 2f, spawnAreaHeight / 2f)
            );

            Vector2 spawnPosition = (Vector2)transform.position + randomOffset;

            bool tooClose = false;

            // Check against other spawned asteroids
            foreach (Vector2 pos in spawnPositions)
            {
                if (Vector2.Distance(pos, spawnPosition) < minDistanceBetweenPrefabs)
                {
                    tooClose = true;
                    break;
                }
            }

            // Check against Purinrins
            if (!tooClose)
            {
                foreach (Transform purinrin in purinrinObjects)
                {
                    if (Vector2.Distance(purinrin.position, spawnPosition) < safeRadiusAroundPurinrin)
                    {
                        tooClose = true;
                        break;
                    }
                }
            }

            if (!tooClose)
            {
                GameObject instance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                float randomScale = Random.Range(scaleRange.x, scaleRange.y);
                instance.transform.localScale = new Vector3(randomScale, randomScale, 1f); // Uniform scale for 2D
                spawnPositions.Add(spawnPosition);
                spawnedCount++;
            }
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Spawn attempt limit reached. Some prefabs may not have been placed.");
        }
    }

    void OnDrawGizmosSelected()
    {
        CalculateBounds();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaWidth, spawnAreaHeight, 0));

        // Show safe zones around Purinrin objects
        GameObject[] purins = GameObject.FindGameObjectsWithTag(purinrinTag);
        Gizmos.color = new Color(1f, 0.2f, 0.6f, 0.4f);
        foreach (GameObject purin in purins)
        {
            Gizmos.DrawWireSphere(purin.transform.position, safeRadiusAroundPurinrin);
        }
    }
}
