using UnityEngine;
using UnityEngine.Events;

public class GenericDragInteractable : MonoBehaviour, IDragInteractable
{
    [SerializeField] private bool _canUse;
    [SerializeField] private SortingData _sortingData;
    [SerializeField] private UnityEvent<Draggable> _onDragHover = new();
    [SerializeField] private UnityEvent<Draggable> _onUseDraggable = new();

    public bool CanUse(Draggable drag)
    {
        return _canUse;
    }

    public void DragHover(Draggable drag)
    {
        _onDragHover.Invoke(drag);
    }

    public SortingData GetSortingOrder()
    {
        return _sortingData;
    }

    public void UseDraggable(Draggable drag)
    {
        _onUseDraggable.Invoke(drag);
    }
}
