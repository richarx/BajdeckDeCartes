using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PrinterAnimation : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnEndPrinting = new UnityEvent();

    private PrinterSFX printerSFX;

    private Animator animator;
    [SerializeField] Animator loadingBarAnimator;
    [SerializeField] Animator smallButtonAnimator;
    [SerializeField] Animator largeButtonAnimator;

    private SqueezeAndStretch squeeze;

    [SerializeField] private float printDuration;

    private bool isPrinting;
    public bool IsPrinting => isPrinting;

    void Start()
    {
        squeeze = GetComponent<SqueezeAndStretch>();
        animator = GetComponent<Animator>();
        printerSFX = GetComponent<PrinterSFX>();
    }

    void Update()
    {

    }

    public void StartPrinting()
    {
        if (isPrinting == true)
            return;

        StartCoroutine(Printing());
    }

    private IEnumerator Printing()
    {
        isPrinting = true;
        squeeze.Trigger();

        animator.Play("Printing");
        smallButtonAnimator.Play("Printing");
        largeButtonAnimator.Play("Printing");
        loadingBarAnimator.Play("Loading"); //Faire en sorte que la dur�e de l'animation corresponde � printDuration pour une belle animation

        printerSFX.PlayLaunchPrintingSound();

        yield return new WaitForSeconds(0.1f);

        printerSFX.PlayPrintingLoop();

        yield return new WaitForSeconds(printDuration - 0.1f);

        StartCoroutine(StopPrinting());
    }

    private IEnumerator StopPrinting()
    {
        animator.Play("Idle");
        loadingBarAnimator.Play("Idle");
        smallButtonAnimator.Play("Idle");
        largeButtonAnimator.Play("Idle");

        printerSFX.PlayEndPrintingSound();

        yield return new WaitForSeconds(0.5f);

        squeeze.Trigger();

        isPrinting = false;
        OnEndPrinting.Invoke();
    }
}
