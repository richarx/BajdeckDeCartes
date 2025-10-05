using System.Collections.Generic;
using UnityEngine;

public class BoosterSFX : MonoBehaviour
{
    [SerializeField] private List<AudioClip> startInteractSound;
    [SerializeField] private List<AudioClip> interruptInteractSound;
    [SerializeField] private List<AudioClip> autoCompleteZipSound;
    [SerializeField] private AudioClip miniZipSound;

    private AudioSource zipSound;

    public void StartInteractSound()
    {
        SFXManager.Instance.PlayRandomSFX(startInteractSound.ToArray());
    }

    public void StopInteractSound()
    {
        SFXManager.Instance.PlayRandomSFX(interruptInteractSound.ToArray());
    }

    public void AutoCompleteSound()
    {
        SFXManager.Instance.PlayRandomSFX(autoCompleteZipSound.ToArray());
    }

    public void StartZipSound()
    {
        if (zipSound == null)
            zipSound = SFXManager.Instance.PlaySFX(miniZipSound);
    }

    public void UpdateZipSound(float slideValue)
    {
        if (zipSound == null)
            return;

        zipSound.pitch = Tools.NormalizeValue(slideValue, 0, 0.2f);
        zipSound.volume = Tools.NormalizeValue(slideValue, 0f, 0.1f);
    }
}
