using UnityEngine;
using Mirror;

public class EnemyAI : NetworkBehaviour
{
    private Transform player; // Reference to the player's transform
    public float detectionRange = 100f; // Detection range in meters
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public float patrolSpeed = 2f; // Speed for patrolling

    private Vector3[] patrolPoints; // Points for patrolling
    private int currentPatrolIndex = 0; // Current patrol point index

    void Start()
    {
        if (!isServer) return;

        // Initialize patrol points in a 10x10 grid around the spawn point
        Vector3 startPosition = transform.position;
        patrolPoints = new Vector3[]
        {
            startPosition + new Vector3(10, 0, 0),
            startPosition + new Vector3(10, 0, 10),
            startPosition + new Vector3(0, 0, 10),
            startPosition
        };

        // Attempt to find the player initially
        FindPlayer();
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }

        if (player == null)
        {
            FindPlayer();
        }

        if (player != null)
        {
            // Calculate the distance between the enemy and the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.Log("Distance to player: " + distanceToPlayer);

            // Check if the player is within the detection range
            if (distanceToPlayer <= detectionRange)
            {
                // Move the enemy towards the player
                MoveTowards(player.position, moveSpeed);
                Debug.Log("Moving towards player at position: " + player.position);
                return;
            }
        }

        // Patrol if no player is detected
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
        if (patrolPoints.Length == 0)
            return;

        Vector3 targetPosition = patrolPoints[currentPatrolIndex];
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget < 0.1f)
        {
            // Move to the next patrol point
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        MoveTowards(targetPosition, patrolSpeed);
        Debug.Log("Patrolling to point: " + targetPosition);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }
}
