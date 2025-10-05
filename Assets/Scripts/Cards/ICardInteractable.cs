using UnityEngine;

public interface ICardInteractable
{
    public void UseCard(Draggable card);

    public int GetSortingOrder();
}
