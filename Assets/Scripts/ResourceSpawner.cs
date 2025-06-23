using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [Header("Spawning Area")]
    [SerializeField] private BoxCollider spawnArea;

    [Header("Resource Settings")]
    [SerializeField] private GameObject resourcePrefab;
    [SerializeField] private int initialResourceCount = 10;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private int maxResources = 50;

    private float timer;

    private void Start()
    {
        SpawnInitialResources();
        timer = spawnInterval;
    }

    private void SpawnInitialResources()
    {
        for (int i = 0; i < initialResourceCount; i++)
        {
            SpawnSingleResource();
        }
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            timer = spawnInterval;

            int currentResourceCount = FindObjectsOfType<ResourceNode>().Length;
            if (currentResourceCount < maxResources)
            {
                SpawnSingleResource();
            }
        }
    }

    private void SpawnSingleResource()
    {
        Vector3 spawnPosition = GetRandomPointInBounds(spawnArea);
        Instantiate(resourcePrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPointInBounds(BoxCollider area)
    {
        Bounds bounds = area.bounds;
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = bounds.center.y; // Предположим, что ресурсы стоят на земле
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, y, z);
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = interval;
    }
}
