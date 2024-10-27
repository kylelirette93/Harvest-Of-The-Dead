using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject zombiePrefab;
    int randomSpawnIndex;
    Transform randomSpawnPoint;
    int spawnDelay = 5;

    void Start()
    {
        RandomizeSpawnPoint();
        StartCoroutine("SpawnEnemy", spawnDelay);
    }

    void RandomizeSpawnPoint()
    {
        randomSpawnIndex = Random.Range(0, spawnPoints.Length);
        randomSpawnPoint = spawnPoints[randomSpawnIndex];
    }
    IEnumerator SpawnEnemy(int delay)
    {
        while (true)
        {
            GameObject newZombie = Instantiate(zombiePrefab, randomSpawnPoint.position, zombiePrefab.transform.rotation);
            yield return new WaitForSeconds(delay);
            RandomizeSpawnPoint();
        }
    }
}
