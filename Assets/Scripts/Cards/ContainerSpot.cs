using UnityEngine;

public class ContainerSpot : MonoBehaviour, ICardInteractable
{
    [SerializeField] private int _sortingOrder = 100;


    private bool hasCard;
    private Draggable cardInSpot;
    private Vector2 smoothPosition;

    private void Awake()
    {

        Draggable.OnDragBegin += OnDragBegin;
    }

    private void OnDestroy()
    {
        Draggable.OnDragBegin -= OnDragBegin;
    }

    private void FixedUpdate()
    {
        if (cardInSpot != null)
            cardInSpot.transform.position = Vector2.SmoothDamp(cardInSpot.transform.position, transform.position, ref smoothPosition, 0.1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Draggable draggable = collision.gameObject.GetComponent<Draggable>();

        if (draggable == null)
            return;

        if (collision.CompareTag("Card") && collision.gameObject.GetComponent<Draggable>().IsBeingDragged == false && hasCard == false)
        {
            hasCard = true;
            PutCardInDisplay(draggable);
        }
    }

    public void OnDragBegin(Draggable draggable)
    {
        if (draggable == cardInSpot)
        {
            EmptyContainer();
        }
    }

    private void PutCardInDisplay(Draggable card)
    {
        cardInSpot = card;

        Rigidbody2D cardRB = card.GetComponent<Rigidbody2D>();

        if (cardRB != null)
        {
            cardRB.angularVelocity = 0f;
            cardRB.linearVelocity = Vector2.zero;
        }
        card.SetToInitialScale();

        card.transform.localRotation = Quaternion.identity;
        
    }

    private void EmptyContainer()
    {
        cardInSpot = null;
        hasCard = false;
    }

    public void UseCard(Draggable card)
    {
        PutCardInDisplay(card);
    }

    public int GetSortingOrder()
    {
        return (_sortingOrder);
    }
}
