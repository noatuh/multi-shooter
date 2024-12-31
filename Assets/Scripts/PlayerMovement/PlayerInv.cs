using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerInv : NetworkBehaviour
{
    public float pickupDistance = 5.0f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q; // Key for dropping items
    public string pickupTag = "Pickup";

    // Using SyncList to handle inventory
    public class SyncListGameObject : SyncList<GameObject> { }

    public SyncListGameObject inventory = new SyncListGameObject();
    private Camera playerCamera;

    void Start()
    {
        if (isLocalPlayer)
        {
            playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera == null)
            {
                Debug.LogError("PlayerCam not found in children.");
            }
            else
            {
                Debug.Log("PlayerCam successfully referenced.");
            }

            // Subscribe to inventory changes if implementing UI
            inventory.Callback += OnInventoryChanged;
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Pickup Functionality
        if (Input.GetKeyDown(pickupKey))
        {
            Debug.Log("Pickup key pressed.");
            RaycastHit hit;
            if (playerCamera != null && Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupDistance))
            {
                Debug.Log($"Raycast hit: {hit.collider.gameObject.name}");
                if (hit.collider.CompareTag(pickupTag))
                {
                    Debug.Log($"Object {hit.collider.gameObject.name} has the '{pickupTag}' tag. Attempting to pick up.");
                    CmdPickupObject(hit.collider.gameObject);
                }
                else
                {
                    Debug.Log($"Object {hit.collider.gameObject.name} does not have the '{pickupTag}' tag.");
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any object.");
            }
        }

        // Drop Functionality
        if (Input.GetKeyDown(dropKey))
        {
            Debug.Log("Drop key pressed.");
            DropObject();
        }
    }

    /// <summary>
    /// Initiates the drop process by selecting the last item in the inventory.
    /// </summary>
    void DropObject()
    {
        if (inventory.Count > 0)
        {
            // Select the last item in the inventory
            GameObject objToDrop = inventory[inventory.Count - 1];
            CmdDropObject(objToDrop);
        }
        else
        {
            Debug.LogWarning("Inventory is empty. Nothing to drop.");
        }
    }

    /// <summary>
    /// Command to handle dropping the object on the server.
    /// </summary>
    /// <param name="obj">The GameObject to drop.</param>
    [Command]
    void CmdDropObject(GameObject obj)
    {
        if (obj != null && obj.CompareTag(pickupTag) && inventory.Contains(obj))
        {
            Debug.Log($"CmdDropObject called for {obj.name} by ConnectionID {connectionToClient.connectionId}");
            // Remove from the server's inventory
            inventory.Remove(obj);
            // Enable the object in the game world
            RpcDropObject(obj, transform.position + transform.forward * 2.0f); // Adjust drop position as needed
        }
        else
        {
            Debug.LogWarning("CmdDropObject: Invalid object, tag mismatch, or object not in inventory.");
        }
    }

    /// <summary>
    /// ClientRpc to handle the visual and physical appearance of the dropped object on all clients.
    /// </summary>
    /// <param name="obj">The GameObject to drop.</param>
    /// <param name="dropPosition">The position where the object will be dropped.</param>
    [ClientRpc]
    void RpcDropObject(GameObject obj, Vector3 dropPosition)
    {
        if (obj != null)
        {
            Debug.Log($"RpcDropObject called for {obj.name} at position {dropPosition}.");
            obj.SetActive(true);
            obj.transform.position = dropPosition;
            obj.transform.rotation = Quaternion.identity; // Reset rotation if necessary

            // Re-enable physics
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.velocity = Vector3.zero; // Reset any residual velocity
                rb.angularVelocity = Vector3.zero;
            }

            // Optionally, re-enable collider if it was disabled
            Collider col = obj.GetComponent<Collider>();
            if (col != null && !col.enabled)
            {
                col.enabled = true;
            }

            // Update Inventory UI if implemented
            // Example: UpdateInventoryUI();
        }
        else
        {
            Debug.LogWarning("RpcDropObject: Received null object.");
        }
    }

    /// <summary>
    /// Command to handle picking up an object on the server.
    /// </summary>
    /// <param name="obj">The GameObject to pick up.</param>
    [Command]
    void CmdPickupObject(GameObject obj)
    {
        if (obj != null && obj.CompareTag(pickupTag) && !inventory.Contains(obj))
        {
            Debug.Log($"CmdPickupObject called for {obj.name} by ConnectionID {connectionToClient.connectionId}");
            // Add to the server's inventory
            inventory.Add(obj);
            // Disable the object on all clients
            RpcDisableObject(obj);
        }
        else
        {
            Debug.LogWarning("CmdPickupObject: Invalid object, tag mismatch, or already in inventory.");
        }
    }

    /// <summary>
    /// ClientRpc to disable the picked-up object on all clients.
    /// </summary>
    /// <param name="obj">The GameObject to disable.</param>
    [ClientRpc]
    void RpcDisableObject(GameObject obj)
    {
        if (obj != null)
        {
            Debug.Log($"RpcDisableObject called for {obj.name}.");
            obj.SetActive(false);
            // Optionally, update the inventory UI here
        }
        else
        {
            Debug.LogWarning("RpcDisableObject: Received null object.");
        }
    }

    /// <summary>
    /// Callback method triggered when the inventory changes.
    /// </summary>
    /// <param name="op">The operation performed.</param>
    /// <param name="index">The index at which the operation occurred.</param>
    /// <param name="oldItem">The old item (for remove operations).</param>
    /// <param name="newItem">The new item (for add operations).</param>
    void OnInventoryChanged(SyncList<GameObject>.Operation op, int index, GameObject oldItem, GameObject newItem)
    {
        UpdateInventoryUI();
    }

    /// <summary>
    /// Updates the inventory UI. Implement your UI logic here.
    /// </summary>
    void UpdateInventoryUI()
    {
        // Example: Update a Text component or a UI list with inventory items
        // This implementation is up to your specific UI setup
    }
}