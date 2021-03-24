using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Script primarily for fixing issue where objects in drawers weren't moving with the drawers when you pull or move them - they would just stay in one place.
 * Any object that enters this scripts trigger gets parented to a target transform "transformToParentTo".
 * Any object that leaves this scripts trigger gets un-parented with the target transform.  
 * Objects that could get parented to the target transform could have children, so the "parentandchildrenstruct" below ensures that all children of the object also get parented to the correct transform. 
 */

public class ParentToTargetTrigger : MonoBehaviour
{
    [SerializeField] private Transform transformToParentTo;

    private void OnTriggerEnter(Collider colliderEnteringTrigger)
    {
        if (transformToParentTo != null && colliderEnteringTrigger.gameObject.tag != "InteractableArea")
        {
            MakeColliderParentOfTarget(colliderEnteringTrigger,target: transformToParentTo);
        }
    }

    private void OnTriggerExit(Collider colliderLeavingTrigger)
    {
        if (transformToParentTo != null && colliderLeavingTrigger.gameObject.tag != "InteractableArea")
        {
            MakeColliderParentOfTarget(collider: colliderLeavingTrigger,target: null);
        }
    }

    private void MakeColliderParentOfTarget(Collider collider,Transform target)
    {
        ParentChildTree parentChildTree = new ParentChildTree(new ParentChildNode(collider.gameObject.transform));
        parentChildTree.ParentRootToTarget(target);
    }
}

public struct ParentChildTree
{
    ParentChildNode rootNode;

    public ParentChildTree(ParentChildNode root)
    {
        this.rootNode = root;
    }

    public void ParentRootToTarget(Transform target)
    {
        rootNode.transform.SetParent(target);
        rootNode.ParentChildrenInHierarchy();
    }
}
public struct ParentChildNode
{
    public Transform transform;
    public List<ParentChildNode> childrenNodeList;

    public ParentChildNode(Transform transform)
    {
        this.transform = transform;
        childrenNodeList = new List<ParentChildNode>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            childrenNodeList.Add(new ParentChildNode(childTransform));
        }
    }

    public void ParentChildrenInHierarchy() //Name something better.
    {
        for (int i = 0; i < childrenNodeList.Count; i++)
        {
            childrenNodeList[i].transform.SetParent(transform);
            childrenNodeList[i].ParentChildrenInHierarchy();
        }
    }
}

