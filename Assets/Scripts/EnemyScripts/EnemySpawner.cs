using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// Place this on empty GameObjects around the world (e.g., in buildings/structures).
// It will spawn enemies (zombies) when a player is nearby and despawn them when players leave.
public class EnemySpawner : NetworkBehaviour
{
    [Header("Spawn Setup")]
    [Tooltip("Prefab of the enemy to spawn. Must have a NetworkIdentity and be registered in NetworkManager.")]
    public GameObject enemyPrefab;

    [Tooltip("Optional fixed spawn points. If empty, spawns within Spawn Radius around this spawner.")]
    public Transform[] spawnPoints;

    [Header("Counts & Timing")]
    [Tooltip("Maximum number of alive enemies this spawner can have at once.")]
    public int maxAlive = 3;

    [Tooltip("Seconds between individual spawns while filling up to Max Alive.")]
    public float respawnDelay = 2f;

    [Tooltip("How often (seconds) to check player proximity and manage spawn/despawn.")]
    public float checkInterval = 1f;

    [Tooltip("If no players in Deactivate Range for this many seconds, despawn all.")]
    public float despawnAfterSeconds = 10f;

    [Header("Ranges (meters)")]
    [Tooltip("Players within this distance will activate spawning.")]
    public float activateRange = 30f;

    [Tooltip("If no players are within this distance for 'Despawn After Seconds', despawn all.")]
    public float deactivateRange = 45f;

    [Tooltip("When not using Spawn Points, radius around this spawner to place enemies.")]
    public float spawnRadius = 10f;

    [Tooltip("Clamp spawn to ground by raycasting down from above the chosen position.")]
    public bool alignToGround = true;

    [Tooltip("Offset above ground when aligning to ground.")]
    public float groundOffset = 0.1f;

    private readonly List<GameObject> alive = new List<GameObject>();
    private bool spawning;
    private float noPlayersTimer;

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(ServerLoop());
    }

    [Server]
    private IEnumerator ServerLoop()
    {
        var wait = new WaitForSeconds(checkInterval);

        while (true)
        {
            // Cleanup destroyed entries
            alive.RemoveAll(go => go == null);

            bool anyInActivate = AnyPlayerWithin(activateRange);
            bool anyInDeactivate = AnyPlayerWithin(deactivateRange);

            if (anyInActivate)
            {
                noPlayersTimer = 0f;
                // Top up to maxAlive
                if (!spawning && alive.Count < maxAlive)
                {
                    StartCoroutine(SpawnUntilFull());
                }
            }
            else
            {
                // No players in activate range; check despawn condition using deactivate range
                if (!anyInDeactivate)
                {
                    noPlayersTimer += checkInterval;
                    if (noPlayersTimer >= despawnAfterSeconds)
                    {
                        DespawnAll();
                        noPlayersTimer = 0f;
                    }
                }
                else
                {
                    // Players are around but outside activate range: don't spawn, don't despawn
                    noPlayersTimer = 0f;
                }
            }

            yield return wait;
        }
    }

    [Server]
    private IEnumerator SpawnUntilFull()
    {
        spawning = true;
        while (alive.Count < maxAlive)
        {
            Vector3 pos;
            Quaternion rot;
            GetSpawnTransform(out pos, out rot);

            GameObject enemy = Instantiate(enemyPrefab, pos, rot);
            NetworkServer.Spawn(enemy);
            alive.Add(enemy);

            yield return new WaitForSeconds(respawnDelay);

            // Early exit if no players nearby anymore
            if (!AnyPlayerWithin(activateRange))
                break;
        }
        spawning = false;
    }

    [Server]
    private void DespawnAll()
    {
        for (int i = 0; i < alive.Count; i++)
        {
            if (alive[i] != null)
            {
                NetworkServer.Destroy(alive[i]);
            }
        }
        alive.Clear();
    }

    [Server]
    private bool AnyPlayerWithin(float range)
    {
        // Use tag-based lookup; ensure your player prefabs are tagged "Player".
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Vector3 p = transform.position;
        foreach (var pl in players)
        {
            if (pl == null || !pl.activeInHierarchy) continue;
            float d = Vector3.Distance(p, pl.transform.position);
            if (d <= range) return true;
        }
        return false;
    }

    [Server]
    private void GetSpawnTransform(out Vector3 position, out Quaternion rotation)
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform t = spawnPoints[Random.Range(0, spawnPoints.Length)];
            position = t.position;
            rotation = t.rotation;
        }
        else
        {
            Vector2 circle = Random.insideUnitCircle * spawnRadius;
            position = transform.position + new Vector3(circle.x, 0f, circle.y);
            rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }

        if (alignToGround)
        {
            // Raycast from above down to find ground height
            Vector3 rayStart = position + Vector3.up * 50f;
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 200f))
            {
                position = hit.point + Vector3.up * groundOffset;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, activateRange);

        Gizmos.color = new Color(1f, 0.6f, 0.2f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, deactivateRange);

        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}
