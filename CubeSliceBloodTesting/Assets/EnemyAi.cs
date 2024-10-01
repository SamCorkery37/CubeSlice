using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;  // List of patrol points
    public float patrolSpeed = 3.5f;
    public float chaseSpeed = 5f;
    public float stoppingDistance = 3f;  // Distance to stop from the player
    public float detectionRange = 10f;   // Range within which the enemy detects the player

    private int currentPatrolIndex;
    private NavMeshAgent agent;
    private Transform player;
    private bool playerInSight = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;  // Assumes player has the tag 'Player'
        agent.autoBraking = false;  // Enemy doesn't slow down when reaching a waypoint
        GoToNextPatrolPoint();
    }

    void Update()
    {
        // Check distance to player and update behavior
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Player is within detection range, chase the player
            playerInSight = true;
        }
        else
        {
            // Player is out of range, go back to patrol
            playerInSight = false;
        }

        if (playerInSight)
        {
            ChasePlayer(distanceToPlayer);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // If the agent is not moving or close to the patrol point, go to the next point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }
        agent.speed = patrolSpeed;  // Patrol speed
    }

    void GoToNextPatrolPoint()
    {
        // Go to the next patrol point in the array, looping back to the start
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPatrolIndex].position;

        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ChasePlayer(float distanceToPlayer)
    {
        agent.speed = chaseSpeed;  // Chase speed

        if (distanceToPlayer > stoppingDistance)
        {
            // Move closer to the player if further than the stopping distance
            agent.destination = player.position;
        }
        else
        {
            // Stop at the defined stopping distance
            agent.isStopped = true;
        }
    }
}
