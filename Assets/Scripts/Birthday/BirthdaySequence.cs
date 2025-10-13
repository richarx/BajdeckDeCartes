using UnityEngine;

public class BirthdaySequence : MonoBehaviour
{
    async void Start()
    {
        await MusicManager.Instance.PlayBirthdayMusic();
        Destroy(gameObject);
    }
}
