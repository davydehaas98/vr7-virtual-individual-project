using UnityEngine;
using UnityEngine.AI;

public class ZombieNavigation : MonoBehaviour
{
    public Transform Target;

    private NavMeshAgent agent = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(Target.position);
    }

    void FixedUpdate()
    {
        agent.SetDestination(Target.position);
    }
}
