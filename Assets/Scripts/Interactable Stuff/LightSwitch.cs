using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : PlayerInteractableObject, iInteractable
{
    public bool IsInteractable {
        get
        {
            return true;
        }
        set
        {
            _IsInteractable = value;
        }
    }

    public event Action InteractedEvent;

    [Header("Status")]
    [SerializeField] private bool _switchedOn;
    private bool SwitchedOn
    {
        get
        {
            return lightSource.enabled == true;
        }
    }

    [Header("Light That Is Effected")]
    [SerializeField] private Light lightSource;
    private FlickeringLight flickeringLight;
    private Action TurnOnLight;
    private Action TurnOffLight;

    //Start.
    public override void Awake() => base.Awake();
    public override void Start()
    {
        base.Start();

        TurnOnLight += EnableLightSource;
        TurnOffLight += DisableLightSource;

        lightSource.TryGetComponent(out flickeringLight); //Is there a flickering light component? 
                                                         
        if(flickeringLight)                               //If so, add enable, disable methods to the delegate.
        {
            TurnOnLight += EnableFlickeringLight;
            TurnOffLight += DisableFlickeringLight;
        }
    }

    private void EnableLightSource() => lightSource.enabled = true;
    private void DisableLightSource() => lightSource.enabled = false;
    private void EnableFlickeringLight() => flickeringLight.enabled = true;
    private void DisableFlickeringLight() => flickeringLight.enabled = false;

    //IInteractable.
    public void PlayerInteracted()
    {
        if (SwitchedOn)
            TurnOffLight();
        else
            TurnOnLight();
    }

    //IInteractable.
    public void PlayerLookedAtMe()
    {
        AimDotUI.Instance.ChangeAimDotToGreen();
    }
    public void PlayerLookedAwayFromMe()
    {
        AimDotUI.Instance.ChangeAimDotBackToNormal();
    }
    public void PlayerStoppedInteraction()
    {
        
    }
    public void PlayerIsLookingAtMe()
    {

    }
}
