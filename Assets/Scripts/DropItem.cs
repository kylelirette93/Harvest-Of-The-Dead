using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public GameObject[] items;
    public struct EnemyDrop
    {
        public string Name;
        public GameObject item;
        public float weight;
    }

    private List<EnemyDrop> enemyDrops;
    void Start()
    {
        // Initialize the enemyDrops list after items have been assigned
        enemyDrops = new List<EnemyDrop>
        {
            new EnemyDrop { Name = "Money", item = items[0], weight = 0.7f },
            new EnemyDrop { Name = "Health", item = items[1], weight = 0.3f }
        };
    }


    public GameObject GetRandomDrop()
    {
        float totalWeight = 0f;
        foreach (var drop in enemyDrops)
        {
            totalWeight += drop.weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var drop in enemyDrops)
        {
            cumulativeWeight += drop.weight;
            if (randomValue < cumulativeWeight)
            {
                return drop.item;
            }
        }
        return null;
    }
}
