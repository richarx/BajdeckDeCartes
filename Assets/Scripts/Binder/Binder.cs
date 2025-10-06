using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Binder : MonoBehaviour, GrabCursor.IInteractable//, IDragInteractable
{

    public event Action OnSlotChanged;

    private BinderSFX binderSFX;

    //[SerializeField] private int _sortingDraggableOrder = 1000;
    [SerializeField] private CardGeneratorConfig _generatorConfig;
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

    // Attention, faut que le Binder soit inactif dans la scene pour que l'Awake marche bien
    private void Awake()
    {
        GetComponentsInChildren(true, _slots);

        _slots.Sort((a, b) => SortUtils.ByHierarchy(a.transform, b.transform));
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SlotIndex = i + 1;
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
        
        Save save = Save.Load<Save>();
        foreach (string code in save.slots)
        {
            CardInstance cardInstance = _generatorConfig.GenerateCard(code, save.GetKey()).GetComponent<CardInstance>();
            if (cardInstance != null)
            {
                TryToPutInSlot(cardInstance, false);
            }
            else
            {
                Debug.LogWarning($"Could not load card from code: {code}");
            }
        }
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

        binderSFX?.PlayOpenBookSounds();
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

    private bool TryToPutInSlot(CardInstance cardInstance, bool needSave = true)
    {
        Slot correctSlot = _slots.Find(x => x.SlotIndex == cardInstance.Data.Number);

        if (correctSlot == null)
        {
            return false;
        }

        var pageIndex = Mathf.FloorToInt(cardInstance.Data.Number / _cardByPage);
        // OpenAtPage(pageIndex);
        GoToDoublePage(Mathf.FloorToInt(pageIndex / 2));

        //Swith with quality
        if (correctSlot.CardInSlot == null || correctSlot.CardInSlot.Quality < cardInstance.Quality)
        {
            if (needSave)
            {
                Save save = Save.Load<Save>();
                if (correctSlot.CardInSlot != null)
                {
                    // TODO réimprimer la carte en passant ? (sans denied le code)
                    save.slots.Remove(Conversion.ToCode(cardInstance, save.GetKey()));
                }
                save.slots.Add(Conversion.ToCode(cardInstance, save.GetKey()));
                save.Save();
            }
            correctSlot.PutCardInSlot(cardInstance);
            OnSlotChanged?.Invoke();
            return true;
        }
        return false;
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

            if (!TryToPutInSlot(cardInstance))
            {
                Close();
                CardTableManager.Instance.UseDraggable(drag);
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
    [ContextMenu("Clear PlayerPrefs")]
    private void ClearSave()
    {
        Save.ClearPrefs();
    }

    public void Interact()
    {

    }

    public void EndInteract()
    {

    }

    public bool CanHover() => false;

    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SortingGroup>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    private class Save : SaveBase
    {
        [SerializeField] public List<string> slots = new();
        [SerializeField] private string _saltkey;

        protected override string PrefKey => "DontDestroyTreesOrYouWillBeSorry";

        protected override void OnSaveInitialization()
        {
            if (string.IsNullOrEmpty(_saltkey))
            {
                _saltkey = Guid.NewGuid().ToString();
            }
        }

        public string GetKey()
        {
            string salted = $"{_saltkey}_BBBB_!x7";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(salted);
            return System.Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }
    }
}
