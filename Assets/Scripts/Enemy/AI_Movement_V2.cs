using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement_V2 : MonoBehaviour
{
    public enum EnemyState
    {
        engaged,
        calm,
        searching
    }
    public EnemyState current_state;
    public GameObject path_object;    
    [SerializeField]private NavMeshAgent pawn_agent;
    private List<GameObject> paths_list;
    [SerializeField] private float step_lenght = 1;
    private Vector3 pawn_last_pos;
    private AudioSource audio_source;
    private int random_path;
    private AI_Path ai_path;
    [Header("MoveMethod")]
    public Vector3 last_node;
    //public Vector3 next_node;
    public Vector3 current_node;
    public Vector3[] array_of_nodes;


    // Start is called before the first frame update
    void Start()
    {
        ai_path = GetComponent<AI_Path>();
        paths_list = new List<GameObject>();
        pawn_last_pos = pawn_agent.transform.position;
        audio_source = GetComponent<AudioSource>();
        GameObject[] PathArray = GameObject.FindGameObjectsWithTag("Path");
        if (PathArray!=null)
        {
            for (int i = 0; i < PathArray.Length; i++)
            {
                paths_list.Add(PathArray[i]);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Defining step and audio of the steps
        Vector3 step_dis = pawn_agent.transform.position - pawn_last_pos;        
        if (Vector3.Distance(pawn_agent.transform.position,pawn_last_pos)>=step_lenght)
        {
            audio_source.Play();
            pawn_last_pos = pawn_agent.transform.position;
        }
        //Switch statement for enemy states(WIP)
        switch (current_state)
        {
            case EnemyState.engaged:
                break;
            case EnemyState.calm:
                Patrol();
                break;
            case EnemyState.searching:
                break;
            default:
                break;
        }
    }

    public void Patrol() 
    {
        if (current_state==EnemyState.calm&&pawn_agent.hasPath==false)
        {
            if (random_path==0)
            {
                random_path = Random.Range(0, paths_list.Count);
            }
            
            for (int i = 0; i < paths_list.Count; i++)
            {
                    NavMeshPath path = new NavMeshPath();
                    array_of_nodes = paths_list[random_path].GetComponent<AI_Path>().array_of_Nodes;
                    path.GetCornersNonAlloc(array_of_nodes);
                    set_move_path(array_of_nodes);
                    
                
                
                //StartCoroutine(Set_Move_Path(array_of_nodes));
            }
        }
    }
    public void set_move_path(Vector3[] nodes)
    {
        //Setting order in which to follow the path
        for (int i = 0; i < nodes.Length; i++)
        {
            int last_index = nodes.Length - 1;
            if (pawn_agent.isStopped)
            {
                current_node = nodes[i];
                last_node = current_node;
                pawn_agent.isStopped = false;
            }
            else if (i > last_index)
            {
                i = 0;//Temp fix(Will call "Look_For_New_Path" method)                
            }
            else if (Vector3.Distance(transform.position, current_node) <= 0.5f)
            {
                i++;
            }

        }
        MoveTo(current_node);
    }


    IEnumerator Set_Move_Path(Vector3[] nodes)
    {

        for (int i = 0; i < nodes.Length; i++)
        {
            int last_index = nodes.Length - 1;
            if (pawn_agent.isStopped)
            {
                current_node = nodes[i];
                last_node = current_node;
                pawn_agent.isStopped = false;
            }
            else if (i > last_index)
            {

                StopCoroutine("Set_Move_Path");
            }
            else if (Vector3.Distance(transform.position, current_node) <= 0.5f)
            {
                i++;
            }

        }
        MoveTo(current_node);

        yield return new WaitForSeconds(0.1f);
    }
    public void MoveTo(Vector3 target) 
        {
        pawn_agent.destination=target;
        Debug.Log(pawn_agent.hasPath);
        }
}
