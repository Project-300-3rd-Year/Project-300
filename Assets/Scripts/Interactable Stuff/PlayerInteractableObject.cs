using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractableObject : PlayerInteractableComponent
{
    [Header("UI")]
    [SerializeField] protected Sprite UIImageToShowIfPlayerLooksAtMe;
}
