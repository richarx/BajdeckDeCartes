using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Rarity { Common, Rare, Epic, Legendary }

public class CardInstance : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _artworkImage;

    public CardData Data { get; private set; }
    public Rarity Rarity { get; private set; }
    public int WearLevel { get; private set; }
    public ushort UUID { get; private set; }

    public void Initialize(CardData data, ushort uuid, Rarity rarity, int wearLevel)
    {
        Data = data;
        UUID = uuid;
        Rarity = rarity;
        WearLevel = wearLevel;
        if (_nameText != null) _nameText.text = Data.CardName;
        if (_descriptionText != null) _descriptionText.text = Data.Description;
        if (_artworkImage != null) _artworkImage.sprite = Data.Artwork;
    }
}
