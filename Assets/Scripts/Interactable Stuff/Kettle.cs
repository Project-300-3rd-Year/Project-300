using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kettle : PlayerInteractableObject, iInteractable
{
    [Header("Particle System To Turn On")]
    [SerializeField] private ParticleSystem ParticleSystem;

    public bool IsInteractable { get { return ParticleSystem.isStopped == true; } set {} }

    //Start.
    public override void Awake()
    {
        base.Awake();
    }
    public override void Start()
    {
        base.Start();
    }

    //IInteractable.
    public void PlayerInteracted()
    {
        if(IsInteractable)
        {
            ParticleSystem.Play();
            UIManager.Instance.aimDot.Reset();
        }
    }
    public void PlayerLookedAtMe()
    {
        if(IsInteractable)
            UIManager.Instance.aimDot.ChangeToGreen();

    }
    public void PlayerLookedAwayFromMe()
    {
        if(IsInteractable)
            UIManager.Instance.aimDot.Reset();

    }
    public void PlayerStoppedInteraction()
    {

    }
}
