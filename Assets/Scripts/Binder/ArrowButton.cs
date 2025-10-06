using UnityEngine;

public class ArrowButton : MonoBehaviour, GrabCursor.IInteractable
{
    [SerializeField] private bool _isLeft = true;
    [SerializeField] private Binder _binder = null;

    private SpriteRenderer _spriteRenderer; 
    private bool _isActive = true;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void GrabCursor.IInteractable.EndInteract()
    {
    }

    SortingData GrabCursor.IInteractable.GetSortingPriority()
    {

        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    void GrabCursor.IInteractable.Interact()
    {
        
        if (_isLeft && _binder != null)
        {
            _binder.PreviousPage();
        }
        else if (_binder != null)
        {
            _binder.NextPage();
        }
    }

    public void Hide()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        _isActive = false;
    }

    public void Show()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
        _isActive = true;
    }

    public void Hover()
    {

    }



    bool GrabCursor.IInteractable.CanHover()
    {
        return (true);
    }
}
