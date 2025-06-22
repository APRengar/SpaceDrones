using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


[RequireComponent(typeof(NavMeshAgent))]
public class DroneAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private BaseManager baseManager;

    private ResourceNode currentTarget;

    private float searchCooldown = 0.5f;
    private float lastSearchTime = -999f;

    public void Initialize(BaseManager ownerBase)
    {
        baseManager = ownerBase;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (currentTarget == null)
        {
            if (Time.time - lastSearchTime >= searchCooldown)
            {
                lastSearchTime = Time.time;
                FindNewTarget();
            }
        }
        else
        {
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distance <= agent.stoppingDistance + 0.1f)
            {
                agent.ResetPath();
                currentTarget.StartCollecting(OnResourceCollected);
                currentTarget = null;
            }
        }
    }

    private void FindNewTarget()
    {
        ResourceNode[] allResources = FindObjectsOfType<ResourceNode>();

        currentTarget = allResources
            .Where(r => r.IsAvailable)
            .OrderBy(r => Vector3.Distance(transform.position, r.transform.position))
            .FirstOrDefault();

        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.transform.position);
        }
    }

    private void OnResourceCollected()
    {
        // После сбора дрон сам найдёт новый ресурс в следующем Update
    }
}
