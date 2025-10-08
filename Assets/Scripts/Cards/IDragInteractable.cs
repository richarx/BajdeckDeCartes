using System;
using UnityEngine;

[Serializable]
public class SortingData
{
    public int _sortingOrder;
    public int SortingOrder => _sortingOrder;
    public int _sortingLayerId;
    public int SortingLayerId => _sortingLayerId;

    public SortingData(int sortingOrder, int sortingLayerId)
    {
        _sortingOrder = sortingOrder;
        _sortingLayerId = sortingLayerId;
    }
}

public interface IDragInteractable
{
    public void UseDraggable(Draggable drag);

    public bool CanUse(Draggable drag);
    public void DragHover(Draggable drag);

    public SortingData GetSortingOrder();
}
