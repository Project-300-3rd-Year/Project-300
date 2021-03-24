using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This should have just been a component. Wanted to change it but ran out of time.
 * All "doorhandles" implement this but not all of them are lockable so it should have been an optional component.
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
