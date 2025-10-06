using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Collections;

public class BoosterOpening : MonoBehaviour, GrabCursor.IInteractable
{
    [HideInInspector] public UnityEvent OnFinishOpeningPack = new UnityEvent();

    private Animator animator;
    private SqueezeAndStretch squeeze;
    private BoosterSFX boosterSFX;

    private float currentSlideValue;

    [SerializeField] private float distance = 3f;
    [SerializeField, Range(0, 1)] private float slideCompletion = 0.7f;
    [SerializeField] private CardSpawner _spawnerBooster;

    [SerializeField] private int endScale = 2;
    [SerializeField] private Vector3 endPosition = Vector3.zero;
    
    private Vector3 _initialBoosterPosition = Vector3.zero;
    private float _initialCursorPosition = 0;
    private float _initialBoosterScale = 1;

    private bool isSliding;
    private bool isAutoCompleting;
    private bool _isActive = true;

    int openingHash;

    Sequence seq;

    void Awake()
    {
        animator = GetComponent<Animator>();
        squeeze = GetComponent<SqueezeAndStretch>();
        boosterSFX = GetComponent<BoosterSFX>();
        openingHash = Animator.StringToHash("Play");
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

            transform.position = Vector3.Lerp(_initialBoosterPosition, endPosition, currentSlideValue / slideCompletion);
            float scale = Mathf.Lerp(_initialBoosterScale, endScale, currentSlideValue / slideCompletion);
            transform.localScale = new Vector3(scale, scale, 1);

        }

        if (slideValue > slideCompletion)
        {
            isAutoCompleting = true;
            transform.position =  endPosition;
            float scale = endScale;
            transform.localScale = new Vector3(scale, scale, 1);

            StartCoroutine(PlayAndWaitDeath(slideValue));

            EndInteract();
            boosterSFX.AutoCompleteSound();
            OnFinishOpeningPack.Invoke();

            // Spawn 
            _spawnerBooster.SpawnNRandomCardsSortedByRarity(5, false);
        }
    }

    public void Interact()
    {
        if (!_isActive) return;

        isSliding = true;
        GrabCursor.instance.HideCursor();

        _initialCursorPosition = GrabCursor.instance.transform.position.x;
        _initialBoosterPosition = this.transform.position;
        _initialBoosterScale = transform.localScale.x;

        boosterSFX.StartInteractSound();
        squeeze.Trigger();
    }


    IEnumerator PlayAndWaitDeath(float slideValue)
    {

        animator.Play("Open", 0, slideValue);
        animator.Update(0f);
        animator.speed = 1f;
        float lenght = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(lenght);

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
