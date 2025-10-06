using UnityEngine;

public class ArrowButton : MonoBehaviour, GrabCursor.IInteractable
{
    [SerializeField] private bool _isLeft = true;
    [SerializeField] private int _sortingPriority = 200;
    [SerializeField] private Binder _binder = null;
    
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

    public void Hover()
    {

    }



    bool GrabCursor.IInteractable.CanHover()
    {
        return (true);
    }
}
