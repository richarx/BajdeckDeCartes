using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private List<MusicTrack> _tracks = new();
    [SerializeField] private List<AudioClip> _transitionFXs = new();
    [SerializeField] private float _volume = 0.2f;
    [SerializeField] private float _silenceDuration = 3f;

    [Serializable]
    private class MusicTrack
    {
        public AudioClip music;
        public int minimumPlays = 1;
        [Range(0, 100)] public int selectionChance = 50;
        [Range(0, 100)] public int stayingChance = 50;
        public float fadeDuration = 3f;
    }


    private AudioSource _source;

    void Start()
    {
        if (_tracks.Count > 0)
            PlayMusic(_tracks[0]).Forget();
    }

    async UniTaskVoid PlayMusic(MusicTrack track)
    {
        if (_transitionFXs.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, _transitionFXs.Count);
            SFXManager.Instance.PlaySFXNoPitchModifier(_transitionFXs[index], _volume);
            await UniTask.Delay(TimeSpan.FromSeconds(_transitionFXs[index].length / 2f));
        }
        int plays = 0;
        _source = SFXManager.Instance.PlaySFXNoPitchModifier(track.music, _volume, loop: true);
        bool staying = true;
        while (plays < track.minimumPlays || staying)
        {
            staying = UnityEngine.Random.Range(0, 100) < track.stayingChance;
            await UniTask.WaitUntil(() => _source == null || _source.clip.length - _source.time <= track.fadeDuration);
            plays++;
            if (staying) await UniTask.WaitUntil(() => _source.time <= 0.1f);
        }
        MusicTrack nextTrack = GetNextTrack(track);
        await FadeOut(track.fadeDuration);

        await UniTask.Delay(TimeSpan.FromSeconds(_silenceDuration));
        if (nextTrack != null)
            PlayMusic(nextTrack).Forget();
    }

    private MusicTrack GetNextTrack(MusicTrack currentTrack)
    {
        List<MusicTrack> possibleTracks = new();
        int totalChance = 0;
        foreach (var track in _tracks)
        {
            if (track != currentTrack)
            {
                possibleTracks.Add(track);
                totalChance += track.selectionChance;
            }
        }

        if (possibleTracks.Count == 0)
            return currentTrack;

        int randomValue = UnityEngine.Random.Range(0, totalChance);
        int cumulativeChance = 0;
        foreach (var track in possibleTracks)
        {
            cumulativeChance += track.selectionChance;
            if (randomValue < cumulativeChance)
                return track;
        }

        return possibleTracks[0];
    }

    private async UniTask FadeOut(float duration)
    {
        _source.loop = false;
        float startVolume = _source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            _source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            await UniTask.Yield();
        }

        _source.volume = 0f;
        Destroy(_source.gameObject);
    }
}
