using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PrinterAnimation : MonoBehaviour
{
    public static UnityEvent OnEndPrinting = new UnityEvent();

    private PrinterSFX printerSFX;

    private Animator animator;
    [SerializeField] Animator loadingBarAnimator;

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
        if (Keyboard.current.gKey.wasPressedThisFrame)
            StartPrinting();
    }
    
    //TODO Méthode a appeler pour l'anim
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
        printerSFX.PlayLaunchPrintingSound();

        yield return new WaitForSeconds (0.1f);

        printerSFX.PlayPrintingLoop();
        loadingBarAnimator.Play("Loading"); //Faire en sorte que la durée de l'animation corresponde à printDuration pour une belle animation

        yield return new WaitForSeconds(printDuration - 0.1f);

        StartCoroutine(StopPrinting());
    }

    private IEnumerator StopPrinting()
    {
        animator.Play("Idle");
        loadingBarAnimator.Play("Idle");

        printerSFX.PlayEndPrintingSound();

        yield return new WaitForSeconds(0.5f);

        squeeze.Trigger();

        isPrinting = false;
        OnEndPrinting.Invoke();
    }
}
