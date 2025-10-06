using System;
using System.Collections.Generic;
using UnityEngine;

public static class History
{
    const string PlayerPrefsKey = "HistoryLogsArray";

    [Serializable]
    class WrappedLogs
    {
        public Log[] array;

        public WrappedLogs() => array = logs.ToArray();

        public void Load() => logs.AddRange(array);
    }

    static List<Log> logs = new();

    public static void CleanupLogs()
    {
        logs = new();
    }

#pragma warning disable IDE1006
    static event Action<LogType, string> _onNewLogWithCatchUp;
#pragma warning restore IDE1006

    public static event Action<LogType, string> OnNewLogWithCatchUp
    {
        add
        {
            foreach ((LogType type, string text) in logs)
                value?.Invoke(type, text);

            _onNewLogWithCatchUp += value;
        }
        remove
        {
            _onNewLogWithCatchUp -= value;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void LoadPreviousLogs()
    {
        CleanupLogs();
        
        if (PlayerPrefs.HasKey(PlayerPrefsKey) == false)
            return;

        string json = PlayerPrefs.GetString(PlayerPrefsKey);

        JsonUtility.FromJson<WrappedLogs>(json)
                   .Load();
    }

    public static void Log(LogType type, string text)
    {
        logs.Add(new(type, text));

        string json = JsonUtility.ToJson(new WrappedLogs(), false);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();

        _onNewLogWithCatchUp?.Invoke(type, text);
    }
}
