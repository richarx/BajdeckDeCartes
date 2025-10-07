using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

public class BoosterOpening : MonoBehaviour, GrabCursor.IInteractable
{
    public static UnityEvent<List<CardInstance>> OnFinishOpeningPack = new UnityEvent<List<CardInstance>>();

    private Animator animator;
    private SqueezeAndStretch squeeze;
    private BoosterSFX boosterSFX;

    private float currentSlideValue;

    [SerializeField] private float distance = 3f;
    [SerializeField] private int _cardsToSpawn = 5;
    [SerializeField, Range(0, 1)] private float slideCompletion = 0.7f;
    [SerializeField] private CardSpawner _spawnerBooster;

    [SerializeField] private int endScale = 2;
    [SerializeField] private Vector3 endPosition = Vector3.zero;

    [SerializeField] private SpriteRenderer _spriteRendererLueur;

    [SerializeField] private MMF_Player openingSequencer;

    private Vector3 _initialBoosterPosition = Vector3.zero;
    private float _initialCursorPosition = 0;
    private float _initialBoosterScale = 1.5f;
    private SpriteRenderer _spriteRenderer;

    private bool isSliding;
    private bool isAutoCompleting;
    private bool _isActive = true;
    private Collider2D _collider;
    private float intensity = 0;
    private MeanShake _meanShake = null;

    Sequence seq;
    private float meanScale = 1;

    void Awake()
    {
        animator = GetComponent<Animator>();
        squeeze = GetComponent<SqueezeAndStretch>();
        boosterSFX = GetComponent<BoosterSFX>();
        _collider = GetComponent<Collider2D>();
        _meanShake = GetComponentInParent<MeanShake>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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

    public bool CanHover() => true;

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

            transform.position = Vector3.Lerp(_initialBoosterPosition, endPosition, t);
            float scale = Mathf.Lerp(_initialBoosterScale, endScale, t);
            transform.localScale = new Vector3(scale, scale, 1);
            _spriteRendererLueur.color = new Color(1, 1, 1, t);
            _collider.attachedRigidbody.linearVelocity = Vector2.zero;
            _collider.attachedRigidbody.angularVelocity = 0;
            _meanShake.intensity = t;
        }

        if (slideValue > slideCompletion)
        {
            isAutoCompleting = true;
            transform.position = endPosition;
            _meanShake.intensity = 1;
            openingSequencer.PlayFeedbacks();
            float scale = endScale;
            transform.localScale = new Vector3(scale, scale, 1);
            _spriteRenderer.sortingOrder = 1000;
            StartCoroutine(PlayAndWaitDeath(slideValue));

            EndInteract();
            boosterSFX.AutoCompleteSound();


            // Spawn 
            var spawned = _spawnerBooster.SpawnNRandomCardsSortedByRarity(_cardsToSpawn, false);
            spawned.ForEach(x => x.transform.position = _spawnerBooster.transform.position);
            OnFinishOpeningPack?.Invoke(spawned);
        }
    }

    public void Interact()
    {
        if (!_isActive) return;

        isSliding = true;
        _meanShake.enabled = true;
        _collider.attachedRigidbody.angularVelocity = 0;
        _collider.attachedRigidbody.linearVelocity = Vector2.zero;
        _collider.attachedRigidbody.rotation = 0;
        GrabCursor.instance.HideCursor();

        _initialCursorPosition = GrabCursor.instance.transform.position.x;
        _initialBoosterPosition = this.transform.position;
        _initialBoosterScale = transform.localScale.x;


        _spriteRenderer.sortingOrder += 100;
        boosterSFX.StartInteractSound();
        squeeze.Trigger();
    }


    IEnumerator PlayAndWaitDeath(float slideValue)
    {
        _collider.enabled = false;
        animator.Play("Open", 0, slideValue);
        animator.Update(0f);
        animator.speed = 1f;
        yield return new WaitForSeconds(2);

        Destroy(this.transform.parent.gameObject);
    }


    public void EndInteract()
    {
        _isActive = false;
        isSliding = false;
        boosterSFX.StopInteractSound();


        GrabCursor.instance.ShowCursor();
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
                .Join(transform.DOScale(new Vector3(_initialBoosterScale, _initialBoosterScale, 1), 0.2f))
                .OnComplete(() => _isActive = true)
                .OnKill(() => { _isActive = true; transform.localScale = new Vector3(_initialBoosterScale, _initialBoosterScale, 1); transform.position = _initialBoosterPosition; });


        }
    }
}
