using UnityEngine;
using Mirror;

public class KeepUpRight : NetworkBehaviour
{
	public float uprightForce = 10f; // Force to keep the player upright

	private Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		// Ensure this script only runs on the local player
		if (!isLocalPlayer)
		{
			return;
		}

		// Calculate the desired up direction
		Vector3 desiredUp = Vector3.up;

		// Calculate the current up direction
		Vector3 currentUp = transform.up;

		// Calculate the torque needed to align the current up direction with the desired up direction
		Vector3 torque = Vector3.Cross(currentUp, desiredUp) * uprightForce;

		// Apply the torque to the Rigidbody
		rb.AddTorque(torque);
	}
}