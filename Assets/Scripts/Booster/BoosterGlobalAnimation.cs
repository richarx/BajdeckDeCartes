using DG.Tweening;
using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoosterGlobalAnimation : MonoBehaviour
{
    [HideInInspector] public static UnityEvent<Rarity> OnDiscoverCard = new UnityEvent<Rarity>();

    [SerializeField] SpriteRenderer background;
    [SerializeField] SpriteRenderer lueur;
    [SerializeField] MMF_Player lueurSequencer;

    [SerializeField] Color colorCommon = new Color(1, 1, 1, 1);
    [SerializeField] Color colorRare = new Color(1, 1, 1, 1);
    [SerializeField] Color colorEpic = new Color(1, 1, 1, 1);
    [SerializeField] Color colorLegendary = new Color(1, 1, 1, 1);

    public static BoosterGlobalAnimation instance;
    
    private void Awake()
    {
        instance = this;
        BoosterOpening.OnFinishOpeningPack.AddListener(OnBoosterOpened);
    }

    private List<CardInstance> cardInWaitingRoom = new List<CardInstance>();
    public int numberofCardsInWaitingRoom => cardInWaitingRoom.Count;

    public bool IsBeingDisplayed => background != null && background.gameObject.activeSelf;
    
    public void OnBoosterOpened(List<CardInstance> cardInstance)
    {
        if (background != null)
            background.gameObject.SetActive(true);
        cardInWaitingRoom.AddRange(cardInstance);

        FLASH(cardInWaitingRoom[cardInWaitingRoom.Count - 1]);
        Draggable.OnDragBegin -= NextCard;
        Draggable.OnDragBegin += NextCard;
    }

    private void FLASH(CardInstance cardInstance)
    {
        if (lueur == null )
            return;

        switch (cardInstance.Data.Rarity)
        {
            case Rarity.Common:
                lueur.color = colorCommon;
                break;
            case Rarity.Rare:
                lueur.color = colorRare;
                break;
            case Rarity.Epic:
                lueur.color = colorEpic;
                break;
            case Rarity.Legendary:
                lueur.color = colorLegendary;
                break;
        }
        lueur.color = new Color(lueur.color.r, lueur.color.g, lueur.color.b, 1);
        lueur.DOFade(0, 0.5f);
        lueur.transform.localScale = new Vector3(0, 0, 1);
        lueur.transform.DOScale(9, 0.5f);
    }

    public void NextCard(Draggable draggable)
    {
        var cardInstance = draggable.GetComponent<CardInstance>();
        if (cardInstance != null && cardInWaitingRoom.Exists(x => x == cardInstance))
        {
            cardInWaitingRoom.Remove(cardInstance);
            if (cardInWaitingRoom.Count > 0)
            {
                FLASH(cardInWaitingRoom[cardInWaitingRoom.Count - 1]);
            }
            else
            {
                Draggable.OnDragBegin -= NextCard;
                if (background != null)
                    background.gameObject.SetActive(false);
            }
            OnDiscoverCard.Invoke(cardInstance.Data.Rarity);
        }
    }
}
