
# Mirror Zombie Spawner System

A flexible, networked enemy spawner for Mirror-based multiplayer games. Spawns zombies near players and despawns them when players leave the area—ideal for open-world survival games like DayZ or Unturned.

## Features

- **Network-aware:** Spawns and despawns run on the server, synced to all clients via Mirror.
- **Proximity-based:** Enemies spawn when players approach, despawn when players leave.
- **Configurable:** Set spawn radii, max alive, respawn delay, and despawn timing per spawner.
- **Ground alignment:** Spawns are raycasted to ground for natural placement.
- **Supports spawn points:** Use explicit spawn locations or random positions within a radius.
- **Gizmo visualization:** See activation/deactivation/spawn areas in the Scene view.

## Setup

1. **Zombie Prefab:**
	 - Must have a `NetworkIdentity` and be registered in your Mirror `NetworkManager`.
	 - Should include your movement/AI scripts (e.g., `EnemyAI`).

2. **Player Tag:**
	 - Your player prefab must be tagged `Player` for proximity checks to work.

3. **Spawner Placement:**
	 - In your scene, create an empty GameObject at each desired spawn area (e.g., inside buildings).
	 - Add the `EnemySpawner` script (found in `Assets/Scripts/EnemyScripts/EnemySpawner.cs`).
	 - Assign your zombie prefab to `enemyPrefab`.
	 - Optionally, add child Transforms as spawn points and assign them to `spawnPoints`.
	 - Adjust `activateRange`, `deactivateRange`, `maxAlive`, `respawnDelay`, and `despawnAfterSeconds` as needed.

## Inspector Fields

- `enemyPrefab`: The zombie prefab to spawn.
- `spawnPoints`: (Optional) Array of Transforms for fixed spawn locations.
- `maxAlive`: Maximum number of zombies alive at once per spawner.
- `respawnDelay`: Time between spawns while filling up to max.
- `activateRange`: Distance at which players trigger spawning.
- `deactivateRange`: Distance at which despawning is considered.
- `despawnAfterSeconds`: Time after last player leaves before despawning all.
- `spawnRadius`: Radius for random spawn positions (if no spawn points).
- `alignToGround`: Whether to raycast to ground for spawn placement.
- `groundOffset`: Height offset above ground after alignment.

## How It Works

- When a player enters the `activateRange`, the spawner begins spawning zombies up to `maxAlive`.
- If no players are within `deactivateRange` for `despawnAfterSeconds`, all zombies are despawned.
- Spawns are placed at random positions within `spawnRadius` or at specified spawn points, and aligned to ground if enabled.

## Extending

- Add custom spawn logic, pooling, or global caps as needed.
- For other enemy types, duplicate the spawner and assign different prefabs.
- For more advanced ground checks, add a LayerMask to the spawner.

## Troubleshooting

- **Enemies not spawning:**
	- Ensure your prefab has a `NetworkIdentity` and is registered in `NetworkManager`.
	- Check that your player is tagged `Player`.
- **Spawns not aligned to ground:**
	- Make sure `alignToGround` is enabled and your ground has colliders.
- **Performance:**
	- Use pooling for large numbers of spawners/enemies.

## License

MIT or project default.
