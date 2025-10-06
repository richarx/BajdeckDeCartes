using UnityEngine;

public class MeanShake : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0f;
    public Vector3 meanPosition;
    public Vector3 meanScale = Vector3.one;

    [Header("Paramètres shake")]
    public float posAmplitude = 0.2f;
    public float scaleAmplitude = 0.05f;
    public float frequency = 20f;
    public bool useUnscaledTime = false;
    public bool damp = false;
    public float decayPerSec = 2f;

    float _phase;

    void Awake()
    {
        _phase = Random.value * 1000f;
    }

    private void OnEnable()
    {
        meanPosition = transform.localPosition;
        meanScale = transform.localScale;
    }

    void Update()
    {
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float tt = (useUnscaledTime ? Time.unscaledTime : Time.time) * frequency + _phase;

        float i = Mathf.Max(0f, intensity);
        if (damp && i > 0f) intensity = Mathf.Max(0f, i - decayPerSec * dt);

        // Offsets lissés autour de 0
        Vector3 posOff = new Vector3(
            Mathf.PerlinNoise(tt, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, tt) - 0.5f,
            Mathf.PerlinNoise(tt, tt) - 0.5f
        ) * (posAmplitude * i * 2f);

        Vector3 scaleMul = new Vector3(
            1f + (Mathf.PerlinNoise(tt, 1f) - 0.5f) * (scaleAmplitude * i * 2f),
            1f + (Mathf.PerlinNoise(1f, tt) - 0.5f) * (scaleAmplitude * i * 2f),
            1f + (Mathf.PerlinNoise(tt * 0.7f, 2f) - 0.5f) * (scaleAmplitude * i * 2f)
        );

        transform.localPosition = meanPosition + posOff;
        transform.localScale = Vector3.Scale(meanScale, scaleMul);
    }

    [ContextMenu("SetMeansFromCurrent")]
    public void SetMeansFromCurrent()
    {
        meanPosition = transform.localPosition;
        meanScale = transform.localScale;
    }
}