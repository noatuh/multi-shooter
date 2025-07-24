using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerJump : NetworkBehaviour
{
    public float jumpForce = 10f;
    private Rigidbody rb;
    private bool isGrounded;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    private CapsuleCollider playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        
        // Create a physics material with no bounce
        PhysicMaterial noBounceMaterial = new PhysicMaterial("NoBounceMaterial");
        noBounceMaterial.bounciness = 0f;
        noBounceMaterial.dynamicFriction = 0.6f;
        noBounceMaterial.staticFriction = 0.6f;
        noBounceMaterial.bounceCombine = PhysicMaterialCombine.Minimum;
        noBounceMaterial.frictionCombine = PhysicMaterialCombine.Average;
        
        // Apply the material to the player's collider
        playerCollider.material = noBounceMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            CmdJump();
        }
    }

    private void CheckGrounded()
    {
        // Simple raycast downward from the bottom of the player
        Vector3 rayStart = transform.position;
        Vector3 rayDirection = Vector3.down;
        float rayDistance = 1.1f; // Slightly more than half the player's height
        
        // Perform the raycast without layer restrictions
        isGrounded = Physics.Raycast(rayStart, rayDirection, rayDistance);
        
        // Debug line to visualize the raycast in the scene view
        Debug.DrawRay(rayStart, rayDirection * rayDistance, isGrounded ? Color.green : Color.red);
    }

    [Command]
    void CmdJump()
    {
        RpcJump();
    }

    [ClientRpc]
    void RpcJump()
    {
        // Apply upward force for the jump
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // Reset vertical velocity before jumping
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Draw gizmos in the editor to visualize the ground check
    private void OnDrawGizmosSelected()
    {
        if (playerCollider == null)
        {
            playerCollider = GetComponent<CapsuleCollider>();
        }

        float radius = playerCollider.radius * 0.9f;
        Vector3 start = transform.position + Vector3.up * radius;
        float maxDistance = playerCollider.height / 2 - radius + groundCheckDistance;

        // Set gizmo color based on whether the player is grounded
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Draw the sphere at the start and end positions of the sphere cast
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(start + Vector3.down * maxDistance, radius);
    }
}