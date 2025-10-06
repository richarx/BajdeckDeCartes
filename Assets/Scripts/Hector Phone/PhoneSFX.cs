using System.Collections.Generic;
using UnityEngine;

public class PhoneSFX : MonoBehaviour
{
    [SerializeField] private AudioClip nothingHappens;
    [SerializeField] private AudioClip ringingSound;
    [SerializeField] private List<AudioClip> physicalPickupSounds;

    private AudioSource ringing;

    public void PlayNothingHappensSound()
    {
        SFXManager.Instance.PlaySFX(nothingHappens);
    }

    public void PlayRingingSound()
    {
        if (ringing == null)
            ringing = SFXManager.Instance.PlaySFX(ringingSound, loop: true);
    }

    public void StopRinging()
    {
        if (ringing != null)
        {
            ringing.Stop();
            ringing = null;
        }
    }

    public void PlayInteractSound()
    {
        SFXManager.Instance.PlayRandomSFX(physicalPickupSounds.ToArray());
    }
}
