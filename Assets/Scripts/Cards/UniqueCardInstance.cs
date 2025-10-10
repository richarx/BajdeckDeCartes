using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Cysharp.Threading.Tasks.Triggers;
using MoreMountains.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UniqueCardInstance : CardInstance
{
    public override void Initialize(CardData data, ushort uuid, Quality quality, int wearLevel)
    {
        Data = data;
        UUID = uuid;
        Quality = Quality.Holographic;
        WearLevel = 0;
        if (_nameText != null) _nameText.text = Data.CardName;
        if (_descriptionText != null) _descriptionText.text = Data.Description;
        if (_artworkImage != null) _artworkImage.sprite = Data.Artwork;
        if (_numberText != null) _numberText.text = Data.Number.ToString();

        if (CardTableManager.Instance != null)
        {
            CardTableManager.Instance.PutAtTop(_draggable);
        }
    }
}
