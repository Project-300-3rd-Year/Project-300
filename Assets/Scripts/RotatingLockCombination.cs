using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotatingLockCombination : MonoBehaviour
{
    public int CurrentNumber { get { return combinationRotations.TakeWhile(q => q != combinationNode.Value).Count(); } }

    int numberAmount = 10;
    [SerializeField] private float rotationAmountPerNumber = 37.1f;
    private LinkedList<Quaternion> combinationRotations = new LinkedList<Quaternion>();
    private LinkedListNode<Quaternion> combinationNode;

    private void Start()
    {
        for (int i = 0; i < numberAmount; i++)
        {
            combinationRotations.AddLast(transform.rotation * Quaternion.Euler(0, rotationAmountPerNumber * i, 0));
        }

        combinationNode = combinationRotations.First;

    }

    public void RotateRight()
    {
        combinationNode = combinationNode.Next;
        if (combinationNode == null)
            combinationNode = combinationRotations.First;

        transform.rotation = combinationNode.Value;
    }
    public void RotateLeft()
    {
        combinationNode = combinationNode.Previous;
        if (combinationNode == null)
            combinationNode = combinationRotations.Last;

        transform.rotation = combinationNode.Value;
    }

}
