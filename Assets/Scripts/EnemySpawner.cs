using UnityEngine;
using System.Collections; // Required for Coroutines

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;       // The prefab to spawn
    public float spawnInterval = 3f;     // Time between spawns in seconds
    public float activationDistance = 40f; // Distance at which spawner wakes up and starts spawning
    
    [Header("Wave Settings")]
    public int maxEnemies = 25;          // Maximum number of enemies to spawn in total
    private int enemiesSpawned = 0;      // How many we have spawned so far

    private Transform player;            // Reference to the player's position
    private bool isSpawning = false;     // Keeps track if the spawner is currently active
    private Camera mainCamera;              // Reference to the main camera for potential future use

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera at the start of the level
        // Find the player at the start of the level
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) 
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // If the player exists, we haven't started spawning, and we still have enemies to spawn
        if (player != null && !isSpawning && enemiesSpawned < maxEnemies)
        {
            // Calculate the distance between the spawner and the player
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            
            // If the player gets close enough, activate the spawner!
            if (distanceToPlayer <= activationDistance)
            {
                isSpawning = true;
                StartCoroutine(SpawnEnemyRoutine());
            }
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        // Loop ONLY until we reach the maximum number of enemies
        while (enemiesSpawned < maxEnemies) 
        {
            while(IsVisibleToCamera())
            {
                yield return new WaitForSeconds(0.5f); // Wait until the spawner is off-screen before spawning the next enemy
            }
            // Create a new enemy at the spawner's exact position
            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                
                // Increase the counter by 1 every time we spawn an enemy
                enemiesSpawned++; 
            }

            // Wait for the specified time BEFORE spawning the next one
            yield return new WaitForSeconds(spawnInterval);
        }
        
        // When the loop finishes, print a message to the console
        Debug.Log("Spawner finished! All " + maxEnemies + " enemies have been deployed.");
    }
    bool IsVisibleToCamera()
    {
        if (mainCamera == null) return false;

        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1;
    }
    
    // Draw a green circle in the editor to easily see the activation range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}