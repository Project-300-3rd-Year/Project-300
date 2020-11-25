using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    public Vector3 finalPosition;    
    public float radius;
    public float turnangle;
    public NavMeshAgent pawn;
    

    //Enemy states
    public bool enganged;
    public bool calm;
    public bool searching;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<NavMeshAgent>();
        
        

        RandomNavmeshLocation(radius);
        
        
        

    }

    // Update is called once per frame
    void Update()
    {

        if (!pawn.hasPath)
        {
            
            RandomNavmeshLocation(radius);
        }
        
    }
    
    public Vector3 RandomNavmeshLocation(float radius)
    {
        
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;        
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        pawn.destination = finalPosition;
        return finalPosition;
    }
    
}

