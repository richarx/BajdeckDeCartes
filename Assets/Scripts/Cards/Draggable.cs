using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;
using static Unity.Collections.AllocatorManager;



public class Draggable : MonoBehaviour, GrabCursor.IInteractable
{
    public static event Action<Draggable> OnDragBegin;
    public static event Action<Draggable> OnDragEnd;

    [SerializeField] private Canvas canvas;
    public Canvas Canvas_ => canvas;

    [SerializeField] private float verticalOffset;
    [SerializeField] private float smoothTimeFollowCursor;

    [SerializeField] private float zoomedScale;
    [SerializeField] private float smoothTimeZoomIn;

    [SerializeField] private float velocityDeceleration;
    [SerializeField] private float rotationAcceleration;

    private Vector3 initialScale;
    private Vector3 targetScale;

    private Vector3 smoothPosition;
    private Vector3 smoothZoom;
    private Vector2 smoothrbVelocity;

    private Vector3 targetPosition;
    private Vector3 currentPosition;
    private Vector3 distancetoPosition;
    private Vector3 previousPosition;

    private BoxCollider2D hitbox;
    private Rigidbody2D rb;

    public bool IsBeingDragged => isBeingDragged;
    private bool isBeingDragged;

    private SqueezeAndStretch squeeze;

    private List<Vector3> averageLastMovementList = new();

    private void Start()
    {

        squeeze = GetComponent<SqueezeAndStretch>();
        hitbox = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        initialScale = transform.localScale;
        targetScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref smoothZoom, smoothTimeZoomIn);

        if (isBeingDragged)
            FollowCursor();

        if (rb.linearVelocity.magnitude > 0)
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, Vector2.zero, ref smoothrbVelocity, velocityDeceleration * Time.fixedDeltaTime);
    }

    private void FollowCursor()
    {
        targetPosition = GrabCursor.instance.transform.position + new Vector3(0, verticalOffset, 0);

        previousPosition = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref smoothPosition, smoothTimeFollowCursor);
        currentPosition = transform.position;

        Vector3 velocity = (previousPosition - transform.position) / Time.deltaTime;
        AddVelocityForAverage(velocity);

        distancetoPosition = targetPosition - currentPosition;

        rb.linearVelocity = Vector2.zero;
    }

    private void AddVelocityForAverage(Vector3 velocity)
    {
        int maxRange = 10;

        averageLastMovementList.Add(velocity);

        if (averageLastMovementList.Count >= maxRange)
            averageLastMovementList.RemoveAt(0);
    }

    private void DoPickupEffect()
    {
        targetScale = initialScale * zoomedScale;
        hitbox.enabled = false;

        rb.angularVelocity = 0f;
        rb.linearVelocity = Vector2.zero;

        transform.localRotation = Quaternion.identity;
    }

    public void Interact()
    {
        averageLastMovementList.Clear();
        isBeingDragged = true;
        OnDragBegin?.Invoke(this);

        canvas.sortingOrder = 100;
        Canvas_.sortingLayerName = "Highlighted";

        DoPickupEffect();

        if (squeeze != null)
            squeeze.Trigger();
    }

    public void EndInteract()
    {
        hitbox.enabled = true;
        isBeingDragged = false;
        OnDragEnd?.Invoke(this);
        TryToInteract();


    }

    public void SlipOnTable()
    {
        targetScale = initialScale;

        rb.linearVelocity = distancetoPosition;


        Vector2 total = Vector2.zero;
        foreach (Vector2 value in averageLastMovementList)
        {
            total += value;
        }

        if (averageLastMovementList.Count > 0)
            rb.AddForceAtPosition(total / averageLastMovementList.Count, transform.position * rotationAcceleration);
    }

    public void SetToInitialScale()
    {
        targetScale = initialScale;
    }

    private void DefaultDropBehaviour()
    {
        Debug.LogWarning("No interactable under " + name);

        SlipOnTable();
        canvas.sortingOrder = 100;

        canvas.sortingLayerID = SortingLayer.GetLayerValueFromName("Table");
        canvas.sortingOrder -= 10;
    }

    private void TryToInteract()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Card"));

        Collider2D[] results;

        results = Physics2D.OverlapBoxAll(this.transform.position, hitbox.size, 0, layerMask);

        
        int bestOrder = int.MinValue;
        ICardInteractable top = null;
        
        for (int i = 0; i < results.Length; i++)
        {
            var c = results[i];
            if (c == null) continue;

            var interact = c.GetComponentInParent<ICardInteractable>();
            if (interact == null) continue;

            int so = interact.GetSortingOrder();
            if (so > bestOrder)
            {
                bestOrder = so;
                top = interact;
            }
        }


        if (top != null)
            top.UseCard(this);
        else
            this.DefaultDropBehaviour();
        // Remplit 'results' et renvoie le nombre
    }
}
