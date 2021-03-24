using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<KeyInventoryItem> keys;

    private void Start()
    {
        //Subscribe events. Spawning in the keys or inventory items in general should happen in awake or just before this gets called.
        Key[] keysInWorld = FindObjectsOfType<Key>(); 
        for (int i = 0; i < keysInWorld.Length; i++)
        {
            keysInWorld[i].PickedUpKeyEvent += AddKeyToInventory;
        }
    }

    //Keys.
    private void AddKeyToInventory(KeyInventoryItem keyToAdd)
    {
        print($"added {keyToAdd.keyName} key to inventory");
        keys.Add(keyToAdd);
    }
    public bool HasKeyInInventory(KeyInventoryItem keyToCheckFor) => keys.Contains(keyToCheckFor);

}
