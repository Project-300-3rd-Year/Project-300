using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_PathNode : MonoBehaviour
{
    public AI_PathNode Node;
    public Color debugColor = Color.red;
    private void OnDrawGizmos()
    {
        if (Node!=null)
        {
            Gizmos.color = debugColor;
            Gizmos.DrawLine(transform.position, Node.transform.position);

            Vector3 direction = Node.transform.position - transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, (transform.position + direction * 0.5f));

        }
    }
}
