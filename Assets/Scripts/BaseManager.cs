using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    [Header("Drone Settings")]
    [SerializeField] private GameObject dronePrefab;
    [SerializeField] private Transform droneSpawnPoint;
    [SerializeField] private int initialDroneCount = 5;

    private List<GameObject> drones = new List<GameObject>();

    public IReadOnlyList<GameObject> Drones => drones;

    private void Start()
    {
        SpawnInitialDrones();
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
        GameObject newDrone = Instantiate(dronePrefab, droneSpawnPoint.position, Quaternion.identity);
        drones.Add(newDrone);
        newDrone.GetComponent<DroneAI>().Initialize(this);
    }

    private void RemoveLastDrone()
    {
        if (drones.Count == 0) return;

        GameObject lastDrone = drones[drones.Count - 1];
        drones.RemoveAt(drones.Count - 1);
        Destroy(lastDrone);
    }
}

