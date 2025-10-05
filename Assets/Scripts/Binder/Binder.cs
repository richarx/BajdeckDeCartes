using UnityEngine;
using System.Collections.Generic;

public class Binder : MonoBehaviour, IDragInteractable
{
    [SerializeField] private int _sortingOrder = 1000;

    private const int _cardByPage = 9;


    private List<Slot> _slots = new List<Slot>();

    private void Awake()
    {
        GetComponentsInChildren<Slot>(true, _slots);

        _slots.Sort((a,b) => SortUtils.ByHierarchy(a.transform, b.transform));

    }
    

    public void Open()
    {

    }

    public void Close()
    {

    }


    public void OpenAtPage(int indexPage)
    {

    }

    public void UseDraggable(Draggable drag)
    {
        var cardInstance = drag.GetComponent<CardInstance>();

        if (cardInstance != null)
        {
            
        }
    }

    public bool CanUse(Draggable drag)
    {
        return (drag.GetComponent<CardInstance>());
    }

    public int GetSortingOrder()
    {
        return (_sortingOrder);
    }
}
