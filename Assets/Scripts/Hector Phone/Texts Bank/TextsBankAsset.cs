using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextsBankAsset", menuName = "Scriptable Objects/Texts Bank Asset")]
public class TextsBankAsset : ScriptableObject
{
    [SerializeField, TextArea(1, 4)] List<string> texts;

    public int Count => texts.Count;

    public string this[int index] => texts[index];

    public string Random => texts[UnityEngine.Random.Range(0, texts.Count)];
}
