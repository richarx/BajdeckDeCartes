using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShredderAnimation : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnEndShredding = new UnityEvent();

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

    //TODO Méthode a appeler pour l'anim
    public void StartShredding()
    {
        if (isShredding == true)
            return;

        StartCoroutine(Shredding());
    }

    private IEnumerator Shredding()
    {
        isShredding = true;
        squeeze.Trigger();

        animator.Play("Shredding");
        buttonAnimator.Play("Shredding");
        shredderSFX.PlayLaunchShreddingSound();

        yield return new WaitForSeconds(shreddingDuration);

        StartCoroutine(StopShredding());
    }

    private IEnumerator StopShredding()
    {
        animator.Play("Idle");
        buttonAnimator.Play("Idle");

        shredderSFX.PlayEndShreddingSound();

        yield return new WaitForSeconds(0.5f);

        squeeze.Trigger();

        isShredding = false;
        OnEndShredding.Invoke();
    }
}
