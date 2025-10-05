using UnityEngine;

public interface IDragInteractable
{
    public void UseDraggable(Draggable drag);

    public bool CanUse(Draggable drag);

    public int GetSortingOrder();
}
