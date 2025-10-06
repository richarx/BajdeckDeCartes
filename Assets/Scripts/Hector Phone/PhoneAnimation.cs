using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//// jai mis des commentaires qui commencent par '////' (Lucas)
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

    void Start() //// Si tu met private a un endroit faut le mettre partout (oui remarque pete couille inutile mais bon)
    {
        state = State.Idle;

        squeeze = GetComponent<SqueezeAndStretch>();
        animator = GetComponent<Animator>();
        phoneSFX = GetComponent<PhoneSFX>();

        //// PAS BESOIN ! ya deja tout ce qui faut^^
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
        state = State.OnThePhone; //// wat ?? ca devrait pas etre ringing ? (et aussi un check verifier qu'on est pas déja dans ce state ! -> en terme d'API faut soit un silent ignore ou un explicit fail)
        phoneSFX.PlayRingingSound();

        animator.Play("Ringing");
    }

    public void PickUpPhone() //// no need for public
    {
        state = State.OnThePhone; //// check if not already in this state (imagine if a monkey was using this tout ça tout ça)

        phoneSFX.StopRinging();
        phoneSFX.PlayInteractSound();

        animator.Play("OnThePhone");

        squeeze.Trigger();

        OnPickUpPhone.Invoke();
    }

    public void FinishCall()
    {
        state = State.Idle;  //// check if not already in this state

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

    public void EndInteract() //// probleme d'interface segregation (une histoire pour un autre jour peut etre) + devrait probablement etre explicit implementation
    {

    }

    public int GetSortingPriority() //// Could be a property
    {
        return 9;
    }

    public void Hover()
    {

    }
}
