using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Each pickable key object has a reference to this KeyInventoryItem.
 * This KeyInventoryItem gets added to player inventory.
 * I have a field for UI message to show as notification message is subscribed to picking up key event which passes over keyinventoryitem as the delegate.
 * Might change that later.
 */

[CreateAssetMenu(fileName = "KeyInventoryItem", menuName = "ScriptableObjects/KeyInventoryItem", order = 2)]
public class KeyInventoryItem : ScriptableObject
{
    public string keyName;
    public string UIMessageToShowWhenPickedUp;
}
