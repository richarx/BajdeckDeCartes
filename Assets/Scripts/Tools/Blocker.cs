using UnityEngine;

public class Blocker : MonoBehaviour, GrabCursor.IInteractable
{

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public bool CanHover()
    {
        return (false);
    }

    public void EndInteract()
    {
    }

    public SortingData GetSortingPriority()
    {
        return (new SortingData(_spriteRenderer.sortingOrder, _spriteRenderer.sortingLayerID));
    }

    public void Hover()
    {
    }

    public void Interact()
    {
    }
}
