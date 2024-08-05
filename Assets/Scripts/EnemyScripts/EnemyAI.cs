using UnityEngine;

public class EnemyAI : MonoBehaviour
{
	private Vector3 spawnPosition;
	private Vector3 targetPosition;
	private float moveInterval = 2.0f; // Time between moves
	private float nextMoveTime;
	private float moveSpeed = 2.0f; // Speed of movement

	void Start()
	{
		spawnPosition = transform.position;
		SetRandomTargetPosition();
		nextMoveTime = Time.time + moveInterval;
	}

	void Update()
	{
		if (Time.time >= nextMoveTime)
		{
			SetRandomTargetPosition();
			nextMoveTime = Time.time + moveInterval;
		}

		MoveTowardsTarget();
	}

	void SetRandomTargetPosition()
	{
		float randomX = Random.Range(-5.0f, 5.0f);
		float randomZ = Random.Range(-5.0f, 5.0f);
		targetPosition = new Vector3(spawnPosition.x + randomX, spawnPosition.y, spawnPosition.z + randomZ);
	}

	void MoveTowardsTarget()
	{
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
	}
}