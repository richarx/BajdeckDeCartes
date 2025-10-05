using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ScriptsListAsset), menuName = "SO/" + nameof(ScriptsListAsset))]
public class ScriptsListAsset : ScriptableObject
{
    [SerializeField, TextArea] List<string> texts;

    public string RandomText => texts[Random.Range(0, texts.Count)];
}
