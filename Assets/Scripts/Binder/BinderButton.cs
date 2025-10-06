using UnityEngine;

public class BinderButton : MonoBehaviour, GrabCursor.IInteractable, IDragInteractable
{
    [SerializeField] private Binder _binder;
    
    public void EndInteract()
    {
    }



    public void Interact()
    {
        if (_binder.IsOpened)
            _binder.Close();
        else
            _binder.Open();
    }

    public void Hover()
    {
        //var cardInstance = GrabCursor.instance.Draggable?.GetComponent<CardInstance>();
        //if (cardInstance != null)
        //{
        //    if (cardInstance.Data != null)
        //        _binder.OpenForNumber(cardInstance.Data.Number);
        //    else
        //        _binder.Open();
        //}

        // Put anim hover when card in hand
    }

    public void UseDraggable(Draggable drag)
    {
        _binder.UseDraggable(drag);
    }

    public bool CanUse(Draggable drag)
    {
        return (drag.GetComponent<CardInstance>() != null);
    }

    public SortingData GetSortingOrder()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }


    public bool CanHover()
    {
        return (true);
    }

}
