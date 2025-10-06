using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSourcePrefab;

    public static SFXManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public AudioSource PlayRandomSFX(AudioClip[] clips, float volume = 0.1f, float delay = 0.0f, bool loop = false, float pitch = 1f)
    {
        int index = Random.Range(0, clips.Length);

        return PlaySFX(clips[index], volume, delay, loop, pitch);
    }

    public AudioSource PlaySFX(AudioClip clip, float volume = 0.1f, float delay = 0.0f, bool loop = false, float pitch = 1f)
    {
        AudioSource source = Instantiate(audioSourcePrefab);

        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.pitch = pitch + Random.Range(-0.05f, 0.05f);
        if (delay <= 0.0f)
            source.Play();
        else
            source.PlayDelayed(delay);

        if (!loop)
            Destroy(source.gameObject, clip.length + delay);

        return source;
    }
    public AudioSource PlaySFXWithSpecificPitch(AudioClip clip, float pitch, float volume = 0.1f, float delay = 0.0f, bool loop = false)
    {
        AudioSource source = Instantiate(audioSourcePrefab);

        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        source.pitch = pitch;
        if (delay <= 0.0f)
            source.Play();
        else
            source.PlayDelayed(delay);

        if (!loop)
            Destroy(source.gameObject, clip.length + delay);

        return source;
    }

    public AudioSource PlaySFXNoPitchModifier(AudioClip clip, float volume = 0.1f, float delay = 0.0f, bool loop = false)
    {
        AudioSource source = Instantiate(audioSourcePrefab);

        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
        if (delay <= 0.0f)
            source.Play();
        else
            source.PlayDelayed(delay);

        if (!loop)
            Destroy(source.gameObject, clip.length + delay);

        return source;
    }
}
