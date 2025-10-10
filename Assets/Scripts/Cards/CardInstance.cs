using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public enum Quality { Normal, Gold, Holographic }

public class CardInstance : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] protected TMP_Text _nameText;
    [SerializeField] protected TMP_Text _descriptionText;
    [SerializeField] protected Image _artworkImage;
    [SerializeField] private List<GameObject> _wearLevels;
    [SerializeField] private List<GameObject> _Frames;
    [SerializeField] private List<GameObject> _Badges;
    [SerializeField] protected TMP_Text _numberText;

    public CardData Data { get; protected set; }
    public Quality Quality { get; protected set; }
    public int WearLevel { get; protected set; }
    public ushort UUID { get; protected set; }

    protected Draggable _draggable;

    void OnDestroy()
    {
        if (CardTableManager.Instance != null)
        {
            CardTableManager.Instance.Remove(_draggable);
        }
    }

    void Awake()
    {
        _draggable = GetComponent<Draggable>();
        Draggable.OnDragBegin += ShowNumber;
        Draggable.OnDragEnd += HideNumber;
    }

    public virtual void Initialize(CardData data, ushort uuid, Quality quality, int wearLevel)
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

        if (CardTableManager.Instance != null)
        {
            CardTableManager.Instance.PutAtTop(_draggable);
        }
        _numberText.text = Data.Number.ToString();
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
        if (CardTableManager.Instance != null)
        {
            CardTableManager.Instance.Remove(_draggable);
        }
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

    public void HideNumber(Draggable dragged)
    {
        if (_numberText != null)
            _numberText.gameObject.SetActive(false);
    }

    public void ShowNumber(Draggable dragged)
    {
        if (dragged == _draggable && _numberText != null)
        {
            _numberText.gameObject.SetActive(true);
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
                break;

            case Quality.Gold:
                _Frames[1].SetActive(true);
                break;

            case Quality.Holographic:
                _Frames[2].SetActive(true);
                break;

            default:
                _Frames[0].SetActive(true);
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
