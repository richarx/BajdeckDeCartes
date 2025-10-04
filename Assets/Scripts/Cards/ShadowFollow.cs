using UnityEngine;

public class ShadowFollow : MonoBehaviour
{
    [SerializeField] private float offsetDistance;
    [SerializeField] private Vector2 direction;
    [SerializeField] private float smoothSpeed;

    private Vector2 smoothPosition;

    private float targetOffsetDistance;

    [SerializeField] private Draggable draggableScript;

    private bool hasDragged;

    private void LateUpdate()
    {
        if (draggableScript.IsBeingDragged && hasDragged != draggableScript.IsBeingDragged)
            StartPickUp();

        if (!draggableScript.IsBeingDragged && hasDragged != draggableScript.IsBeingDragged)
            StopPickUp();

        transform.position = Vector2.SmoothDamp(transform.position, (Vector2)transform.parent.position + direction * targetOffsetDistance, ref smoothPosition, smoothSpeed);
    }

    private void StartPickUp()
    {
        hasDragged = draggableScript.IsBeingDragged;
        targetOffsetDistance = offsetDistance;
    }

    private void StopPickUp()
    {
        hasDragged = draggableScript.IsBeingDragged;
        targetOffsetDistance = 0f;
    }
}
