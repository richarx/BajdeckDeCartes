using UnityEngine;
using System.Collections.Generic;

public class Binder : MonoBehaviour, IDragInteractable, GrabCursor.IInteractable
{
    // TODO: Je n'aime pas ce lien fort entre les deux classe
    private CardTableManager _cardTable;

    [SerializeField] private int _sortingDraggableOrder = 1000;
    [SerializeField] private int _sortingInteractablePriority = 100;

    private int _cardByPage = 9;
    private int _currentDoublePage = 0;
    private List<Slot> _slots = new List<Slot>();

    public bool IsOpened { get { return (gameObject.activeInHierarchy); } } 

    private void Awake()
    {
        GetComponentsInChildren<Slot>(true, _slots);

        _slots.Sort((a, b) => SortUtils.ByHierarchy(a.transform, b.transform));
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SlotIndex = i;
        }

        var gridLayout = GetComponentInChildren<GridLayout2D>();
        if (gridLayout != null)
        {

            _cardByPage = gridLayout.Count;
        }
        else
        {
            _cardByPage = 9;
        }

        _cardTable = FindFirstObjectByType<CardTableManager>();
    }


    public void Open()
    {
        // Put anim here

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }


    public void OpenAtPage(int indexPage)
    {
        _currentDoublePage = Mathf.FloorToInt(indexPage / 2);
    }

    public void UseDraggable(Draggable drag)
    {
        var cardInstance = drag.GetComponent<CardInstance>();

        if (cardInstance != null)
        {
            CardData datas = cardInstance.Data;
            if (datas == null)
            {
                Debug.LogError("Card drop without datas");
                return;
            }
            Slot correctSlot = _slots.Find(x => x.SlotIndex == datas.Number);

            // TODO: Switch 9 to binded property
            var pageIndex = Mathf.FloorToInt(datas.Number / _cardByPage);

            OpenAtPage(pageIndex);

            if (correctSlot.CardInSlot == null || correctSlot.CardInSlot.Rarity < cardInstance.Rarity)
            {
                correctSlot.PutCardInSlot(cardInstance);
            }
            else
            {
                Close();
                _cardTable.UseDraggable(drag);
            }

        }
    }

    public bool CanUse(Draggable drag)
    {
        return (drag.GetComponent<CardInstance>());
    }

    public int GetSortingOrder()
    {
        return (_sortingDraggableOrder);
    }

    public void Interact()
    {
        
    }

    public void EndInteract()
    {

    }

    public bool ShouldHover() => false;

    public int GetSortingPriority()
    {
        return (_sortingInteractablePriority);   
    }
}
