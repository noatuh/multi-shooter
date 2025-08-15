# Multi-Shooter Game

A multiplayer survival game built using Unity and Mirror. Players must survive in a hostile environment filled with zombies, gather resources, and collaborate or compete with other players.

## Features

- **Multiplayer Gameplay:** Built on Mirror for seamless networked experiences.
- **Zombie AI:** Dynamic, proximity-based spawning and despawning of zombies.
- **Resource Gathering:** Collect resources to craft items and survive.
- **Base Building:** Construct shelters to protect yourself from enemies.
- **Weapons and Combat:** A variety of weapons to defend against zombies and other players.
- **Customizable Settings:** Adjust game parameters like zombie spawn rates, player health, and more.

## Setup

1. **Clone the Repository:**

   ```bash
   git clone <repository-url>
   ```

2. **Unity Version:**

   - Open the project in Unity 2021.3 or later.

3. **Install Dependencies:**

   - Ensure Mirror is installed via Unity Package Manager.

4. **Build Settings:**

   - Set the build target to your desired platform (e.g., Windows, Mac, Linux).

5. **Network Configuration:**

   - Configure the `NetworkManager` prefab in the scene.

6. **Player Prefab:**

   - Ensure the player prefab is assigned in the `NetworkManager` and tagged as `Player`.

## Gameplay Mechanics

### Zombie Spawner System

- **Proximity-based Spawning:** Zombies spawn near players and despawn when players leave the area.

- **Configurable Parameters:** Adjust spawn radius, activation range, and more via the `EnemySpawner` script.

### Resource Gathering

- **Collectibles:** Resources like wood, stone, and metal can be gathered from the environment.

- **Crafting:** Use resources to craft weapons, tools, and building materials.

### Base Building

- **Construction:** Build walls, doors, and other structures to create a safe base.

- **Upgrades:** Strengthen your base with better materials.

### Weapons and Combat

- **Weapon Types:** Includes melee weapons, firearms, and explosives.

- **Combat System:** Engage in combat with zombies and other players.

## Inspector Fields

### EnemySpawner

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

## Troubleshooting

- **Zombies Not Spawning:**

  - Ensure the zombie prefab has a `NetworkIdentity` and is registered in the `NetworkManager`.

  - Check that the player prefab is tagged `Player`.

- **Performance Issues:**

  - Use object pooling for large numbers of zombies or spawners.

- **Network Issues:**

  - Verify that the `NetworkManager` is correctly configured.

## License

MIT or project default.
