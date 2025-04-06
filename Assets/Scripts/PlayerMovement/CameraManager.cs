using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraManager : NetworkBehaviour
{
    public Transform playerCamera;
    
    [Header("Camera Settings")]
    public Vector3 cameraOffset = new Vector3(0, 0.5f, 0); // Height offset from player center
    public float smoothTime = 0.1f; // Lower for faster response, higher for more smoothing
    
    // Variables for smoothing
    private Vector3 currentVelocity;
    private Transform originalCameraParent;

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
                return;
            }
        }

        // Store original parent
        originalCameraParent = playerCamera.parent;
        
        // Unparent camera to prevent direct physics influence
        playerCamera.parent = null;
        playerCamera.gameObject.SetActive(true);
    }

    public override void OnStopLocalPlayer()
    {
        base.OnStopLocalPlayer();

        if (playerCamera != null)
        {
            // Restore original parent
            playerCamera.parent = originalCameraParent;
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
    
    void LateUpdate()
    {
        if (!isLocalPlayer || playerCamera == null)
            return;
        
        // Calculate target position with offset
        Vector3 targetPosition = transform.position + cameraOffset;
        
        // Smoothly move camera position
        playerCamera.position = Vector3.SmoothDamp(
            playerCamera.position, 
            targetPosition, 
            ref currentVelocity, 
            smoothTime
        );
        
        // Keep the camera looking at the same rotation as player's look direction
        // We'll sync camera Y rotation with player, but keep X rotation (up/down) from PlayerLook
        Vector3 currentRotation = playerCamera.eulerAngles;
        playerCamera.rotation = Quaternion.Euler(currentRotation.x, transform.eulerAngles.y, 0);
    }
}
