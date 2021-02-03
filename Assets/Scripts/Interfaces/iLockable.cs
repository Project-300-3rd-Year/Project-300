using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * 
 * 
 */

public interface iLockable 
{
    bool IsLocked { get; set; }
    KeyInventoryItem KeyToUnlockMe { get; }
    KeyCode KeyCodeToUnlockMe { get; }
    Sprite UnlockSprite { get; }

    void Unlock();
    void Lock();
}
