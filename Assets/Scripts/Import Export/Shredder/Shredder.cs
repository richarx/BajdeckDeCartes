using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour, IDragInteractable
{
    [SerializeField] private UnityEvent<string> OnCardShredded;



    public void UseDraggable(Draggable card)
    {
        CardInstance cardInstance = card.GetComponent<CardInstance>();
        if (cardInstance != null && cardInstance.Data.Rarity != Rarity.Unique)
        {
            string code = Conversion.ToCode(cardInstance, Resources.Load<BuildKey>("build_key")?.Value);
            Debug.Log($"Shredded card code: {code}");
            ClipboardUtility.CopyToClipboard(code);
            Conversion.ExcludeCode(code);
            OnCardShredded?.Invoke(code);
        }

        Destroy(card.gameObject);

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
