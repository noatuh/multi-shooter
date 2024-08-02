using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerJump : NetworkBehaviour
{
	public float jumpForce = 5f;
	private Rigidbody rb;

	// Start is called before the first frame update
	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdJump();
		}
	}

	[Command]
	void CmdJump()
	{
		RpcJump();
	}

	[ClientRpc]
	void RpcJump()
	{
		rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
	}
}