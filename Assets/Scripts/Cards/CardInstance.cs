using System;
using System.Security.Cryptography;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public enum Quality { Normal, Gold, Holographic }

public class CardInstance : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _artworkImage;

    public CardData Data { get; private set; }
    public Quality Quality { get; private set; }
    public int WearLevel { get; private set; }
    public ushort UUID { get; private set; }

    public void Initialize(CardData data, ushort uuid, Quality quality, int wearLevel)
    {
        Data = data;
        UUID = uuid;
        Quality = quality;
        WearLevel = wearLevel;
        if (_nameText != null) _nameText.text = Data.CardName;
        if (_descriptionText != null) _descriptionText.text = Data.Description;
        if (_artworkImage != null) _artworkImage.sprite = Data.Artwork;
    }

    private int _DEBUGNumber = 0;

    [ContextMenuItem("Assign index", nameof(DEBUGSetIndex))]
    public int newIndex; // affiche un bouton � droite du champ

    public void DEBUGSetIndex()
    {
        Data = new CardData();
        Data.DEBUGSetNumber(newIndex);
    }

    public void Store()
    {
        _canvas.sortingLayerName = "Binded";
        _canvas.sortingOrder = 10;
    }
}
