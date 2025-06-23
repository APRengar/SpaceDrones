using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DroneAI : MonoBehaviour
{
    private enum DroneState { Idle, MovingToResource, Gathering, MovingToBase, Depositing }

    [SerializeField] private DroneState state = DroneState.Idle;

    [SerializeField] private GameObject carriedResourcePrefab;
    [SerializeField] private Transform carryPoint;

    private GameObject carriedVisual;

    private BaseManager baseManager;
    private Material teamMaterial;
    private NavMeshAgent agent;

    private ResourceNode currentTarget;

    [SerializeField] private float interactionDistance = 2f;

    private bool hasResource = false;

    public void Initialize(BaseManager ownerBase, Material mat)
    {
        baseManager = ownerBase;
        teamMaterial = mat;

        ApplyMaterial(mat);
        agent = GetComponent<NavMeshAgent>();

        agent.avoidancePriority = Random.Range(10, 90);

        FindNewTarget();

        if (currentTarget != null)
        {
            state = DroneState.MovingToResource;
            agent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            state = DroneState.Idle;
        }
    }

    private void ApplyMaterial(Material mat)
    {
        var renderer = GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
            renderer.material = mat;
    }

    private void Update()
    {
        switch (state)
        {
            case DroneState.Idle:
                if (currentTarget != null)
                {
                    state = DroneState.MovingToResource;
                }
                else
                {
                    FindNewTarget();
                }
                break;
            case DroneState.MovingToResource:
                if (currentTarget == null)
                {
                    FindNewTarget();
                    break;
                }

                if (Vector3.Distance(transform.position, currentTarget.transform.position) <= interactionDistance)
                {
                    StartCollectingResource();
                }
                break;

            case DroneState.MovingToBase:
                float distToBase = Vector3.Distance(transform.position, baseManager.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    agent.isStopped = true;
                    break;
                }
                if (distToBase <= interactionDistance)
                {
                    DepositResource();
                }
                break;
        }
    }

    private void StartCollectingResource()
    {
        if (state != DroneState.MovingToResource || currentTarget == null) return;

        if (!currentTarget.IsAvailable)
        {
            // Цель уже занята кем-то другим — найти новую
            currentTarget = null;
            FindNewTarget();
            return;
        }

        state = DroneState.Gathering;
        agent.isStopped = true;

        currentTarget.StartCollecting(OnResourceCollected);
    }

    private void OnResourceCollected()
    {
        agent.isStopped = false;
        hasResource = true;
        currentTarget = null;
        // ➕ Спавн визуального ресурса
        if (carriedResourcePrefab != null && carryPoint != null)
        {
            carriedVisual = Instantiate(carriedResourcePrefab, carryPoint);
            carriedVisual.transform.localPosition = Vector3.zero;
            carriedVisual.transform.localRotation = Quaternion.identity;
        }

        MoveToBase();
    }

    private void MoveToBase()
    {
        state = DroneState.MovingToBase;
        agent.SetDestination(baseManager.transform.position);
    }

    private void DepositResource()
    {
        if (!hasResource) return;

        hasResource = false;
        baseManager.ReportResourceCollected();
        // ➖ Удаление визуального ресурса
        if (carriedVisual != null)
        {
            Destroy(carriedVisual);
            carriedVisual = null;
        }

        FindNewTarget();
    }

    private void FindNewTarget()
    {
        ResourceNode[] allNodes = GameObject.FindObjectsOfType<ResourceNode>();
        float minDist = float.MaxValue;
        ResourceNode closest = null;

        foreach (var node in allNodes)
        {
            float dist = Vector3.Distance(transform.position, node.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = node;
            }
        }

        if (closest != null)
        {
            currentTarget = closest;
            state = DroneState.MovingToResource;
            agent.SetDestination(currentTarget.transform.position);
        }
        else
        {
            state = DroneState.Idle;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);
    }
}
