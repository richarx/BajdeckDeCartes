using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using EasyButtons;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }
    [SerializeField] private List<MusicTrack> _tracks = new();
    [SerializeField] private List<AudioClip> _transitionFXs = new();
    [SerializeField] private AudioClip _birthdayMusic;
    [SerializeField] private float _highVolume = 0.1f;
    [SerializeField] private float _lowVolume = 0.05f;
    [SerializeField] private float _transitionDuration = 0.5f;
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

    private bool _volumeChange = false;
    private float _targetVolume;
    private float _currentVolume;
    private float _durationLeft;

    private AudioSource _source;
    private CancellationTokenSource _linkedCancellationSource;
    private CancellationTokenSource _manualCancellationSource;

    void Awake()
    {
        Instance = this;
        ResetCancellation();
    }

    void Start()
    {
        PhoneAnimation.OnStartRinging.AddListener(() => ChangeVolume(true));
        PhoneAnimation.OnHangUpPhone.AddListener(() => ChangeVolume(false));
        _currentVolume = _highVolume;
        if (_tracks.Count > 0)
            PlayMusic(_tracks[0]).Forget();
    }

    void OnDestroy()
    {
        if (_source != null)
        {
            _source.loop = false;
            _source.Stop();
        }
    }

    void ChangeVolume(bool low)
    {
        if (low)
            _targetVolume = _lowVolume;
        else
            _targetVolume = _highVolume;
        _durationLeft = _transitionDuration;

        if (!_volumeChange && _source != null)
            ChangeVolume().Forget();
    }

    async UniTaskVoid ChangeVolume()
    {
        _volumeChange = true;

        while (_durationLeft > 0f)
        {
            _durationLeft -= Time.deltaTime;
            _currentVolume = Mathf.Lerp(_currentVolume, _targetVolume, Time.deltaTime / _durationLeft);
            if (_source != null)
                _source.volume = _currentVolume;
            await UniTask.Yield();
        }
        _volumeChange = false;
    }

    private void ResetCancellation()
    {
        CancellationToken destroyToken = this.GetCancellationTokenOnDestroy();
        _manualCancellationSource = new CancellationTokenSource();
        _linkedCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(_manualCancellationSource.Token, destroyToken);
    }

    [Button]
    async public UniTask PlayBirthdayMusic()
    {
        _manualCancellationSource.Cancel();
        ResetCancellation();
        await FadeOut(0.5f);
        _source = SFXManager.Instance.PlaySFXNoPitchModifier(_birthdayMusic, _currentVolume, loop: false);
        await UniTask.WaitUntil(() => _source == null, cancellationToken: _linkedCancellationSource.Token);
        PlayMusic(_tracks[0]).Forget();
    }

    async UniTaskVoid PlayMusic(MusicTrack track)
    {
        try
        {
            if (_transitionFXs.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, _transitionFXs.Count);
                SFXManager.Instance.PlaySFXNoPitchModifier(_transitionFXs[index], _currentVolume);
                await UniTask.WaitForSeconds(_transitionFXs[index].length / 2f, cancellationToken: _linkedCancellationSource.Token);
            }
            int plays = 0;
            _source = SFXManager.Instance.PlaySFXNoPitchModifier(track.music, _currentVolume, loop: true);
            bool staying = true;
            while (plays < track.minimumPlays || staying)
            {
                staying = UnityEngine.Random.Range(0, 100) < track.stayingChance;
                await UniTask.WaitUntil(() => _source.clip.length - _source.time <= track.fadeDuration, cancellationToken: _linkedCancellationSource.Token);
                plays++;
                if (staying) await UniTask.WaitUntil(() => _source.time <= 0.1f, cancellationToken: _linkedCancellationSource.Token);
            }
            MusicTrack nextTrack = GetNextTrack(track);
            await FadeOut(track.fadeDuration);

            await UniTask.Delay(TimeSpan.FromSeconds(_silenceDuration), cancellationToken: _linkedCancellationSource.Token);
            if (nextTrack != null)
                PlayMusic(nextTrack).Forget();

        }
        catch (OperationCanceledException)
        {
            Debug.Log("Cancel Music");
        }
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
        if (_source == null) return;
        _source.loop = false;
        float startVolume = _source.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            _source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            await UniTask.Yield(_linkedCancellationSource.Token);
            if (_source == null) return;
        }

        _source.volume = 0f;
        Destroy(_source.gameObject);
        _source = null;
    }
}
