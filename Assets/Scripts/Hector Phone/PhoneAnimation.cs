using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PhoneAnimation : MonoBehaviour, GrabCursor.IInteractable
{
    public static UnityEvent OnPickUpPhone = new UnityEvent();

    private PhoneSFX phoneSFX;
    private Animator animator;

    private State state;

    private SqueezeAndStretch squeeze;

    private enum State
    {
        Idle,
        Ringing,
        OnThePhone
    }

    void Start()
    {
        state = State.Idle;

        squeeze = GetComponent<SqueezeAndStretch>();
        animator = GetComponent<Animator>();
        phoneSFX = GetComponent<PhoneSFX>();

        //AddListener sur le script de Lucas -> StartRinging
        //AddListener sur le script de Lucas -> FinishCall
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            FinishCall();
            state = State.Idle;
        }

        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            StartRinging();
            state = State.Ringing;
        }

        if (Keyboard.current.yKey.wasPressedThisFrame)
        {
            PickUpPhone();
            state = State.OnThePhone;
        }
    }



    private void ErrorSound()
    {
        phoneSFX.PlayNothingHappensSound();
        squeeze.Trigger();
    }

    public void StartRinging()
    {
        state = State.OnThePhone;
        phoneSFX.PlayRingingSound();

        animator.Play("Ringing");
    }

    public void PickUpPhone()
    {
        state = State.OnThePhone;

        phoneSFX.StopRinging();
        phoneSFX.PlayInteractSound();

        animator.Play("OnThePhone");

        squeeze.Trigger();

        OnPickUpPhone.Invoke();
    }

    public void FinishCall()
    {
        state = State.Idle;

        phoneSFX.PlayInteractSound();

        squeeze.Trigger();

        animator.Play("Idle");
    }

    public void Interact()
    {
        if (state == State.OnThePhone)
            return;
        else if (state == State.Idle)
            ErrorSound();
        else
            PickUpPhone();
    }


    public bool CanHover()
    {
        return true;
    }

    public void EndInteract()
    {

    }

    public int GetSortingPriority()
    {
        return 9;
    }

    public void Hover()
    {

    }
}
