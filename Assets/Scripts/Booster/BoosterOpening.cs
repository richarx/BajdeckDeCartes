using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BoosterOpening : MonoBehaviour, GrabCursor.IInteractable
{
    [HideInInspector] public UnityEvent OnFinishOpeningPack = new UnityEvent();

    private Animator animator;
    private SqueezeAndStretch squeeze;

    private float slideValue;

    [SerializeField] private Transform startSlidePosition;
    [SerializeField] private Transform endSlidePosition;

    private bool isSliding;
    private bool isAutoCompleting;

    void Start()
    {
        animator = GetComponent<Animator>();
        squeeze = GetComponent<SqueezeAndStretch>();
    }

    void Update()
    {
        if (isSliding)
            PlayAnimation(Slide());
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
            OnFinishOpeningPack.Invoke();
        }
    }

    public void Interact()
    {
        isSliding = true;

        squeeze.Trigger();
    }

    public void EndInteract()
    {
        isSliding = false;
    }
}
