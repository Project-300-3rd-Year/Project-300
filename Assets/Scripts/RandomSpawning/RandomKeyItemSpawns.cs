using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKeyItemSpawns : MonoBehaviour
{
    [SerializeField] private KeyInventoryItem[] allKeyInventoryItems;
    [SerializeField] private Transform[] keySpawnPositionsHolder; //Get the spawn positions from all the children of this object.

    private void Awake()
    {
        SpawnAllKeyItems();
    }

    private void SpawnAllKeyItems()
    {
        for (int k = 0; k < allKeyInventoryItems.Length; k++)
        {
            List<Transform> spawnPositions = new List<Transform>();
            for (int i = 0; i < keySpawnPositionsHolder[k].childCount; i++)
            {
                spawnPositions.Add(keySpawnPositionsHolder[k].GetChild(i));
            }

            //Transform randomTransform = spawnPositions[UnityEngine.Random.Range(0, spawnPositions.Count)];
            Transform randomTransform = spawnPositions[spawnPositions.Count - 1];
            GameObject go = Instantiate(allKeyInventoryItems[k].keyPrefab.gameObject, randomTransform.transform.position, randomTransform.rotation);
            go.transform.SetParent(keySpawnPositionsHolder[k]);
        }
    }
}