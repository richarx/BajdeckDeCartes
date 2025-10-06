using System.Collections.Generic;
using UnityEngine;

public class ButtonClick : MonoBehaviour, GrabCursor.IInteractable
{
    private Animator animator;
    [SerializeField] private List<AudioClip> buttonClicks;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        animator.Play("Click");
        SFXManager.Instance.PlayRandomSFX(buttonClicks.ToArray());
    }

    public bool CanHover()
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
}
