using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iHideable 
{
    void OnEnteringHidingSpot();
    void OnReachingHidingSpot();
    void OnLeavingHidingSpot();
    void OnLeftHidingSpot();
}
