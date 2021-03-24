using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Note",menuName ="ScriptableObjects/Note",order =1)]
public class Note : ScriptableObject
{
   public string date;
   public string text;
}
