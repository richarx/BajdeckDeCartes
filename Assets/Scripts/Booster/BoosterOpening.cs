using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BoosterOpening : MonoBehaviour, GrabCursor.IInteractable
{
    [HideInInspector] public UnityEvent OnFinishOpeningPack = new UnityEvent();

    private Animator animator;
    private SqueezeAndStretch squeeze;
    private BoosterSFX boosterSFX;

    private float currentSlideValue;

    [SerializeField] private Transform startSlidePosition;
    [SerializeField] private Transform endSlidePosition;
    [SerializeField] private int _sortingPriority = 10;

    private bool isSliding;
    private bool isAutoCompleting;

    void Start()
    {
        animator = GetComponent<Animator>();
        squeeze = GetComponent<SqueezeAndStretch>();
        boosterSFX = GetComponent<BoosterSFX>();
    }

    void Update()
    {
        if (isSliding)
            PlayAnimation(Slide());
    }

    public int GetSortingPriority()
    {
        return (_sortingPriority);
    }

    public bool CanHover() => true;

    public void Hover()
    {

    }



    private float Slide()
    {
        float currentPosition = GrabCursor.instance.transform.position.x;

        return Mathf.InverseLerp(startSlidePosition.position.x, endSlidePosition.position.x, currentPosition);
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
        }

        if (slideValue > 0.7f)
        {
            isAutoCompleting = true;
            animator.Play("Open", 0, slideValue);
            animator.Update(0f);
            animator.speed = 1f;
            EndInteract();
            boosterSFX.AutoCompleteSound();
            OnFinishOpeningPack.Invoke();
        }
    }

    public void Interact()
    {
        isSliding = true;

        boosterSFX.StartInteractSound();
        squeeze.Trigger();
    }

    public void EndInteract()
    {
        isSliding = false;
        boosterSFX.StopInteractSound();

        if (currentSlideValue <= 0.7f)
            animator.Play("Idle");
    }
}
