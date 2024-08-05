using UnityEngine;
using Mirror;

public class PlayerTagger : NetworkBehaviour
{
	public override void OnStartLocalPlayer()
	{
		// Ensure the GameObject is tagged as "Player"
		if (gameObject.tag != "Player")
		{
			gameObject.tag = "Player";
		}
	}
}