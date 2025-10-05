using UnityEngine;

public class BoosterSFX : MonoBehaviour
{
    [SerializeField] private AudioClip startInteractSound;
    [SerializeField] private AudioClip interruptInteractSound;
    [SerializeField] private AudioClip autoCompleteSound;
    [SerializeField] private AudioClip miniZipSound;

    private AudioSource zipSound;

    private float lastMinizipTimeStamp;
    public float intervalbetweenZip;

    public void StartInteractSound()
    {

    }

    public void InterruptSlidingSound()
    {

    }

    public void AutoCompleteSound()
    {

    }

    public void StartZipSound(float slideValue)
    {
        if (zipSound == null)
            zipSound = SFXManager.Instance.PlaySFX(miniZipSound);
        else
            UpdateZipSound(slideValue);
    }

    private void UpdateZipSound(float slideValue)
    {
        zipSound.pitch = Tools.NormalizeValue(slideValue, 0, 0.2f);
        zipSound.volume = Tools.NormalizeValue(slideValue, 0.05f, 0.1f);
    }

    //Rajouter du pitch pour faire le bruit de plus en plus aigu

    //Appelle a chaque frame
    //Appelle quand on commence à zipper

}
