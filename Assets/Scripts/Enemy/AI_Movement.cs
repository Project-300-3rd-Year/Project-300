using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    public enum EnemyState
    {
        enagaged,
        calm,
        searching
    }

    [Header("Pathnode movement")]
    private NavMeshAgent pawn;
    [SerializeField] private float radius;
    [SerializeField] private int randompathnumber;
    public AI_PathNode CurrentNode;
    public AI_PathNode LastNode;
    public AI_PathNode[] Nodes;
    public float speed;

    [Header("Target Data/Detection")]
    [SerializeField] private Vector3 target;
    [SerializeField] private Vector3 playerlastposition;
    [SerializeField] private LayerMask ignore;
    [SerializeField] private LayerMask layersToBlock;
    RaycastHit hitInfo;

    [SerializeField] private float spotrange;
    protected Transform player;
    public Collider enemycollider;
    public EnemyState current_state;
    Coroutine LookAround;

    [Header("Looking Around")]
    public float turnspeed;
    private Vector3[] random;
    public float waitTime;
    private float targetangle;
    
    // Start is called before the first frame update
    void Start()
    {
        current_state = EnemyState.calm;
        pawn = GetComponent<NavMeshAgent>();        
        RandomLookPoints();
        randompathnumber = Random.Range(0, Nodes.Length);
        PickRandomPath(randompathnumber);        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //enemycollider = GetComponent<CapsuleCollider>();
        LookAround = null;
        
    }

    // Update is called once per frame
    void Update()
    {   //Setting Path if pawn has no path
        if (current_state==EnemyState.calm)
        {
            pawn.speed = 1.5f;
            PawnMoveToRandomPath();
            CheckIfSpotted();
            CheckIfLostSight();

        }
        else if (current_state==EnemyState.enagaged)
        {
            Engage(enemycollider);
        }
        else if (current_state==EnemyState.searching)
        {
            
            MoveTo(playerlastposition);

            if (LookAround == null && Vector3.Distance(transform.position,playerlastposition)<=2f)
            {
                LookAround = StartCoroutine(LookingCoroutine());
            }

        }
        
        

    }

    private void CheckIfLostSight()
    {
        if (Vector3.Distance(transform.position, player.transform.position) >= spotrange && !Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, Mathf.Infinity, ignore) && (layersToBlock & 1 << hitInfo.collider.gameObject.layer) == 0)
            
                current_state = EnemyState.searching;            
        
    }

    private void CheckIfSpotted()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= spotrange && Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, spotrange, ignore)&& (layersToBlock & 1 << hitInfo.collider.gameObject.layer) == 0)
        {
            pawn.speed = 2.8f;
            
                current_state = EnemyState.enagaged;            
                print("CheckIfSpotted");
            
                
        }
    }

    private void PawnMoveToRandomPath()
    {
        if (Vector3.Distance(pawn.pathEndPosition, transform.position) <= 1f || Vector3.Distance(playerlastposition,transform.position)<=1f&&CurrentNode==null)
        {
            randompathnumber = Random.Range(0, Nodes.Length);
            PickRandomPath(randompathnumber);
        }
     }

    IEnumerator LookingCoroutine()
    {
        print("Looking");
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
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetangle, turnspeed * Time.deltaTime);
                    transform.eulerAngles = Vector3.up * angle;
                    if (Vector3.Distance(transform.position, player.transform.position) <= spotrange && Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, Mathf.Infinity, ignore) && (layersToBlock & 1 << hitInfo.collider.gameObject.layer) == 0)
                    {
                        current_state = EnemyState.enagaged;
                        pawn.isStopped = false;
                        LookAround = null;
                        CurrentNode = null;
                        StopCoroutine(LookingCoroutine());
                        
                    }
                    yield return null;

                }       

                             
                yield return new WaitForSeconds(waitTime);
            }
            LookAround = null;
            CurrentNode = null;

            pawn.isStopped = false;
            current_state = EnemyState.calm;
            StopCoroutine(LookingCoroutine());


        }
            
            
                
    }
    
    public void Engage(Collider collider)
    {


        if (Physics.Raycast(transform.position, player.position - transform.position, out hitInfo, spotrange, ignore)&& (layersToBlock & 1 << hitInfo.collider.gameObject.layer) == 0)
        {
            
            
                if (Vector3.Distance(transform.position, player.transform.position) <= spotrange)
                {
                    Debug.DrawRay(transform.position, player.position - transform.position * 1000, Color.red, 2000);
                    
                    
                    MoveTo(player.position);
                
                    print("player hit");
                    playerlastposition = player.position;
                }           
            


        }
        else
        {
            print("player lost");
            
            current_state = EnemyState.searching;
        }
        
    }    
    public void RandomLookPoints()
    {

        random = new Vector3[3];
        for (int i = 0; i < random.Length; i++)
        {           
            random[i] = new Vector3(Random.insideUnitSphere.x*radius,Random.insideUnitSphere.y * radius, Random.insideUnitSphere.z * radius);
        }

    }
    public void PickRandomPath(int PathNumber)
    {

        for (int i = 0; i < Nodes.Length; i++)
        {
            CurrentNode = Nodes[PathNumber];
            if (i==PathNumber)
            {
                if (CurrentNode != LastNode)
                {
                    MoveToPathNode(CurrentNode);
                    LastNode = CurrentNode;
                }
            }
            
        }

    }

    public void MoveToPathNode(AI_PathNode node)
    {
        CurrentNode=node;
        MoveTo(CurrentNode.transform.position);
        
    }
    public virtual void MoveTo(Vector3 targetPosition)
    {
        target = targetPosition;
        pawn.destination=target;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag=="Player")
        {
            print("Collided");
        }
    }
          }

