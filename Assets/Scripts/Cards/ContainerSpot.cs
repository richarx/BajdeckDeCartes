using UnityEngine;

public class ContainerSpot : MonoBehaviour
{
    private bool hasCard;
    private GameObject cardInSpot;

    private Vector2 smoothPosition;

    private void FixedUpdate()
    {
        if (cardInSpot != null)
            cardInSpot.transform.position = Vector2.SmoothDamp(cardInSpot.transform.position, transform.position, ref smoothPosition, 0.1f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Card") && collision.gameObject.GetComponent<Draggable>().IsBeingDragged == false && hasCard == false)
        {
            hasCard = true;
            PutCardInDisplay(collision.gameObject);
        }
    }

    private void PutCardInDisplay(GameObject card)
    {
        cardInSpot = card;

        Rigidbody2D cardRB = card.GetComponent<Rigidbody2D>();

        if (cardRB != null)
        {
            cardRB.angularVelocity = 0f;
            cardRB.linearVelocity = Vector2.zero;
        }

        card.transform.localRotation = Quaternion.identity;

        card.GetComponent<Draggable>().OnDragCard.AddListener(EmptyContainer);
    }

    private void EmptyContainer()
    {
        cardInSpot.GetComponent<Draggable>().OnDragCard.RemoveAllListeners();

        cardInSpot = null;
        hasCard = false;
    }
}
