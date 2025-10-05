using UnityEngine;
using UnityEngine.Events;

public class Shredder : MonoBehaviour, ICardInteractable
{
    [SerializeField] private UnityEvent<string> OnCardShredded;
    [SerializeField] private int _sortingOrder = 50;


    
    public void UseCard(Draggable card)
    {
        CardInstance cardInstance = card.GetComponent<CardInstance>();
            if (cardInstance != null)
            {
                string code = Conversion.ToCode(cardInstance, Resources.Load<BuildKey>("build_key")?.Value);
                Debug.Log($"Carte déchiquetée : {code} avec clé {Resources.Load<BuildKey>("build_key")?.Value}");
                ClipboardUtility.CopyToClipboard(code);
                OnCardShredded?.Invoke(code);
            }

            Destroy(card.gameObject);

    }


    public int GetSortingOrder()
    {
        return (_sortingOrder);
    }

}
