using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShredderAnimation : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Draggable> OnEndShredding = new UnityEvent<Draggable>();

    private ShredderSFX shredderSFX;

    private Animator animator;
    [SerializeField] private Animator buttonAnimator;

    private SqueezeAndStretch squeeze;

    [SerializeField] private float shreddingDuration;

    private bool isShredding;
    public bool IsShredding => isShredding;

    void Start()
    {
        squeeze = GetComponent<SqueezeAndStretch>();
        animator = GetComponent<Animator>();
        shredderSFX = GetComponent<ShredderSFX>();
    }

    void Update()
    {

    }

    public void StartShredding(Draggable card)
    {
        if (isShredding == true)
            return;

        StartCoroutine(Shredding(card));
    }

    private IEnumerator Shredding(Draggable card)
    {
        isShredding = true;
        squeeze.Trigger();

        card.SetToInitialScale();

        animator.Play("Shredding");
        buttonAnimator.Play("Shredding");
        shredderSFX.PlayLaunchShreddingSound();

        yield return new WaitForSeconds(shreddingDuration);

        StartCoroutine(StopShredding(card));
    }

    private IEnumerator StopShredding(Draggable card)
    {
        animator.Play("Idle");
        buttonAnimator.Play("Idle");

        shredderSFX.PlayEndShreddingSound();

        yield return new WaitForSeconds(0.5f);

        squeeze.Trigger();

        isShredding = false;
        OnEndShredding.Invoke(card);
    }
}
