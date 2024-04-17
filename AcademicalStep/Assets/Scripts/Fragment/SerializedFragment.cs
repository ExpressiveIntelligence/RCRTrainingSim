using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializedFragment
{
    public string content;
    public string fragmentID;
    public SerializedChoice[] choices;
    public SerializedCharacter[] characters;
    public string speakerID;
    public string backgroundPath;
    public Dictionary<string, string> tags;
    public string timeRendered; //gives time the fragment was rendered
    public string systemMessage; // Error messages, etc.
    

    public override string ToString()
    {
        string temp = "";
        temp += "content: " + this.content + "\n";
        temp += "fragmentID: " + this.fragmentID + "\n";
        for (int i = 0; i < this.choices.Length; i++)
        {
            temp += "choice id " + i + ": " + this.choices[i].id + "\n";
            temp += "choice text " + i + ": " + this.choices[i].text + "\n";
        }
        for (int i = 0; i < this.characters.Length; i++)
        {
            temp += "character id " + i + ": " + this.characters[i].id + "\n";
            temp += "character name " + i + ": " + this.characters[i].name + "\n";
            temp += "character location " + i + ": " + this.characters[i].x + ", " + this.characters[i].y + "\n";
            foreach (var tag in this.characters[i].tags)
            {
                temp += "character tag " + i + ": " + tag.Key + ": " + tag.Value + "\n";
            }
        }
        temp += "tags:\n";
        foreach (var tag in this.tags)
        {
            temp += "  " + tag.Key + ": " + tag.Value + "\n";
        }
        temp += "speakerID: " + this.speakerID + "\n";
        temp += "backgroundPath: " + this.backgroundPath + "\n";
        temp += "systemMessage: " + this.systemMessage;

        return temp;
    }
}
