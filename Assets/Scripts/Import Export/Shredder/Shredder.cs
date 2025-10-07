using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour, IDragInteractable
{
    [SerializeField] private UnityEvent<string> OnCardShredded;
    [SerializeField] private ShredderAnimation animator;

    private CardInstance cardInstance;
    private bool isShredding; 

    private void Start()
    {

    }

    public void UseDraggable(Draggable card)
    {
        cardInstance = card.GetComponent<CardInstance>();
        if (cardInstance != null && cardInstance.Data.Rarity != Rarity.Unique && !isShredding)
        {
            Shredd(card).Forget();
        }
        else
        {
            CardTableManager.Instance.UseDraggable(card);
        }
    }

    private async UniTaskVoid Shredd(Draggable card)
    {
        isShredding = true;

        string code = Conversion.ToCode(cardInstance, Resources.Load<BuildKey>("build_key")?.Value);
        animator.StartShredding(card);

        await ShredderAnimation.OnEndShredding;

        Debug.Log($"Shredded card code: {code}");
        ClipboardUtility.CopyToClipboard(code);
        Conversion.ExcludeCode(code);
        OnCardShredded?.Invoke(code);
        Destroy(card.gameObject);

        isShredding = false;
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

}
