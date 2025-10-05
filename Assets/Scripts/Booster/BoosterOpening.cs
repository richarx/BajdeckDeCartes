using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BoosterOpening : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnFinishOpeningPack = new UnityEvent();

    private Animator animator;

    private float slideValue;

    [SerializeField] private Transform startSlidePosition;
    [SerializeField] private Transform endSlidePosition;

    private bool isSliding;
    private bool isAutoCompleting;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isSliding)
            PlayAnimation(Slide());

        if (isSliding && !GrabCursor.instance.IsGrabbing && isAutoCompleting == false)
            ResetAnimation();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (GrabCursor.instance.IsGrabbing && collider.CompareTag("Cursor") && GrabCursor.instance.hasSomething == false && isSliding == false)
            StartSliding();
    }

    private float Slide()
    {
        float currentPosition = GrabCursor.instance.transform.position.x;

        return Mathf.InverseLerp(startSlidePosition.position.x, endSlidePosition.position.x, currentPosition);
    }

    private void StartSliding()
    {
        isSliding = true;
        GrabCursor.instance.hasSomething = true;
    }

    private void StopSliding()
    {
        isSliding = false;
        GrabCursor.instance.hasSomething = false;
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
            StopSliding();
            OnFinishOpeningPack.Invoke();
        }
    }

    private void ResetAnimation()
    {
        animator.Play("Idle");
        StopSliding();
    }
}
