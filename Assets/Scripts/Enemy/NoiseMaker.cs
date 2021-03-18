using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    private float radius=10;
    private RaycastHit HitInfo;
    private LayerMask ignore;
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
        audioSource.clip = audioClip;
        audioSource.Play();


        if (Physics.SphereCast(transform.position, radius*speed,transform.position+new Vector3(0,-2,0),out HitInfo,ignore))
        {
            AI = HitInfo.collider.gameObject.GetComponent<AI_Movement_V2>();
            AI.heard = true;
            AI.heard_position = transform.position;
        }
        
    }
}
