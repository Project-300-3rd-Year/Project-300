using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Suitcase which opens when padlock is unlocked - listens for padlock unlock event.
// Only set up for one padlock / suitcase - ran out of time so couldn't expand on this further.
// Front door key has a chance to spawn inside the suitcase but not guarenteed.

[RequireComponent(typeof(Animator))]
public class Suitcase : MonoBehaviour
{
    //Componenents.
    private Animator animator;

    private void Awake() => animator = GetComponent<Animator>();
    private void Start() => FindObjectOfType<Padlock>().unlockedEvent += () => animator.SetBool("Open", true);
}
