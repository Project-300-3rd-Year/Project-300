using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    //Moving to random pathnode
    public NavMeshAgent pawn;
    public Vector3 finalPosition;
    public float radius;
    

    //Enemy states
    public bool enganged;
    public bool calm;
    public bool searching;

    //Enemy looking around
    public float turnspeed;
    public Vector3[] random;
    public Transform[] transforms;
    public float waitTime;
    public float targetangle;

    //Movement alterations for states
    public float speed;
    Coroutine LookAround;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<NavMeshAgent>();
        RandomNavmeshLocation(radius);
        RandomLookPoints();
        
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!pawn.hasPath)
        {
            if (LookAround==null)
            {
                LookAround = StartCoroutine("LookingCoroutine");
            }

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
    
    IEnumerator LookingCoroutine()
    {
        print("OI");
            pawn.isStopped = true;
        if (pawn.isStopped)
        { 

        
        
            for (int i = 0; i < random.Length; i++)
            {
                Vector3 directionlook = (random[i] - transform.position).normalized;
                targetangle = 90 - Mathf.Atan2(directionlook.z, directionlook.x) * Mathf.Rad2Deg;

                while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y,targetangle))>0.2)
                {
                    print("rotating");
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(random[i].normalized), turnspeed * Time.deltaTime);
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, turnspeed * Time.deltaTime);
                    transform.eulerAngles = Vector3.up * angle;
                    yield return null;
                }
                
                yield return new WaitForSeconds(waitTime);
            }
            pawn.isStopped = false;
            RandomNavmeshLocation(radius);
            LookAround = null;

        }
            
            
                
    }
    public void RandomLookPoints()
    {

        random = new Vector3[3];
        for (int i = 0; i < random.Length; i++)
        {
            //random[i] = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
            random[i] = new Vector3(Random.insideUnitSphere.x*radius,Random.insideUnitSphere.y * radius, Random.insideUnitSphere.z * radius);
        }
    }



}

