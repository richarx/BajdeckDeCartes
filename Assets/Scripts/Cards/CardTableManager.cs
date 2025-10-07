using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;


public class CardTableManager : MonoBehaviour, IDragInteractable
{
    private static float BETWEEN_SPACE = 0.01f;
    public static CardTableManager Instance { get; private set; } = null;

    [SerializeField] private CardGeneratorConfig _generatorConfig;

    private List<Draggable> _onTable = new List<Draggable>();
    private List<CardInstance> _cards = new List<CardInstance>();

    private bool _isDirty = false;
    private static Save _save = null;

    public Bounds Bounds { get; private set; }

    private void InstanciateCards()
    {
        if (_save == null)
        {
            _save = Save.Load<Save>();
        }
        foreach (var cardPos in _save.cards)
        {
            CardInstance cardInstance = _generatorConfig.GenerateCard(cardPos.code, _save.GetKey())?.GetComponent<CardInstance>();
            if (cardInstance != null)
            {
                cardInstance.transform.position = cardPos.position;
                cardInstance.transform.eulerAngles = cardPos.rotation;
                cardInstance.transform.SetParent(CardParentSingleton.instance.transform, true);
                PutAtTop(cardInstance.GetComponent<Draggable>());
            }
        }
    }

    private void SaveState()
    {
        if (_save == null)
        {
            _save = Save.Load<Save>();
        }
        _save.cards.Clear();
        foreach (var card in _cards)
        {
            _save.cards.Add(new Save.CardPos()
            {
                code = Conversion.ToCode(card, _save.GetKey()),
                position = card.transform.position,
                rotation = card.transform.eulerAngles
            });
        }
        _save.Save();
    }

    void OnApplicationQuit()
    {
        Instance = null;
    }

    public void Start()
    {
        InstanciateCards();
        Instance = this;
    }

    public void Awake()
    {
        Bounds = GetComponent<BoxCollider2D>().bounds;
        Draggable.OnDragBegin += DragBegin;
        _onTable.AddRange(FindObjectsByType<Draggable>(FindObjectsSortMode.None)); // TODO prendre les cartes a part aussi
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        MoveUp(0);
    }

    private void DragBegin(Draggable draggable)
    {
        Remove(draggable);
    }

    public SortingData GetSortingOrder()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public bool CanUse(Draggable draggable)
    {
        return (true);
    }

    public void UseDraggable(Draggable draggable)
    {
        draggable.SlipOnTable();
        PutAtTop(draggable);
    }

    public void PutAtTop(Draggable draggable)
    {
        draggable.Canvas_.sortingLayerName = "Table";
        CardInstance cardInstance = draggable.GetComponent<CardInstance>();
        // r�cup�rer la position de la cardIstance dans la liste
        int index = _onTable.IndexOf(draggable);
        if (index >= 0)
        {
            _onTable.RemoveAt(index);
            MoveUp(index);
            if (cardInstance != null)
            {
                _cards.Remove(cardInstance);
            }
        }
        _onTable.Add(draggable);
        if (cardInstance != null)
        {
            _cards.Add(cardInstance);
        }
        SetZAndLayer(_cards.Count - 1);
        _isDirty = true;
    }

    void LateUpdate()
    {
        if (_isDirty)
        {
            SaveState();
            _isDirty = false;
        }
    }

    public void Remove(Draggable draggable)
    {
        int index = _onTable.IndexOf(draggable);

        if (index >= 0)
        {
            _onTable.RemoveAt(index);
            if (draggable.TryGetComponent(out CardInstance cardInstance))
            {
                _cards.Remove(cardInstance);
            }
            MoveUp(index);
            _isDirty = true;
        }
    }

    private void SetZAndLayer(int index)
    {
        _onTable[index].Canvas_.sortingOrder = index;

        Vector3 pos = _onTable[index].transform.position;
        pos.z = BETWEEN_SPACE * -1 * (index - 1); // d�cale d'une carte vers l'avant (donc vers le n�gatif)
        _onTable[index].transform.position = pos;
    }

    private void MoveUp(int index)
    {
        for (; index < _onTable.Count; index++)
        {
            SetZAndLayer(index);
        }
    }


    [ContextMenu("Clear PlayerPrefs")]
    private void ClearSave()
    {
        Save.ClearPrefs();
    }


    private class Save : SaveBase
    {
        [Serializable]
        public class CardPos
        {
            public Vector3 position;
            public Vector3 rotation;
            public string code;
        }

        [SerializeField] public List<CardPos> cards = new();
        [SerializeField] private string _saltkey;
        protected override string PrefKey => "MoustiqueAreOnTheWayToPutOutTheFire";

        protected override void OnSaveInitialization()
        {
            if (string.IsNullOrEmpty(_saltkey))
            {
                _saltkey = Guid.NewGuid().ToString();
            }
        }

        public string GetKey()
        {
            string salted = $"{_saltkey}_AAAA_!x7";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(salted);
            return System.Convert.ToBase64String(bytes)
                .Replace("=", "")
                .Replace("+", "-")
                .Replace("/", "_");
        }
    }
}