using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PhoneAnimation : MonoBehaviour, GrabCursor.IInteractable
{
    public static UnityEvent OnPickUpPhone = new UnityEvent();
    public static UnityEvent OnHangUpPhone = new UnityEvent();
    public static UnityEvent OnStartRinging = new UnityEvent();


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

    void Start() //// si tu met private a un endroit faut le mettre partout (oui remarque pete couille inutile mais bon)
    {
        state = State.Idle;

        squeeze = GetComponent<SqueezeAndStretch>();
        animator = GetComponent<Animator>();
        phoneSFX = GetComponent<PhoneSFX>();

        //// pas besoin y'a tout :)
        //AddListener sur le script de Lucas -> StartRinging
        //AddListener sur le script de Lucas -> FinishCall
    }

    void Update()
    {

    }

    private void ErrorSound()
    {
        phoneSFX.PlayNothingHappensSound();
        squeeze.Trigger();
    }

    public void StartRinging()
    {
        state = State.Ringing;
        phoneSFX.PlayRingingSound();
        OnStartRinging.Invoke();

        animator.Play("Ringing");
    }

    public void PickUpPhone() //// no need for public
    {
        state = State.OnThePhone; //// pas besoin du check de state si cest privé :)

        phoneSFX.StopRinging();
        phoneSFX.PlayInteractSound();
        phoneSFX.PlayTalkingLoop();

        animator.Play("OnThePhone");

        squeeze.Trigger();

        OnPickUpPhone.Invoke();
    }

    public void FinishCall()
    {
        state = State.Idle; //// check si pas deja dans ce state (what if a monkey used this tout ça)

        phoneSFX.PlayInteractSound();
        phoneSFX.StopTalkingLoop();

        squeeze.Trigger();

        OnHangUpPhone.Invoke();

        animator.Play("Idle");
    }

    public void Interact()
    {
        Debug.Log($"Interract : {state}");

        if (state == State.OnThePhone)
            return;
        else if (state == State.Idle)
            ErrorSound();
        else
            PickUpPhone();
    }

    public bool CanInteract() //// probleme d'interface segregation (une histoire pour un autre jour peut etre) + devrait probablement etre explicit implementation
    {
        return true;
    }

    public void EndInteract()
    {

    }

    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public void Hover()
    {

    }

    public void EndHover()
    {
    }
}
