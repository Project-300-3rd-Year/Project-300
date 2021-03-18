using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AI_Movement_V2 : MonoBehaviour
{
    [Header("AI Misc")]
    private AudioSource audio_source;
    public bool heard;
    [SerializeField] private float step_lenght = 1;
    public Animator animator;
    public bool Ended;

    public enum EnemyState
    {
        engaged,
        calm,
        searching,
        heard,
        breaking_down_door
    }
    [Header("AI State")]
    public EnemyState current_state;
    private EnemyState last_state;

    [Header("Paths")]
    public List<GameObject> paths_list;
    public DoorHandle[] lockedDoors;

    [Header("Pathfinding")]
    public Vector3 last_node;    
    public Vector3 current_node;
    public Vector3[] array_of_nodes;
    public GameObject selected_path;
    public GameObject path_object;
    [SerializeField] private NavMeshAgent pawn_agent;    
    private Vector3 pawn_last_pos;

    [Header("Target Data/Detection")]
    public Light detectionSpotLight;
    private float detectionViewAngle;
    private float detectionViewDistance;
    public GameObject Player;    
    private Vector3 playerlastposition;
    public Vector3 heard_position;
    [SerializeField]private LayerMask ignore;
    [SerializeField]private LayerMask layersToBlock;
    RaycastHit hitInfo;    
    public float spotrange;

    //Breaking down door sequence.
    private Coroutine breakDownDoorCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        Ended = false;
        current_state = EnemyState.calm;
        heard = false;
        pawn_agent.isStopped = true;
        //paths_list = new List<GameObject>();
        audio_source = GetComponent<AudioSource>();
        //search_animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        pawn_last_pos = pawn_agent.transform.position;
        //GameObject[] PathArray = GameObject.FindGameObjectsWithTag("Path");

        detectionViewAngle = detectionSpotLight.spotAngle;
        detectionViewDistance = detectionSpotLight.range;


        //Subscribe to door unlock events to add paths that are behind the locked doors.
        for (int i = 0; i < lockedDoors.Length; i++)
        {
            lockedDoors[i].UnlockEvent += OnPlayerUnlockingDoor;
        }     
    }

    private void OnPlayerUnlockingDoor(DoorHandle doorUnlocked)
    {
        paths_list.AddRange(doorUnlocked.enemyPathsToActivateOnUnlocking);
    }

    // Update is called once per frame
    void Update()
    {
        StepSound();
        CheckForSpotted();
        //Debug.Log(paths_list.FindIndex(s => s == selected_path)+"Selected Path");        
        if (current_state==EnemyState.engaged)
        {
            Attack();
        }
        else if (current_state == EnemyState.calm)
        {
            Patrol();
        }
        else if (current_state == EnemyState.searching)
        {
            Search();
        }
        else if (current_state == EnemyState.heard)
        {
            MoveToNoise();
        } 
    }

    public void Attack()
    {
        pawn_agent.isStopped = false;
        last_state = EnemyState.engaged;
        //Debug.DrawLine(transform.position, playerlastposition,Color.red);
        pawn_agent.speed = 2f;        
        MoveTo(playerlastposition);

        if(Vector3.Distance(transform.position,Player.transform.position) <= 1.5f && CanSeePlayer())
        {
            GameManager.Instance.onGameEnd();
            pawn_agent.isStopped = true;
            UIManager.Instance.imgFadeToBlack.FadeToBlack(delegate ()
            {
                SceneManager.LoadScene(3);
                GameManager.Instance.currentGameSessionState = GameSessonState.GameOverLose;
            });
        }


        if (pawn_agent.remainingDistance<=0.1f)
        {
            current_state = EnemyState.searching;
        }
    }
    public void Patrol() 
    {
        last_state = EnemyState.calm;
        pawn_agent.speed = 1f;
        if (current_state==EnemyState.calm)
        {
            if (selected_path == null)
            {                
                selected_path = PickAPath(paths_list);
                array_of_nodes = selected_path.GetComponent<AI_Path>().array_of_Nodes;
                StartCoroutine(Set_Move_Path(array_of_nodes));
            }           
        }
    }
    public void MoveToNoise()
    {
        MoveTo(heard_position);
        if (Vector3.Distance(pawn_agent.transform.position, heard_position) <= 0.1f)
        {
            current_state = EnemyState.searching;
            heard = false;
        }
    }
    public void Search() 
    {
        animator.SetBool("Search", true);

        if(CanSeePlayer())
        {
            animator.SetBool("Search", false);
            playerlastposition = Player.transform.position;
            current_state = EnemyState.engaged;
        }

        else if (Ended)
        {     
            animator.SetBool("Search", false);
            pawn_agent.isStopped = true;
            current_state = EnemyState.calm;
            Ended = false;
        }
    }

    private bool HitObjectBlocksRaycast => (layersToBlock & 1 << hitInfo.collider.gameObject.layer) != 0;


    private bool CanSeePlayer()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < detectionViewDistance)
        {
            Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
            if (angleToPlayer < detectionViewAngle / 2f)
            {
                if (Physics.Raycast(transform.position, Player.transform.position - transform.position, out hitInfo, detectionViewDistance, ignore))
                {

                    if (HitObjectBlocksRaycast == false)
                    {
                        if(HidingSpot.IsInHiding == false)
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }


    public void CheckForSpotted() 
    {
        if(CanSeePlayer())
        {
            playerlastposition = Player.transform.position;
            current_state = EnemyState.engaged;
            StopAllCoroutines();
            selected_path = null;
        }
        else if (heard)
        {
            current_state = EnemyState.heard;
        }

        else if (last_state == EnemyState.searching && pawn_agent.isStopped == true)
        {
            current_state = EnemyState.calm;

        }


        //if (Physics.Raycast(transform.position, Player.transform.position - transform.position, out hitInfo, spotrange, ignore)
        //    &&
        //    (layersToBlock & 1 << hitInfo.collider.gameObject.layer) == 0)
        //    {
        //    playerlastposition = Player.transform.position;
        //    current_state = EnemyState.engaged;
        //    StopAllCoroutines();
        //    selected_path = null;
        //    }
        //else if (heard)
        //{
        //    current_state = EnemyState.heard;
        //}
        
        //else if(last_state==EnemyState.searching&&pawn_agent.isStopped==true) 
        //{
        //    current_state = EnemyState.calm;
            
        //}
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag=="Door")
        {
            BreakDownDoor breakDownDoor;
            collision.gameObject.TryGetComponent(out breakDownDoor);

            if(breakDownDoor)
            {
                if (breakDownDoorCoroutine == null)
                    breakDownDoorCoroutine = StartCoroutine(BreakDownDoor(breakDownDoor));
            }
        }

    }

    private IEnumerator BreakDownDoor(BreakDownDoor breakDownDoor)
    {
        pawn_agent.isStopped = true;
        yield return StartCoroutine(breakDownDoor.StartBreakingDownSequence(this));
        pawn_agent.isStopped = false;
        breakDownDoorCoroutine = null;
    }

    public void StepSound() 
    {
        Vector3 step_dis = pawn_agent.transform.position - pawn_last_pos;
        if (Vector3.Distance(pawn_agent.transform.position, pawn_last_pos) >= step_lenght)
        {
            audio_source.Play();
            pawn_last_pos = pawn_agent.transform.position;
        }
    }
    IEnumerator Set_Move_Path(Vector3[] nodes)
    {        
        int last_index = nodes.Length - 1;
        pawn_agent.isStopped = false;
        for (int i = 0; i < nodes.Length; )
        {
            if (!pawn_agent.isStopped)
            {
                current_node = nodes[i];
                last_node = current_node;                
                MoveTo(current_node);
                yield return new WaitWhile(()=>pawn_agent.remainingDistance >= 0.5f);                
                pawn_agent.isStopped = true;
                i++;
                if (current_node == nodes[last_index])
                {
                    
                    pawn_agent.isStopped = true;
                    selected_path = null;
                    StopCoroutine(Set_Move_Path(array_of_nodes));

                }
            }
            else pawn_agent.isStopped = false;
        }  
    }
    public void MoveTo(Vector3 target) 
    {
        pawn_agent.destination=target;        
    }
  
    public GameObject PickAPath(List<GameObject> paths) 
    {
        int selected_path_index = Random.Range(0, paths.Count);
        
        foreach (var Path in paths)
        {
            if (paths.FindIndex(p=>p==Path)==selected_path_index)
            {                 
                return Path;
            }
        }
        return null;
    }
    public void AnimationEnded() 
    {
        Ended = true;
    }
}
