using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iInteractable
{
    //event Action InteractedEvent;

    bool IsInteractable { get; set; }

    void PlayerInteracted();
    void PlayerStoppedInteraction();
    void PlayerLookedAwayFromMe();
    void PlayerLookedAtMe();
}
