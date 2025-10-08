using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonClick : MonoBehaviour, GrabCursor.IInteractable
{
    public UnityEvent onInteract = new();
    public UnityEvent onHover = new();
    public UnityEvent onEndInteract = new();
    private Animator animator;
    [SerializeField] private List<AudioClip> buttonClicks;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        onInteract.Invoke();
        animator.Play("Click");
        SFXManager.Instance.PlayRandomSFX(buttonClicks.ToArray());
    }

    public bool CanInteract()
    {
        return true;
    }

    public void EndInteract()
    {
        onEndInteract.Invoke();
    }

    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public void Hover()
    {
        onHover.Invoke();
    }

    public void EndHover()
    {
    }
}
