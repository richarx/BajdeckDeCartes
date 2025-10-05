using UnityEngine;

public class BinderButton : MonoBehaviour, GrabCursor.IInteractable, IDragInteractable
{
    [SerializeField] private int _sortingPriority = 100;
    [SerializeField] private Binder _binder;
    
    public void EndInteract()
    {
    }

    public int GetSortingPriority()
    {
        return (_sortingPriority);
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

    public int GetSortingOrder()
    {
        return (50);
    }

    public bool CanHover()
    {
        return (true);
    }

}
