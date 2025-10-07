using System.Collections;
using UnityEngine;

public class PrinterSFX : MonoBehaviour
{
    [SerializeField] private AudioClip cardEjected;
    [SerializeField] private AudioClip endPrinting;
    [SerializeField] private AudioClip launchPrinting;
    [SerializeField] private AudioClip printingLoop;

    private AudioSource printingLoopSound;

    public void PlayEndPrintingSound()
    {
        if (printingLoopSound != null)
            Destroy(printingLoopSound.gameObject);

        SFXManager.Instance.PlaySFX(endPrinting);

        SFXManager.Instance.PlaySFX(cardEjected, delay: 0.5f);
    }

    public void PlayLaunchPrintingSound()
    {
        SFXManager.Instance.PlaySFX(launchPrinting);
    }

    public void PlayPrintingLoop()
    {
        printingLoopSound = SFXManager.Instance.PlaySFX(launchPrinting, loop: true);
    }
}
