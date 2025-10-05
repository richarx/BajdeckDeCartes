using UnityEngine;

public class BinderButton : MonoBehaviour, GrabCursor.IInteractable
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

    public bool ShouldHover()
    {
        return (true);
    }
}
