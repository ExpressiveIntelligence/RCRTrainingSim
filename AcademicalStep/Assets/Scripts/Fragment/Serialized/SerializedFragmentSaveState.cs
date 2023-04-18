using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedFragmentSaveState
{
    public string currentFragment;
    public SerializedCharacter[] characters;
    public Dictionary<string, object> stateVariables;
}
