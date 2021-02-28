using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//Set to zero at start all the time.
public class RotatingLockCombination : MonoBehaviour
{
    public int CurrentNumber { get { return combinationRotations.TakeWhile(q => q != combinationNode.Value).Count(); } }

    public int numberAmount = 10;
    [SerializeField] private float rotationAmountPerNumber = 37.1f;
    private LinkedList<Quaternion> combinationRotations = new LinkedList<Quaternion>();
    private LinkedListNode<Quaternion> combinationNode;

    //Material.
    private Renderer renderer;
    private Material defaultMaterial;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        defaultMaterial = renderer.material;
    }

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

    public void ResetMaterial() => renderer.material = defaultMaterial;
    public void AssignNewMaterial(Material newMaterial) => renderer.material = newMaterial;

}
