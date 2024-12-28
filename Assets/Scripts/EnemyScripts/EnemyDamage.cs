using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class EnemyDamage : NetworkBehaviour
{
    public float damageRadius = 1.0f;
    public int damageAmount = 10;
    public float damageInterval = 1.0f; // Time between damage applications

    private SphereCollider damageCollider;
    private Dictionary<GameObject, float> playerDamageTimers = new Dictionary<GameObject, float>();

    void Start()
    {
        if (isServer)
        {
            damageCollider = gameObject.AddComponent<SphereCollider>();
            damageCollider.isTrigger = true;
            damageCollider.radius = damageRadius;
        }
    }

    void Update()
    {
        if (!isServer) return;

        List<GameObject> playersInRange = new List<GameObject>();

        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playersInRange.Add(hit.gameObject);
            }
        }

        foreach (var player in playersInRange)
        {
            if (!playerDamageTimers.ContainsKey(player))
            {
                playerDamageTimers[player] = 0f;
            }

            playerDamageTimers[player] += Time.deltaTime;

            if (playerDamageTimers[player] >= damageInterval)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                }
                playerDamageTimers[player] = 0f;
            }
        }

        // Remove players that are no longer in range
        List<GameObject> keys = new List<GameObject>(playerDamageTimers.Keys);
        foreach (var player in keys)
        {
            if (!playersInRange.Contains(player))
            {
                playerDamageTimers.Remove(player);
            }
        }
    }
}