using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class NoiseMaker : MonoBehaviour
{
    private float radius=1f;
    private RaycastHit HitInfo;
    public LayerMask layerMask;
    private AI_Movement_V2 AI;
    private AudioSource audioSource;
    public AudioClip audioClip;
    public void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }


    public void MakeNoise(float speed) 
    {
        AudioSource.PlayClipAtPoint(audioClip, transform.position);
    
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius * speed, layerMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < colliders.Length; i++)
        {
            AI = colliders[i].gameObject.GetComponent<AI_Movement_V2>();
            if (AI != null)
            {
                AI.heard = true;
                AI.heard_position = transform.position;
            }
        }
    }
}
