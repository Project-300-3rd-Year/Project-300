using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Path : MonoBehaviour
{

    public List<Vector3> List_Of_Nodes;
    public Vector3[] array_of_Nodes;
    // Start is called before the first frame update
    void Start()
    {
        List_Of_Nodes = new List<Vector3>();
        var transforms = gameObject.GetComponentsInChildren<Transform>(); 
        foreach (var item in transforms)
        {
            List_Of_Nodes.Add(item.position);
        }
        array_of_Nodes = List_Of_Nodes.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
