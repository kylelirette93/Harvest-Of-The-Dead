using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject zombiePrefab;
    int randomSpawnIndex;
    Transform randomSpawnPoint;
    public float spawnDelay = 5;
    public float globalChaseSpeed = 2f;

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
    IEnumerator SpawnEnemy(float delay)
    {
        while (true)
        {
            GameObject newZombie = ObjectPool.sharedInstance.GetPooledObject();
            if (newZombie != null)
            {
                newZombie.transform.position = randomSpawnPoint.position;
                newZombie.SetActive(true);
            }
            yield return new WaitForSeconds(delay);
            RandomizeSpawnPoint();

            // Make zombie move faster whenever a zombie dies.
            Zombie zombieController = newZombie.GetComponent<Zombie>();

            if (zombieController != null)
            {
                zombieController.chaseSpeed = globalChaseSpeed;
                if (zombieController.chaseSpeed < 15)
                {
                    globalChaseSpeed += 0.1f;
                }
                else
                {
                    globalChaseSpeed = 15;
                }
            }

            // Zombie's spawn faster each time a zombie dies, capping at 1.5 seconds.
            if (spawnDelay > 1.5f)
            {
                spawnDelay -= 0.2f;
            }
            else
            {
                spawnDelay = 1.5f;
            }
            delay = Mathf.Clamp(spawnDelay, 1.5f, 5);
        }
    }
}
