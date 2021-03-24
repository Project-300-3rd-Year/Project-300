using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BreakableItem : MonoBehaviour
{
    //Components.
    Rigidbody rigidbody;

    [SerializeField] private GameObject[] possiblePrefabsToSpawn;
    [SerializeField] private float collisonVelocityToBreak;
    [SerializeField] private float breakablePrefabDissapearTime;

    private void Awake() => rigidbody = GetComponent<Rigidbody>();

    private void OnCollisionEnter(Collision collision)
    {
        //print($"Magnitude of collision: {collision.relativeVelocity.magnitude}");
        if(collision.relativeVelocity.magnitude >= collisonVelocityToBreak)
        {
            System.Random rng = new System.Random();

            Destroy(Instantiate(possiblePrefabsToSpawn[rng.Next(0, possiblePrefabsToSpawn.Length - 1)],transform.position,transform.rotation), breakablePrefabDissapearTime);
            Destroy(gameObject);
        }
    }
}
