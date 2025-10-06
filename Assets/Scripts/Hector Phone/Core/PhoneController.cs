using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : ReceivingAchievementMonoBehaviour
{
    [SerializeField] float cooldownBetweenTwoCalls = 1f;

    PhoneAnimation anim;
    DialogBubble bubble;

    readonly Queue<AchievementAsset> queue = new();

    bool isInCall = false;
    float lastCallEndTime = float.MinValue;

    void Awake()
    {
        anim = GetComponentInChildren<PhoneAnimation>();
        bubble = GetComponentInChildren<DialogBubble>();
    }

    void OnEnable() => PhoneAnimation.OnPickUpPhone.AddListener(DoTheCall);

    void OnDisable() => PhoneAnimation.OnPickUpPhone.RemoveListener(DoTheCall);

    public override void OnAchievementReceived(AchievementAsset achievement)
    {
        queue.Enqueue(achievement);
    }

    void Update()
    {
        if (isInCall || Time.time - lastCallEndTime < cooldownBetweenTwoCalls)
            return;

        if (queue.Count > 0)
        {
            anim.StartRinging();
        }
    }

    void DoTheCall()
    {
        if (isInCall)
            return;

        if (queue.TryDequeue(out AchievementAsset achievement) == false)
            return;

        StartCoroutine(CallRoutine(achievement));
    }

    IEnumerator CallRoutine(AchievementAsset achievement)
    {
        isInCall = true;

        foreach (string text in achievement.HectorTexts)
        {
            bool isComplete = false;

            bubble.Display(text, () => isComplete = true);

            yield return new WaitUntil(() => isComplete);
        }

        foreach ((LogType type, string text) in achievement.Logs)
            History.Log(type, text);

        isInCall = false;
        lastCallEndTime = Time.time;
    }
}
