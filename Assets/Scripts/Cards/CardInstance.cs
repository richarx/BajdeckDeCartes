using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private List<GameObject> _wearLevels;
    [SerializeField] private List<GameObject> _Frames;
    [SerializeField] private List<GameObject> _Badges;

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

        UpdateWearLevel(wearLevel);
        UpdateFrame(quality);
        UpdateBadge(data.Rarity);
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

    private void UpdateWearLevel(int wearLevel)
    {
        foreach (GameObject _wearLevel in _wearLevels)
        {
            _wearLevel.SetActive(false);
        }

        System.Random random = new System.Random();
        _wearLevels.OrderBy(x => random.Next()).ToArray();

        int i = 0;
        while (i < wearLevel && i < _wearLevels.Count)
        {
            _wearLevels[i].SetActive(true);
            i++;
        }
    }

    private void UpdateFrame(Quality quality)
    {
        foreach (GameObject frame in _Frames)
        {
            frame.SetActive(false);
        }

        switch (quality)
        {
            case Quality.Normal:
                _Frames[0].SetActive(true);
                _nameText.color = new Color(40, 45, 60); //DarkBlue from palette
                _descriptionText.color = new Color(40, 45, 60);
                break;
            
            case Quality.Gold:
                _Frames[1].SetActive(true);
                _nameText.color = new Color(247, 146, 146); //Pink from palette
                _descriptionText.color = new Color(247, 146, 146);
                break;
            
            case Quality.Holographic:
                _Frames[2].SetActive(true);
                _nameText.color = new Color(247, 146, 146); //Pink from palette
                _descriptionText.color = new Color(247, 146, 146);
                break;

            default:
                _Frames[0].SetActive(true);
                _nameText.color = new Color(40, 45, 60); //DarkBlue from palette
                _descriptionText.color = new Color(40, 45, 60);
                break;
        }
    }

    private void UpdateBadge(Rarity rarity)
    {
        foreach (GameObject badge in _Badges)
        {
            badge.SetActive(false);
        }

        switch (rarity)
        {
            case Rarity.Common:
                _Badges[0].SetActive(true);
                break;

            case Rarity.Rare:
                _Badges[1].SetActive(true);
                break;

            case Rarity.Epic:
                _Badges[2].SetActive(true);
                break;

            case Rarity.Legendary:
                _Badges[3].SetActive(true);
                break;

            default:
                _Badges[0].SetActive(true);
                break;
        }
    }
}
