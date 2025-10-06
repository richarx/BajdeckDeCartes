using System;
using UnityEngine;

[Serializable]
public struct Log
{
    public LogType type;
    [TextArea(1, 3)] public string text;

    public Log(LogType type, string text) => (this.type, this.text) = (type, text);

    public void Deconstruct(out LogType type, out string text) => (type, text) = (this.type, this.text);
}
