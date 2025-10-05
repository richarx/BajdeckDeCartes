using UnityEngine;
using System.Collections.Generic;

public class Binder : MonoBehaviour, IDragInteractable, GrabCursor.IInteractable
{
    // TODO: Je n'aime pas ce lien fort entre les deux classe
    private CardTableManager _cardTable;

    [SerializeField] private int _sortingDraggableOrder = 1000;
    [SerializeField] private int _sortingInteractablePriority = 100;
    [SerializeField] private ArrowButton leftArrow = null;
    [SerializeField] private ArrowButton rightArrow = null;
    [SerializeField] private GameObject[] pages;

    private int _cardByPage = 9;
    private int _currentDoublePage = 0;
    private int _maxDoublePage = 0;
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

        _maxDoublePage = Mathf.FloorToInt((pages.Length - 1) / 2);
        _cardTable = FindFirstObjectByType<CardTableManager>();
        GoToDoublePage(0);
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

    public void Hover()
    {
        
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

    public void OpenAtPage(int indexPage)
    {
        _currentDoublePage = Mathf.FloorToInt(indexPage / 2);

        GoToDoublePage(_currentDoublePage);
    }

    public void GoToDoublePage(int doublePageIndex)
    {
        _currentDoublePage = doublePageIndex;


        pages[_currentDoublePage * 2].SetActive(true);
        pages[_currentDoublePage * 2 + 1].SetActive(true);

        if (_currentDoublePage == _maxDoublePage)
        {
            rightArrow.gameObject.SetActive(false);
        }
        else
        {
            rightArrow.gameObject.SetActive(true);
        }

        if (_currentDoublePage == 0)
        {
            leftArrow.gameObject.SetActive(false);
        }
        else
        {
            leftArrow.gameObject.SetActive(true);
        }
    }

    public void NextPage()
    {
        if (_currentDoublePage < _maxDoublePage)
        {
            pages[_currentDoublePage * 2].SetActive(false);
            pages[_currentDoublePage * 2 + 1].SetActive(false);
            
            GoToDoublePage(_currentDoublePage + 1);
        }
    }

    public void PreviousPage()
    {
        if (_currentDoublePage > 0)
        {
            pages[_currentDoublePage * 2].SetActive(false);
            pages[_currentDoublePage * 2 + 1].SetActive(false);

            GoToDoublePage(_currentDoublePage - 1);
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

    public bool CanHover() => false;

    public int GetSortingPriority()
    {
        return (_sortingInteractablePriority);   
    }
}
