using System;
using System.Collections.Generic;
using UnityEngine;

public static class History
{
    const string PlayerPrefsKey = "HistoryLogsArray";

    [Serializable]
    struct SerializedLog
    {
        public LogSource source;
        public string text;

        public SerializedLog(LogSource source, string text) => (this.source, this.text) = (source, text);

        public void Deconstruct(out LogSource source, out string text) => (source, text) = (this.source, this.text);
    }

    [Serializable]
    class WrappedLogs
    {
        public SerializedLog[] array;

        public WrappedLogs() => array = logs.ToArray();

        public void Load() => logs.AddRange(array);
    }

    static readonly List<SerializedLog> logs = new();

#pragma warning disable IDE1006
    static event Action<LogSource, string> _onNewLogWithCatchUp;
#pragma warning restore IDE1006

    public static event Action<LogSource, string> OnNewLogWithCatchUp
    {
        add
        {
            foreach ((LogSource source, string text) in logs)
                value?.Invoke(source, text);

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
        if (PlayerPrefs.HasKey(PlayerPrefsKey) == false)
            return;

        string json = PlayerPrefs.GetString(PlayerPrefsKey);

        JsonUtility.FromJson<WrappedLogs>(json)
                   .Load();
    }

    public static void Log(LogSource source, string text)
    {
        logs.Add(new(source, text));

        string json = JsonUtility.ToJson(new WrappedLogs(), false);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();

        _onNewLogWithCatchUp?.Invoke(source, text);
    }
}
