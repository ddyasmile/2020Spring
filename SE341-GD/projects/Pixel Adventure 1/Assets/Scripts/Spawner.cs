using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> platforms = new List<GameObject>();

    public float spawnTime;
    private float countTime;
    private Vector3 spawnPosition;

    public float xSpawnRange;

    // Update is called once per frame
    void Update()
    {
        SpawnPlatform();
    }

    public void SpawnPlatform()
    {
        countTime += Time.deltaTime;
        spawnPosition = transform.position;
        spawnPosition.x = Random.Range(-xSpawnRange, xSpawnRange);

        if (countTime >= spawnTime)
        {
            CreatePlatform();
            countTime = 0;
        }
    }

    public void CreatePlatform()
    {
        int index = Random.Range(0, platforms.Count);
        GameObject platformPrototype = platforms[index];

        int spikeNum = 0;
        if (platformPrototype.CompareTag("Spike"))
        {
            spikeNum++;
        }
        if (spikeNum > 1)
        {
            spikeNum = 0;
            countTime = spawnTime;
            GameObject newTmpPlatform = Instantiate(platforms[0], spawnPosition, Quaternion.identity);
            newTmpPlatform.transform.SetParent(this.gameObject.transform);
            return;
        }

        GameObject newPlatform = Instantiate(platformPrototype, spawnPosition, Quaternion.identity);
        newPlatform.transform.SetParent(this.gameObject.transform);
    }
}
