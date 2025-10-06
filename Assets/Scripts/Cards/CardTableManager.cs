using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.VFX;
using System;


public class CardTableManager : MonoBehaviour, IDragInteractable
{
    public static CardTableManager instance;
    

    private List<Draggable> _onTable = new List<Draggable>();

    public void Awake()
    {
        Draggable.OnDragBegin += DragBegin;
        _onTable.AddRange(FindObjectsByType<Draggable>(FindObjectsSortMode.None));
        SortInList();

        CardTableManager.instance = this;
    }

    private void DragBegin(Draggable draggable)
    {
        int index = _onTable.FindIndex(x => x == draggable);

        if (index >= 0)
        {
            _onTable.RemoveAt(index);
        }

        SortInList();
    }

    public SortingData GetSortingOrder()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public bool CanUse(Draggable drag)
    {
        return (true);
    }

    public void UseDraggable(Draggable card)
    {
        _onTable.Add(card);

        card.SlipOnTable();
        card.Canvas_.sortingLayerName = "Table";
        card.Canvas_.sortingOrder = _onTable.Count - 1;
    }

    private void SortInList()
    {
        for (int i = 0; i < _onTable.Count; i++)
        {
            _onTable[i].Canvas_.sortingOrder = i;
        }
    }
}
