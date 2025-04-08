using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraManager : NetworkBehaviour
{
    public Transform playerCamera;
    
    [Header("Camera Settings")]
    public Vector3 cameraOffset = new Vector3(0, 0.5f, 0); // Height offset from player center
    public float smoothTime = 0.25f; // Increased for more stability
    
    [Header("Advanced Stabilization")]
    public int stabilizationSamples = 10; // Number of position samples to average
    public LayerMask collisionLayers = ~0; // All layers by default
    public float collisionOffset = 0.2f; // Distance to maintain from obstacles
    
    // Variables for smoothing
    private Vector3 currentVelocity;
    private Transform originalCameraParent;
    private Vector3[] positionSamples;
    private int currentSample = 0;
    private bool samplesInitialized = false;

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
        
        // Initialize position samples array
        positionSamples = new Vector3[stabilizationSamples];
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
        
        // Calculate stabilized position based on averaged samples
        Vector3 targetPosition = GetStabilizedPosition();
        
        // Apply camera collision
        targetPosition = HandleCameraCollision(targetPosition);
        
        // Apply smooth damping
        Vector3 newPosition = Vector3.SmoothDamp(
            playerCamera.position,
            targetPosition,
            ref currentVelocity,
            smoothTime
        );
        
        playerCamera.position = newPosition;
        
        // Sync rotation while preserving look-up/down angle
        Vector3 currentRotation = playerCamera.eulerAngles;
        playerCamera.rotation = Quaternion.Euler(currentRotation.x, transform.eulerAngles.y, 0);
    }
    
    Vector3 GetStabilizedPosition()
    {
        // Add current position to samples
        positionSamples[currentSample] = transform.position;
        currentSample = (currentSample + 1) % stabilizationSamples;
        
        // If we haven't filled the array yet, initialize remaining samples
        if (!samplesInitialized)
        {
            for (int i = 0; i < stabilizationSamples; i++)
            {
                if (positionSamples[i] == Vector3.zero)
                    positionSamples[i] = transform.position;
            }
            samplesInitialized = true;
        }
        
        // Average all samples
        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 sample in positionSamples)
        {
            averagePosition += sample;
        }
        averagePosition /= stabilizationSamples;
        
        // Apply offset to the averaged position
        return averagePosition + cameraOffset;
    }
    
    Vector3 HandleCameraCollision(Vector3 desiredPosition)
    {
        // Cast a ray from the player to the desired camera position
        Vector3 direction = desiredPosition - transform.position;
        float distance = direction.magnitude;
        
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.2f, direction.normalized, out hit, distance, collisionLayers))
        {
            // If we hit something, position the camera at the hit point minus offset
            return transform.position + direction.normalized * (hit.distance - collisionOffset);
        }
        
        return desiredPosition;
    }
}
