using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar]
    private int health = 100;

    public int GetCurrentHealth()
    {
        return health;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            CmdTakeDamage(10);
        }

        if (health <= 0)
        {
            CmdShutdownPlayer();
        }
    }

    [Command]
    public void CmdTakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            CmdShutdownPlayer();
        }
    }

    [Command]
    void CmdShutdownPlayer()
    {
        RpcShutdownPlayer();
    }

    [ClientRpc]
    void RpcShutdownPlayer()
    {
        // Disable player components
        foreach (var component in GetComponents<Behaviour>())
        {
            component.enabled = false;
        }

        // Optionally, disable the GameObject
        gameObject.SetActive(false);

        // Close the game
        Application.Quit();

        // Display death message
        Debug.Log("Player has been shut down.");
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        health -= amount;
        if (health <= 0)
        {
            RpcShutdownPlayer();
        }
    }
}
