using UnityEngine;
using System.Collections.Generic;
using TMPro;
using MoreMountains.Feedbacks;


public class Slot : MonoBehaviour
{
    [SerializeField] TMP_Text _textObj;
    [SerializeField] MMF_Player _player;

    private int _slotIndex = -1;

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

        card.transform.SetParent(this.transform);

        Rigidbody2D cardRB = card.GetComponent<Rigidbody2D>();

        if (cardRB != null)
        {
            cardRB.angularVelocity = 0f;
            cardRB.linearVelocity = Vector2.zero;
        }

        Draggable draggable = card.GetComponent<Draggable>();
        draggable.isActive = false;
        draggable.SetToScale(scale);


        MMF_DestinationTransform destination = _player.FeedbacksList.Find(x => x is MMF_DestinationTransform) as MMF_DestinationTransform;
        destination.TargetTransform = card.transform;

        _player.PlayFeedbacks();
    }
}
