using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomKeyItemSpawns : MonoBehaviour
{
    [SerializeField] private Key keyToSpawn; //Make into array later on obviously and change all of this. Jagged array maybe?
    [SerializeField] private Transform[] randomKeyTransforms;

    private void Start()
    {
        Transform randomTransform = randomKeyTransforms[Random.Range(0, randomKeyTransforms.Length - 1)];
        Instantiate(keyToSpawn.gameObject, randomTransform.transform.position, randomTransform.rotation);
    }
}
