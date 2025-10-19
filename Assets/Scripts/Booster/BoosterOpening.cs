using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

public class BoosterOpening : MonoBehaviour, GrabCursor.IInteractable
{
    public static UnityEvent<List<CardInstance>> OnFinishOpeningPack = new UnityEvent<List<CardInstance>>();
    [SerializeField] private UnityEvent _onOpening = new();
    [SerializeField] private float distance = 3f;
    [SerializeField] private int _cardsToSpawn = 5;
    [SerializeField, Range(0, 1)] private float slideCompletion = 0.7f;
    [SerializeField] private CardSpawner _spawnerBooster;

    [SerializeField] private int endScale = 2;

    [SerializeField] private SpriteRenderer _spriteRendererLueur;

    [SerializeField] private MMF_Player openingSequencer;
    [SerializeField] private bool _dontSave = false;

    private Animator animator;
    private SqueezeAndStretch squeeze;
    private BoosterSFX boosterSFX;

    private float currentSlideValue;

    private Vector3 _initialBoosterPosition = Vector3.zero;
    private float _initialCursorPosition = 0;
    private float _initialBoosterScale = 1.5f;
    private Vector3 _initialRotation;
    private SpriteRenderer _spriteRenderer;

    private bool isSliding;
    private bool isAutoCompleting;
    private bool _isActive = true;
    private Collider2D _collider;
    private float intensity = 0;
    private MeanShake _meanShake = null;

    Sequence seq;
    private float meanScale = 1;
    private bool _aplicationquit = false;
    private Rigidbody2D _rb;
    private SqueezeAndStretch _squeezeAndStretch;

    void Awake()
    {
        animator = GetComponent<Animator>();
        squeeze = GetComponent<SqueezeAndStretch>();
        boosterSFX = GetComponent<BoosterSFX>();
        _collider = GetComponent<Collider2D>();
        _meanShake = GetComponentInParent<MeanShake>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rb = GetComponentInParent<Rigidbody2D>();
        _squeezeAndStretch = GetComponent<SqueezeAndStretch>();
    }

    void Start()
    {
        if (!_dontSave)
            BoosterSaver.Instance.BoostersCount += 1;
    }

    private void OnApplicationQuit()
    {
        _aplicationquit = true;

    }

    void OnDestroy()
    {
        if (!_aplicationquit && !_dontSave)
            BoosterSaver.Instance.BoostersCount -= 1;
    }

    void Update()
    {
        if (isSliding)
            PlayAnimation(Slide());
    }


    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public bool CanInteract() => true;

    public void Hover()
    {

    }



    private float Slide()
    {
        float currentPosition = GrabCursor.instance.transform.position.x;

        if (currentPosition < _initialCursorPosition)
            _initialCursorPosition = currentPosition;
        return Mathf.InverseLerp(_initialCursorPosition, _initialCursorPosition + distance, currentPosition);
    }

    private void PlayAnimation(float slideValue)
    {
        if (isAutoCompleting == true)
            return;
        else
        {
            currentSlideValue = slideValue;
            animator.speed = 0f;
            animator.Play("Open", 0, slideValue);
            animator.Update(0f);

            float t = currentSlideValue / slideCompletion;

            float scale = Mathf.Lerp(_initialBoosterScale, endScale, t);
            transform.localScale = new Vector3(scale, scale, 1);
            _spriteRendererLueur.color = new Color(1, 1, 1, t);
            _meanShake.intensity = t;
        }

        if (slideValue > slideCompletion)
        {
            seq.Complete();
            _squeezeAndStretch.running = false;
            float scale = endScale;
            transform.localScale = new Vector3(scale, scale, 1);
            isAutoCompleting = true;
            _meanShake.intensity = 1;
            openingSequencer.PlayFeedbacks();
            _spriteRenderer.sortingOrder = 1000;
            PlayAndWaitDeath(slideValue);

            EndInteract();
            boosterSFX.AutoCompleteSound();


            // Spawn 
            _onOpening.Invoke();
            var spawned = _spawnerBooster.SpawnNRandomCardsSortedByRarity(_cardsToSpawn, false);
            spawned.ForEach((x) =>
            {
                CardTableManager.Instance.AddCard(x);
            });
            OnFinishOpeningPack?.Invoke(spawned);
        }
    }

    public void Interact()
    {
        if (!_isActive) return;

        _rb.angularVelocity = 0;
        _rb.linearVelocity = Vector2.zero;
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _rb.simulated = false;
        isSliding = true;
        _meanShake.enabled = true;
        GrabCursor.instance.HideCursor();

        _initialCursorPosition = GrabCursor.instance.transform.position.x;
        _initialBoosterPosition = transform.parent.position;
        _initialBoosterScale = transform.localScale.x;
        _initialRotation = transform.rotation.eulerAngles;
        seq?.Kill();
        seq = DOTween.Sequence()
            .SetLink(gameObject)
            .SetEase(Ease.InOutQuad)
            .Join(transform.DOMove(Vector3.zero, 0.2f))
            .Join(transform.DORotate(Vector3.zero, 0.2f));


        _spriteRenderer.sortingOrder += 100;
        boosterSFX.StartInteractSound();
        squeeze.Trigger();
    }


    void PlayAndWaitDeath(float slideValue)
    {
        _collider.enabled = false;
        animator.Play("Open", 0, slideValue);
        animator.Update(0f);
        animator.speed = 1f;
        Destroy(transform.parent.gameObject, 2);
    }


    public void EndInteract()
    {
        _isActive = false;
        isSliding = false;
        GrabCursor.instance.ShowCursor();
        if (_rb == null) return;
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _rb.simulated = true;
        boosterSFX.StopInteractSound();
        openingSequencer.RestoreInitialValues();


        if (currentSlideValue <= 0.7f)
        {
            _spriteRenderer.sortingOrder -= 100;
            _meanShake.enabled = false;
            animator.Play("Idle");
            _isActive = false;
            seq?.Kill();
            seq = DOTween.Sequence()
                .SetLink(gameObject)
                .SetEase(Ease.InOutQuad)
                .Join(transform.DOMove(_initialBoosterPosition, 0.2f))
                .Join(transform.DORotate(_initialRotation, 0.2f))
                .Join(transform.DOScale(new Vector3(_initialBoosterScale, _initialBoosterScale, 1), 0.2f))
                .OnComplete(() => _isActive = true)
                .OnKill(() => { _isActive = true; transform.localScale = new Vector3(_initialBoosterScale, _initialBoosterScale, 1); transform.position = _initialBoosterPosition; });


        }
    }

    public void EndHover()
    {
    }
}
