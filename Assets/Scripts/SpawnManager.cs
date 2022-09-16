using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] float spawnRadius;
    [SerializeField] float spawnHeight;
    [SerializeField] int spawnCount;
    [SerializeField] float startTime;
    [SerializeField] float cooldown;
    bool canSpawn = false;

    [SerializeField] GameObject[] enemies;

    private void Start()
    {
        Invoke(nameof(SpawnEnemies), startTime);
    }

    void Update()
    {
        int enemiesLeft = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemiesLeft == 0 && canSpawn) {
            Invoke(nameof(SpawnEnemies), cooldown);
            canSpawn = false;
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++) {
            GameObject enemy = enemies[Random.Range(0, enemies.Length)];
            Instantiate(enemy, GetRandomPos(), Quaternion.identity, GameObject.Find("Enemies").transform);        
        }
        canSpawn = true;
    }

    Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(-spawnRadius, spawnRadius), spawnHeight, Random.Range(-spawnRadius, spawnRadius));
    }
}
