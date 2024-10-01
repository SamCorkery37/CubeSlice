using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCWanderWithConversations : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;
    public float stopDuration = 2f;
    public float conversationRange = 5f; // Range within which NPCs can notice each other
    public float conversationDuration = 3f; // How long NPCs stay in "conversation"
    public float conversationChance = 0.2f; // 20% chance to start a conversation

    private NavMeshAgent agent;
    private float timer;
    private bool isStopping = false;
    private bool inConversation = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        if (!inConversation)
        {
            if (!isStopping)
            {
                timer += Time.deltaTime;

                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.SetDestination(newPos);
                    timer = 0f;
                    StartCoroutine(StopAndPause());
                }
            }

            // Check for nearby NPCs
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, conversationRange);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("NPC") && hitCollider.gameObject != this.gameObject)
                {
                    // Add random chance for conversation
                    if (Random.value <= conversationChance)
                    {
                        StartCoroutine(HoldConversation(hitCollider.transform));
                    }
                    break;
                }
            }
        }
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    IEnumerator StopAndPause()
    {
        isStopping = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(stopDuration);
        agent.isStopped = false;
        isStopping = false;
    }

    // Coroutine to handle conversation logic
    IEnumerator HoldConversation(Transform otherNPC)
    {
        inConversation = true;

        // Stop both NPCs
        agent.isStopped = true;

        // Rotate to face each other
        Vector3 directionToOther = otherNPC.position - transform.position;
        directionToOther.y = 0; // Keep the rotation on the horizontal plane
        Quaternion lookRotation = Quaternion.LookRotation(directionToOther);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Simulate the conversation for a set duration
        yield return new WaitForSeconds(conversationDuration);

        // Resume wandering after conversation
        agent.isStopped = false;
        inConversation = false;
    }
}
