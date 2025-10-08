using UnityEngine;
using UnityEngine.Events;

public class GenericInteractable : MonoBehaviour, GrabCursor.IInteractable
{
    [SerializeField] private bool _canHover;
    [SerializeField] private SortingData _sortingData;
    [SerializeField] private UnityEvent _onInteract = new();
    [SerializeField] private UnityEvent _onHover = new();
    [SerializeField] private UnityEvent _onEndInteract = new();

    public bool CanInteract()
    {
        return _canHover;
    }

    public void EndHover()
    {
    }

    public void EndInteract()
    {
        _onEndInteract.Invoke();
    }

    public SortingData GetSortingPriority()
    {
        return _sortingData;
    }

    public void Hover()
    {
        _onHover.Invoke();
    }

    public void Interact()
    {
        _onInteract.Invoke();
    }
}
