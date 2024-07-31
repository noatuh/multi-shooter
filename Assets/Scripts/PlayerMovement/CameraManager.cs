using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraManager : NetworkBehaviour
{
    public Transform playerCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Automatically find the camera if not assigned
        if (playerCamera == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                playerCamera = mainCamera.transform;
                Debug.Log("Player camera automatically assigned.");
            }
            else
            {
                Debug.LogError("Player camera not assigned and no main camera found.");
            }
        }

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
        }
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();

        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }

    void Start()
    {
        if (!isLocalPlayer && playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }
    }
}
