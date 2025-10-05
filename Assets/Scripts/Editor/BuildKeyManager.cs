using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildKeyManager
{
    private const string ResourcePath = "Assets/Resources/build_key.asset";

    [InitializeOnLoadMethod]
    static void Ensure()
    {
        // try load existing
        var soAsset = AssetDatabase.LoadAssetAtPath<BuildKey>(ResourcePath);
        if (soAsset != null)
        {
            // check _stored via SerializedObject
            var so = new SerializedObject(soAsset);
            var prop = so.FindProperty("_stored");
            if (prop != null && !string.IsNullOrEmpty(prop.stringValue))
                return; // already has a value -> do nothing

            // set stored if empty or missing
            if (prop != null)
            {
                prop.stringValue = System.Guid.NewGuid().ToString("N");
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(soAsset);
                AssetDatabase.SaveAssets();
            }
            return;
        }

        // create directory & asset
        Directory.CreateDirectory("Assets/Resources");
        var asset = ScriptableObject.CreateInstance<BuildKey>();
        // set _stored via SerializedObject
        var newSo = new SerializedObject(asset);
        var newProp = newSo.FindProperty("_stored");
        if (newProp != null)
        {
            newProp.stringValue = System.Guid.NewGuid().ToString("N");
            newSo.ApplyModifiedProperties();
        }
        AssetDatabase.CreateAsset(asset, ResourcePath);
        EditorUtility.SetDirty(asset);
        AssetDatabase.SaveAssets();
    }
}

