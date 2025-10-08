using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;


public class Slot : MonoBehaviour
{
    [SerializeField] TMP_Text _textObj;
    [SerializeField] MMF_Player _player;

    private int _slotIndex = -1;
    private Transform _oldParent = null;

    // TODO Put Somewhere else
    private const float scale = 2f;

    public CardInstance CardInSlot { get; private set; } = null;


    public int SlotIndex
    {
        get { return (_slotIndex); }
        set { _slotIndex = value; UpdateDisplay(); }
    }

    public void UpdateDisplay()
    {
        _textObj.text = _slotIndex.ToString();

    }


    public void PutCardInSlot(CardInstance card)
    {
        CardInSlot = card;

        _oldParent = card.transform.parent;
        card.transform.SetParent(this.transform, true);

        Rigidbody2D cardRB = card.GetComponent<Rigidbody2D>();

        if (cardRB != null)
        {
            cardRB.angularVelocity = 0f;
            cardRB.linearVelocity = Vector2.zero;
        }

        Collider2D collider2D = card.GetComponent<Collider2D>();
        collider2D.isTrigger = true;
        //collider2D.enabled = false;

        card.transform.localRotation = Quaternion.identity;

        Draggable draggable = card.GetComponent<Draggable>();

        Draggable.OnDragBegin += TryEmptySlot;
        //draggable.isActive = false;
        draggable.SetToScale(scale);
        draggable.transform.localScale = new Vector3(scale, scale, 1);


        card.transform.localPosition = Vector3.zero;
        card.Store();
    }

    private void TryEmptySlot(Draggable draggable)
    {
        var cardInstance = draggable.GetComponent<CardInstance>();
        if (cardInstance == CardInSlot)
        {
            EmptySlot();
        }
    }

    public CardInstance EmptySlot()
    {
        CardInstance cardInstance = CardInSlot;
        if (CardInSlot != null)
        {
            CardInSlot.transform.SetParent(CardParentSingleton.instance.transform, true);
            CardInSlot = null;
            Draggable.OnDragBegin -= TryEmptySlot;
            Binder.Instance.SaveSlotRemove(this, cardInstance);
        }
        return cardInstance;
    }
}
