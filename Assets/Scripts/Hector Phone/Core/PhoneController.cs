using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : ReceivingAchievementMonoBehaviour
{
    [SerializeField] float minCooldownBetweenTwoCalls = 3f;
    [SerializeField] float maxCooldownBetweenTwoCalls = 7f;
    [SerializeField] EntitySpawner spawner;
    [SerializeField] BoosterGlobalAnimation boosterAnim;

    PhoneAnimation anim;
    DialogBubble bubble;

    readonly Queue<AchievementAsset> queue = new();

    bool isInCall = false;
    float cooldown = 0f;

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
        cooldown -= Time.deltaTime;

        if (isInCall || cooldown > 0f || boosterAnim.numberofCardsInWaitingRoom != 0)
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

        bool isComplete = false;
        bubble.Display(achievement.HectorTexts, () => isComplete = true);
        yield return new WaitUntil(() => isComplete);

        spawner.GiveReward(achievement.Reward);

        foreach ((LogType type, string text) in achievement.Logs)
            History.Log(type, text);

        anim.FinishCall();

        isInCall = false;
        cooldown = Random.Range(minCooldownBetweenTwoCalls, maxCooldownBetweenTwoCalls);
    }
}
