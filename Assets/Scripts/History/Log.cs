using System;

[Serializable]
public struct Log
{
    public LogType type;
    public string text;

    public Log(LogType type, string text) => (this.type, this.text) = (type, text);

    public void Deconstruct(out LogType type, out string text) => (type, text) = (this.type, this.text);
}
