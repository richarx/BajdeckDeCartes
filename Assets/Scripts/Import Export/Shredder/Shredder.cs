using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour, IDragInteractable
{
    [SerializeField] private UnityEvent<string> OnCardShredded;
    [SerializeField] private ShredderAnimation animator;

    private bool isShredding;
    private Save _save = null;

    private void Awake()
    {
        _save = Save.Load<Save>();
    }

    public void UseDraggable(Draggable card)
    {
        CardInstance cardInstance = card.GetComponent<CardInstance>();
        if (cardInstance != null && cardInstance.Data.Rarity != Rarity.Unique && !isShredding)
        {
            Shredd(cardInstance, card).Forget();
        }
        else
        {
            CardTableManager.Instance.UseDraggable(card);
        }
    }

    private async UniTaskVoid Shredd(CardInstance cardInstance, Draggable card)
    {
        isShredding = true;

        string code = Conversion.ToCode(cardInstance, Resources.Load<BuildKey>("build_key")?.Value);
        animator.StartShredding(card);

        await ShredderAnimation.OnEndShredding;

        Debug.Log($"Shredded card code: {code}");
        if (cardInstance)
        {
            _save.codeAndInfos.Add(new() { code = code, cardData = cardInstance.Data, quality = cardInstance.Quality, wearLevel = cardInstance.WearLevel });
            _save.Save();
        }
        Conversion.ExcludeCode(code);
        PutLastCodeInClipboard();
        History.Log(LogType.ShredderCode, $"{cardInstance.Data.CardName}:\n{code}");
        OnCardShredded?.Invoke(code);
        Destroy(card.gameObject);

        isShredding = false;
    }

    public void PutLastCodeInClipboard()
    {
        Save.CodeAndInfo codeinfo = _save.codeAndInfos.LastOrDefault();
        if (codeinfo != null)
        {
            Debug.Log($"Put card code in clipboard: {codeinfo.code}");
            ClipboardUtility.CopyToClipboard(codeinfo.code);
        }
    }

    public bool CanUse(Draggable draggable)
    {
        return (draggable.GetComponent<CardInstance>() != null);
    }

    public SortingData GetSortingOrder()
    {
        var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public void DragHover(Draggable drag)
    {
    }

    [Serializable]
    private class Save : SaveBase
    {
        [Serializable]
        public class CodeAndInfo
        {
            public string code;
            public CardData cardData;
            public Quality quality;
            public int wearLevel;
        }
        [SerializeField] public List<CodeAndInfo> codeAndInfos = new();
        protected override string PrefKey => "tOnlyTheBravesCanHandleSpitWater";
    }

}
