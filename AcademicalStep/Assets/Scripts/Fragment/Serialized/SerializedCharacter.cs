using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedCharacter
{
    public string id;
    public string name;
    public string assetPath;
    public Tuple<int, int> position;
}
