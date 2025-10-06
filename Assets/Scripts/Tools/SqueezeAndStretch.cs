using System.Collections;
using UnityEditor;
using UnityEngine;

public class SqueezeAndStretch : MonoBehaviour
{
    [SerializeField] bool _shouldRefreshOGScale = false;

    public Transform targetTransform;
    private Coroutine squeezeRoutine = null;

    public float xSqueeze;
    public float ySqueeze;
    public float duration;
    public float delay;
    private Vector3 OGSize;

    void Awake()
    {
        OGSize = targetTransform.localScale;
    }

    public void Trigger()
    {
        if (squeezeRoutine != null)
            StopCoroutine(squeezeRoutine);

        squeezeRoutine = StartCoroutine(Squeeze());
    }

    private IEnumerator Squeeze()
    {
        if (_shouldRefreshOGScale)
            OGSize = targetTransform.localScale;

        Vector3 newSize = new Vector3(OGSize.x * xSqueeze, OGSize.y * ySqueeze, OGSize.z);

        float timer = 0f;
        float t = 0f;

        if (delay != 0)
            yield return new WaitForSeconds(delay);

        while (timer < duration)
        {
            t = Tools.NormalizeValue(timer, 0, duration);
            targetTransform.localScale = Vector3.Lerp(newSize, OGSize, t);
            yield return null;
            timer += Time.deltaTime;
        }

        targetTransform.localScale = OGSize;
        squeezeRoutine = null;
    }
}
