using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneController : ReceivingAchievementMonoBehaviour
{
    [SerializeField] float minCooldownBeforeCall = 5f;
    [SerializeField] float maxCooldownBeforeCall = 15f;
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

    void OnEnable()
    {
        PhoneAnimation.OnPickUpPhone.AddListener(DoTheCall);
        PhoneAnimation.OnTryPickUpPhone.AddListener(() => cooldown = Mathf.Min(cooldown, 1.5f));
    }

    void OnDisable() => PhoneAnimation.OnPickUpPhone.RemoveListener(DoTheCall);

    public override void OnAchievementReceived(AchievementAsset achievement)
    {
        if (cooldown <= 0)
            cooldown = UnityEngine.Random.Range(minCooldownBeforeCall, maxCooldownBeforeCall);
        queue.Enqueue(achievement);
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        if (boosterAnim.numberofCardsInWaitingRoom != 0) // pour que le telephone attende un peu que test ouvert ton booster avant de ring
            cooldown = UnityEngine.Random.Range(minCooldownBeforeCall, maxCooldownBeforeCall);

        if (isInCall || cooldown > 0f)
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

        achievement.Finish();
        cooldown = UnityEngine.Random.Range(minCooldownBeforeCall, maxCooldownBeforeCall);

        isInCall = false;
    }
}
