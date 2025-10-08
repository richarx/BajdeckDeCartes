using System;
using UnityEngine;

[Serializable]
public abstract class SaveBase
{
    protected abstract string PrefKey { get; }

    protected virtual void OnSaveInitialization() { }

    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(PrefKey, json);
        PlayerPrefs.Save();
    }

    public void ClearPrefs()
    {
        PlayerPrefs.DeleteKey(PrefKey);
        PlayerPrefs.Save();
    }

    public static T Load<T>() where T : SaveBase, new()
    {
        T instance = new T();
        if (PlayerPrefs.HasKey(instance.PrefKey))
        {
            string json = PlayerPrefs.GetString(instance.PrefKey);
            JsonUtility.FromJsonOverwrite(json, instance);
        }
        else
        {
            instance.OnSaveInitialization();
        }
        return instance;
    }
}
