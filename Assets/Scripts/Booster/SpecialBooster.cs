using UnityEngine;

public class SpecialBooster : MonoBehaviour
{
    private bool _aplicationquit = false;
    void Start()
    {
        BoosterSaver.Instance.BoostersSpeCount += 1;
    }

    void OplicationQuit()
    {
        _aplicationquit = true;
    }

    void OnDestroy()
    {
        if (!_aplicationquit)
            BoosterSaver.Instance.BoostersSpeCount -= 1;
    }
}
