using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour, IDragInteractable
{
    [SerializeField] private UnityEvent<string> OnCardShredded;
    [SerializeField] private int _sortingOrder = 50;



    public void UseDraggable(Draggable card)
    {
        CardInstance cardInstance = card.GetComponent<CardInstance>();
        if (cardInstance != null)
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

    public int GetSortingOrder()
    {
        return (_sortingOrder);
    }

}
