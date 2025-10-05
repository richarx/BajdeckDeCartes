using System.Collections.Generic;
using System;
using UnityEngine;

public class Binder : MonoBehaviour, GrabCursor.IInteractable//, IDragInteractable
{

    public event Action OnSlotChanged;

    private BinderSFX binderSFX;

    //[SerializeField] private int _sortingDraggableOrder = 1000;
    [SerializeField] private int _sortingInteractablePriority = 100;
    [SerializeField] private ArrowButton leftArrow = null;
    [SerializeField] private ArrowButton rightArrow = null;
    [SerializeField] private GameObject[] pages;

    private int _cardByPage = 9;
    private int _currentDoublePage = 0;
    private int _maxDoublePage = 0;
    private List<Slot> _slots = new List<Slot>();

    public List<Slot> Slots => _slots;
    public GameObject[] Pages => pages;

    public int CardByPage => _cardByPage;
    public bool IsOpened { get { return (gameObject.activeInHierarchy); } } 

    private void Awake()
    {
        GetComponentsInChildren(true, _slots);

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
        binderSFX = GetComponent<BinderSFX>();
        gameObject.SetActive(false);
    }

    private void Start()
    {

    }

    public void Open()
    {
        gameObject.SetActive(true);

        binderSFX.PlayOpenBookSounds();
    }

    public void Close()
    {
        gameObject.SetActive(false);

        binderSFX.PlayOpenBookSounds();
    }

    public void Hover()
    {

    }

    public void OpenForNumber(int number)
    {
        pages[_currentDoublePage * 2].SetActive(false);
        pages[_currentDoublePage * 2 + 1].SetActive(false);
        var pageIndex = Mathf.FloorToInt(number / _cardByPage);
        OpenAtPage(pageIndex);
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

            if (correctSlot == null)
            {
                CardTableManager.instance.UseDraggable(drag);
                return;
            }

            // TODO: Switch 9 to binded property
            var pageIndex = Mathf.FloorToInt(datas.Number / _cardByPage);
            // OpenAtPage(pageIndex);
            GoToDoublePage(Mathf.FloorToInt(pageIndex / 2));

            //Swith with quality
            if (correctSlot.CardInSlot == null || (correctSlot.CardInSlot != null && correctSlot.CardInSlot.Quality < cardInstance.Quality))
            {
                correctSlot.PutCardInSlot(cardInstance);
                OnSlotChanged?.Invoke();
            }
            else
            {
                Close();
                CardTableManager.instance.UseDraggable(drag);
            }

        }
    }

    public void OpenAtPage(int indexPage)
    {
        pages[_currentDoublePage * 2].SetActive(false);
        pages[_currentDoublePage * 2 + 1].SetActive(false);
        Open();
        GoToDoublePage(Mathf.FloorToInt(indexPage / 2));
    }

    public void GoToDoublePage(int doublePageIndex)
    {

        pages[_currentDoublePage * 2].SetActive(false);
        pages[_currentDoublePage * 2 + 1].SetActive(false);

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

        binderSFX.PlayTurnPageSound();
    }

    public void PreviousPage()
    {
        if (_currentDoublePage > 0)
        {
            pages[_currentDoublePage * 2].SetActive(false);
            pages[_currentDoublePage * 2 + 1].SetActive(false);

            GoToDoublePage(_currentDoublePage - 1);
        }

        binderSFX.PlayTurnPageSound();
    }

    //public bool CanUse(Draggable drag)
    //{
    //    return (drag.GetComponent<CardInstance>());
    //}

    //public int GetSortingOrder()
    //{
    //    return (_sortingDraggableOrder);
    //}

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
