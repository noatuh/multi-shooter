using UnityEngine;
using Mirror;

public class TreeCollider : NetworkBehaviour
{
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        if (capsuleCollider == null)
        {
            Debug.LogError("Capsule Collider not found on Tree Prefab. Please add one in the Inspector.");
            return;
        }

        // Ensure the collider is set correctly
        capsuleCollider.isTrigger = false; // It should not be a trigger for solid collisions
    }

    public override void OnStartServer()
    {
        // Ensure collider is enabled on the server
        capsuleCollider.enabled = true;
    }

    public override void OnStartClient()
    {
        if (!isServer)
        {
            // Ensure collider is enabled on clients
            capsuleCollider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Ensure the player has the tag "Player"
        {
            Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 pushDirection = collision.contacts[0].point - transform.position;
                pushDirection.y = 0; // Keep the push in the horizontal plane
                pushDirection.Normalize();

                float pushForce = 5f; // Adjust this value for the desired push strength
                playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }
        }
    }
}
