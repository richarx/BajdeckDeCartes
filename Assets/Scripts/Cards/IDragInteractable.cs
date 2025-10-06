using UnityEngine;

public class SortingData
{
    public int sortingOrder;
    public int sortingLayerId;

    public SortingData(int sortingOrder, int sortingLayerId)
    {
        this.sortingOrder = sortingOrder;
        this.sortingLayerId = sortingLayerId;
    }
}

public interface IDragInteractable
{
    public void UseDraggable(Draggable drag);

    public bool CanUse(Draggable drag);

    public SortingData GetSortingOrder();
}
