using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "AchievementAsset", menuName = "Scriptable Objects/Achievement Asset")]
public class AchievementAsset : ScriptableObject
{
    const string PlayerPrefsPrefix = "Achievement_";

    [SerializeField, TextArea(1, 3)] List<string> hectorTexts;
    public IReadOnlyList<string> HectorTexts => hectorTexts;

    [SerializeField] List<Log> logs;
    public IReadOnlyList<Log> Logs => logs;

    [field: SerializeField]
    public AchievementReward Reward { get; private set; }
    public UnityEvent OnFinish { get; private set; } = new();
    public bool Triggered => _triggered || PlayerPrefs.HasKey(UniqueID);

    string UniqueID => PlayerPrefsPrefix + name;

    bool _triggered = false;

    private void OnEnable()
    {
        _triggered = false;
        OnFinish = new();
    }

    /// <summary>
    /// Can be called too much (will only take effect once).
    /// </summary>
    public void Trigger()
    {
        if (PlayerPrefs.HasKey(UniqueID) || _triggered)
            return;
        _triggered = true;
        foreach (ReceivingAchievementMonoBehaviour mb in FindObjectsByType<ReceivingAchievementMonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            mb.OnAchievementReceived(this);
    }

    public void Finish()
    {
        PlayerPrefs.SetInt(UniqueID, 0);
        PlayerPrefs.Save();
        OnFinish.Invoke();
    }
}
