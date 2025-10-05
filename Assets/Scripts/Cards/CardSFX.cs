using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardSFX : MonoBehaviour
{
    [SerializeField] private List<AudioClip> pickUpSounds;
    [SerializeField] private List<AudioClip> dropSounds;

    public void PickUpSound()
    {
        SFXManager.Instance.PlayRandomSFX(pickUpSounds.ToArray());
    }

    public void DropSound()
    {
        SFXManager.Instance.PlayRandomSFX(dropSounds.ToArray());

    }
}
