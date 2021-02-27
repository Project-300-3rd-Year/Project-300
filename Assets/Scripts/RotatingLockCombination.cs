using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLockCombination : MonoBehaviour
{
    int numberAmount = 10;
    private LinkedList<Quaternion> combinationRotations = new LinkedList<Quaternion>();
    private LinkedListNode<Quaternion> combinationNode;
    [SerializeField] private float rotationAmountPerNumber = 37.1f;

    private void Start()
    {
        for (int i = 0; i < numberAmount; i++)
        {
            combinationRotations.AddLast(transform.rotation * Quaternion.Euler(0, rotationAmountPerNumber * i, 0));
        }

        combinationNode = combinationRotations.First;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            combinationNode = combinationNode.Previous;
            if (combinationNode == null)
                combinationNode = combinationRotations.Last;

            transform.rotation = combinationNode.Value;

        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            combinationNode = combinationNode.Next;
            if (combinationNode == null)
                combinationNode = combinationRotations.First;

            transform.rotation = combinationNode.Value;
        }
    }
}
