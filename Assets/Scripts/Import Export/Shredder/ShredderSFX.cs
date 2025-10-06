using UnityEngine;

public class ShredderSFX : MonoBehaviour
{
    [SerializeField] private AudioClip cardPutInto;
    [SerializeField] private AudioClip startShredding;
    [SerializeField] private AudioClip shreddingLoop;
    [SerializeField] private AudioClip endShredding;
    [SerializeField] private AudioClip endNotification;

    private AudioSource shreddingLoopSound;

    public float delaynotifaftershredding;

    public void PlayEndShreddingSound()
    {
        if (shreddingLoopSound != null)
            Destroy(shreddingLoopSound);

        SFXManager.Instance.PlaySFX(endShredding);

        SFXManager.Instance.PlaySFX(endNotification, delay: delaynotifaftershredding);
    }

    public void PlayLaunchShreddingSound()
    {
        SFXManager.Instance.PlaySFX(cardPutInto);
        SFXManager.Instance.PlaySFX(startShredding);
    }

    public void PlayShreddingLoop()
    {
        shreddingLoopSound = SFXManager.Instance.PlaySFX(shreddingLoop, loop: true);
    }
}
