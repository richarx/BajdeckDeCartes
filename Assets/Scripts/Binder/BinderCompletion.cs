using System.Collections.Generic;
using System.Linq;
using UnityEngine;


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

    private bool _isLoaded = false;

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
            rewardTracker.slots = page.GetComponentsInChildren<Slot>(true);
            _rewardTrackers.Add(rewardTracker);
        }

        Load();
        _isLoaded = true;
    }

    private void OnDestroy()
    {
        _binder.OnSlotChanged -= CheckSlotAndGiveTheReward;
    }

    public int ComputeCompletedPagesCount()
    {
        return _rewardTrackers.Count((r) => r.reward != TypeOfReward.None);
    }

    public int ComputeTotalCardsCount()
    {
        int count = 0;
        foreach (RewardTracker reward in _rewardTrackers)
        {
            count += ComputeUniqueCardCount(reward.slots);
        }

        return count;
    }

    private void CheckSlotAndGiveTheReward()
    {
        if (!_isLoaded) return;

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

    private int ComputeUniqueCardCount(Slot[] slots)
    {
        int numberOfSlot = 0;

        foreach (Slot slot in slots)
        {
            if (slot.CardInSlot != null)
            {
                numberOfSlot++;
            }
        }

        return numberOfSlot;
    }

    private void GiveRewardNormal(RewardTracker rewardTracker)
    {
        rewardTracker.reward = TypeOfReward.Normal;

        Printer.instance.PrintBoosters(_numberBoosterOnNormal);
        Save();
    }

    private void GiveRewardHolo(RewardTracker reward)
    {
        reward.reward = TypeOfReward.Holo;

        Printer.instance.PrintBoosters(_numberBoosterOnHolo);
        Save();
    }

    private void Save()
    {
        int i = 0;

        foreach (var reward in _rewardTrackers)
        {
            string name = "Slot_" + i.ToString();

            PlayerPrefs.SetInt(name, (int)reward.reward);
            i++;
        }
    }

    private void Load()
    {
        int i = 0;

        foreach (var reward in _rewardTrackers)
        {
            string name = "Slot_" + i.ToString();

            var value = PlayerPrefs.GetInt(name, -1);
            if (value >= 0)
            {
                reward.reward = (TypeOfReward)value;
            }
            i++;
        }
    }
}
