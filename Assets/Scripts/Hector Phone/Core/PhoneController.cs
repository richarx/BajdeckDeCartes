using System.Collections.Generic;
using UnityEngine;

public class PhoneController : ReceivingAchievementMonoBehaviour
{
    [SerializeField] float cooldownBetweenTwoCalls = 1f;

    PhoneAnimation anim;

    readonly Queue<AchievementAsset> queue;

    float lastCallEndTime = float.MinValue;

    void Awake()
    {
        anim = GetComponent<PhoneAnimation>();
    }

    void OnEnable()
    {
        PhoneAnimation.OnPickUpPhone.AddListener(DoTheCall);
    }

    void OnDisable()
    {
        PhoneAnimation.OnPickUpPhone.RemoveListener(DoTheCall);
    }

    public override void OnAchievementReceived(AchievementAsset achievement)
    {
        queue.Enqueue(achievement);
    }

    void Update()
    {
        if (Time.time - lastCallEndTime < cooldownBetweenTwoCalls)
            return;

        if (queue.Count > 0)
        {
            anim.StartRinging();
        }
    }

    void DoTheCall()
    {
        AchievementAsset achievement = queue.Dequeue();



        lastCallEndTime = Time.time;
    }
}
