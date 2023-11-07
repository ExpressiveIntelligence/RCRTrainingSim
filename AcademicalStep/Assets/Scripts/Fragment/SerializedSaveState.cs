using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;

[Serializable]
public class SerializedSaveState
{
    public string currentFragment;
    public State stepState;
    public SerializedCharacter[] characters;
}