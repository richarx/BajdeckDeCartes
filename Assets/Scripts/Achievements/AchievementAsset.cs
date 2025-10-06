using System.Collections.Generic;
using UnityEngine;

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

    string UniqueID => PlayerPrefsPrefix + name;

    /// <summary>
    /// Can be called too much (will only take effect once).
    /// </summary>
    public void Trigger()
    {
        if (PlayerPrefs.HasKey(UniqueID))
            return;

        PlayerPrefs.SetInt(UniqueID, 0);
        PlayerPrefs.Save();

        foreach (ReceivingAchievementMonoBehaviour mb in FindObjectsByType<ReceivingAchievementMonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            mb.OnAchievementReceived(this);
    }
}
