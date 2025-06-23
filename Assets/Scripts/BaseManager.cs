using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamColor
{
    Blue,
    Red
}

public class BaseManager : MonoBehaviour
{
    [Header("Team Settings")]
    [SerializeField] private TeamColor team;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Transform modelRoot;
    public Color TeamColorValue => GetTeamColor(team);

    [Header("Drone Settings")]
    [SerializeField] private GameObject dronePrefab;
    // [SerializeField] private Transform droneSpawnPoint;
    [SerializeField] private int initialDroneCount = 3;
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private float SpawnArcAngle = 90f;

    private List<GameObject> drones = new List<GameObject>();

    public IReadOnlyList<GameObject> Drones => drones;

    [Header("Resources")]
    private int collectedResources = 0;
    public int CollectedResources => collectedResources;
    public event System.Action<BaseManager> OnResourceChanged;

    private void Start()
    {
        ApplyTeamColor();
        SpawnInitialDrones();
    }

    private void ApplyTeamColor()
    {
        if (modelRoot != null)
        {
            var renderer = modelRoot.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // renderer.material.color = GetTeamColor(team);
                renderer.material = GetTeamMaterial();
            }
        }
    }

    private Color GetTeamColor(TeamColor team)
    {
        switch (team)
        {
            case TeamColor.Blue:
                return Color.blue;
            case TeamColor.Red:
                return Color.red;
            default:
                return Color.white;
        }
    }

    public Material GetTeamMaterial()
    {
        return team == TeamColor.Blue ? blueMaterial : redMaterial;
    }

    private void SpawnInitialDrones()
    {
        for (int i = 0; i < initialDroneCount; i++)
        {
            SpawnDrone();
        }
    }

    public void SetDroneCount(int newCount)
    {
        while (drones.Count < newCount)
            SpawnDrone();

        while (drones.Count > newCount)
            RemoveLastDrone();
    }

    private void SpawnDrone()
    {
        int index = drones.Count;
        int totalCount = initialDroneCount;

        Vector3 spawnPos = GetDistributedSpawnPosition(index, totalCount);

        // Направление от базы к позиции дрона
        Vector3 directionFromBase = (spawnPos - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(directionFromBase, Vector3.up);

        GameObject newDrone = Instantiate(dronePrefab, spawnPos, rotation);
        drones.Add(newDrone);
        newDrone.GetComponent<DroneAI>().Initialize(this, GetTeamMaterial());
    }

    private Vector3 GetDistributedSpawnPosition(int index, int totalCount)
    {
        if (totalCount == 1)
        {
            return transform.position + transform.forward * spawnRadius;
        }

        float angleStep = SpawnArcAngle / (totalCount - 1);
        float startAngle = -SpawnArcAngle / 2f;
        float angle = startAngle + index * angleStep;

        // Поворачиваем локальную дугу на +90, чтобы она шла вдоль forward
        float adjustedAngle = angle + 90f;

        Vector3 localOffset = Quaternion.Euler(0, adjustedAngle, 0) * (Vector3.forward * spawnRadius);
        return transform.position + transform.rotation * localOffset;
    }

    private void RemoveLastDrone()
    {
        if (drones.Count == 0) return;

        GameObject lastDrone = drones[drones.Count - 1];
        drones.RemoveAt(drones.Count - 1);
        Destroy(lastDrone);
    }

    public void ReportResourceCollected()
    {
        collectedResources++;
        OnResourceChanged?.Invoke(this);
    }
}

