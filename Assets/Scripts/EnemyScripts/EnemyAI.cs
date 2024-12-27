using UnityEngine;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : NetworkBehaviour
{
    private Transform player;
    public float detectionRange = 5f;
    public float moveSpeed = 2f;
    public float patrolSpeed = 2f;
    public float hoverHeight = 1f;

    private Vector3[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Rigidbody rb;

    void Start()
    {
        if (!isServer) return;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disable gravity, as we manually control vertical position

        Vector3 startPosition = transform.position;

        float randomOffsetX = Random.Range(-5f, 5f);
        float randomOffsetZ = Random.Range(-5f, 5f);
        Vector3 randomOffset = new Vector3(randomOffsetX, 0, randomOffsetZ);

        float pathRange = 20f;

        List<Vector3> randomPoints = new List<Vector3>();
        for (int i = 0; i < 4; i++)
        {
            float offsetX = Random.Range(-pathRange, pathRange);
            float offsetZ = Random.Range(-pathRange, pathRange);
            randomPoints.Add(startPosition + new Vector3(offsetX, 0, offsetZ));
        }

        patrolPoints = randomPoints.ToArray();

        FindPlayer();
    }

    void Update()
    {
        if (!isServer) return;

        if (player == null)
        {
            FindPlayer();
        }

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log("Distance to player: " + distanceToPlayer);

            if (distanceToPlayer <= detectionRange)
            {
                MoveTowards(player.position, moveSpeed);
                Debug.Log("Moving towards player at position: " + player.position);
                return;
            }
        }

        Patrol();
    }

    [ServerCallback]
    void OnPlayerSpawned(GameObject playerObject)
    {
        if (playerObject.CompareTag("Player"))
        {
            player = playerObject.transform;
            Debug.Log("Player found at position: " + player.position);
        }
    }

    void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Player found at position: " + player.position);
        }
        else
        {
            Debug.Log("Player not found");
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Vector3 targetPosition = patrolPoints[currentPatrolIndex];
        
        // Adjust target position to maintain hover height above ground
        targetPosition.y = GetGroundHeight(targetPosition) + hoverHeight;

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            Debug.Log("Switching to patrol point index: " + currentPatrolIndex);
        }

        MoveTowards(targetPosition, patrolSpeed);
        Debug.Log("Patrolling to point: " + targetPosition);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;

        // Calculate the desired velocity to move towards the target
        Vector3 velocity = direction * speed;

        // Update the Rigidbody's velocity
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Adjust position to maintain hover height
        float groundHeight = GetGroundHeight(transform.position);
        float desiredHeight = groundHeight + hoverHeight;
        transform.position = new Vector3(transform.position.x, desiredHeight, transform.position.z);

        Debug.Log("Moving towards target. Current position: " + transform.position + ", Direction: " + direction);
    }

    float GetGroundHeight(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity))
        {
            return hit.point.y;
        }
        return position.y;
    }
}
