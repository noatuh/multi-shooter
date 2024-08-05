using UnityEngine;
using Mirror;

public class EnemyAI : NetworkBehaviour
{
    private Transform player; // Reference to the player's transform
    public float detectionRange = 100f; // Detection range in meters
    public float moveSpeed = 2f; // Speed at which the enemy moves

    void Start()
    {
        if (!isServer) return;

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
            return;
        }

        // Calculate the distance between the enemy and the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Debug.Log("Distance to player: " + distanceToPlayer);

        // Check if the player is within the detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Move the enemy towards the player
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            Debug.Log("Moving towards player at position: " + player.position);
        }
        else
        {
            Debug.Log("Player out of range");
        }
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
}
