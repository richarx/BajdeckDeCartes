using UnityEngine;
using System.Collections.Generic;
using System.Linq;


enum TypeOfReward
{
    None,
    Normal,
    Holo
}


class RewardTracker
{
    public TypeOfReward reward = TypeOfReward.None;
    public Slot[] slots;
    
}

public class BinderCompletion : MonoBehaviour
{
    [SerializeField] int _numberBoosterOnNormal = 10;
    [SerializeField] int _numberBoosterOnHolo = 20;

    private Binder _binder;

    private List<RewardTracker> _rewardTrackers = new List<RewardTracker>();
   
    private void Awake()
    {
        _binder = GetComponent<Binder>();

        if (_binder == null)
        {
            Debug.LogError("No binder in binder completion");
            return;
        }

        _binder.OnSlotChanged += CheckSlotAndGiveTheReward;
        
        foreach (var page in _binder.Pages)
        {
            var rewardTracker = new RewardTracker();
            rewardTracker.reward = TypeOfReward.None;
            rewardTracker.slots = page.GetComponentsInChildren<Slot>();
            _rewardTrackers.Add(rewardTracker);
        }

    }

    private void OnDestroy()
    {
        _binder.OnSlotChanged -= CheckSlotAndGiveTheReward;
    }

    private void CheckSlotAndGiveTheReward()
    {
        foreach (RewardTracker reward in _rewardTrackers)
        {
            if (reward.reward == TypeOfReward.None && CheckIfHere(reward.slots))
            {
                GiveRewardNormal(reward);
            }

            if (reward.reward == TypeOfReward.Normal && CheckIfHolo(reward.slots))
            {
                GiveRewardHolo(reward);
            }
        }
    }

    private bool CheckIfHolo(Slot[] slots)
    {
        foreach (Slot slot in slots)
        {
            if (slot.CardInSlot != null)
            {

                if (slot.CardInSlot.Quality != Quality.Holographic)
                {
                    return (false);
                }
            }
            else
                return (false);
        }
        return (true);
    }

    private bool CheckIfHere(Slot[] slots)
    {
        int numberOfSlot = 0;
        
        foreach (Slot slot in slots)
        {
            if (slot.CardInSlot != null)
            {
                numberOfSlot++;
            }
        }
        if (numberOfSlot == 9)
            return (true);
        return (false);
    }

    private void GiveRewardNormal(RewardTracker rewardTracker)
    {
        rewardTracker.reward = TypeOfReward.Normal;

        Printer.instance.PrintBoosters(Vector3.zero, _numberBoosterOnNormal);
    }

    private void GiveRewardHolo(RewardTracker reward)
    {
        reward.reward = TypeOfReward.Holo;

        Printer.instance.PrintBoosters(Vector3.zero, _numberBoosterOnHolo);
    }
}
