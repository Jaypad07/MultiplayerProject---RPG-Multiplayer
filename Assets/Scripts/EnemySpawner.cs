using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviourPun
{
    public string enemyPrefabPath;
    public float maxEnemies;
    public float spawnRadius;
    public float spawnCheckTime;

    private float lastSpawnCheckTime;
    private List<GameObject> curEnemies = new List<GameObject>();

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (Time.time - lastSpawnCheckTime > spawnCheckTime)
        {
            lastSpawnCheckTime = Time.time;
            TrySpawn();
        }
    }

    void TrySpawn()
    {
        // Remove any dead enemies from the curEnemies list
        for (int i = 0; i < curEnemies.Count; i++)
        {
            if (!curEnemies[i])
            {
                curEnemies.RemoveAt(i);
            }
        }
        
        // If we have maxed out our enemies, return
        if (curEnemies.Count >= maxEnemies)
        {
            return;
        }
        
        // Otherwise, spawn an enemy
        Vector3 randomInCircle = Random.insideUnitCircle * spawnRadius;
        GameObject enemy = PhotonNetwork.Instantiate(enemyPrefabPath, transform.position + randomInCircle, Quaternion.identity);
        curEnemies.Add(enemy);
    }
}
