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
    public BoxCollider2D HitBox => hitbox;

    private Rigidbody2D rb;

    public bool IsBeingDragged => isBeingDragged;
    private bool isBeingDragged;

    private SqueezeAndStretch squeeze;
    private CardSFX cardSFX;

    private List<Vector3> averageLastMovementList = new();

    public bool isActive = true;

    private string _saved_layer;

    private void Awake()
    {
        squeeze = GetComponent<SqueezeAndStretch>();
        hitbox = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        cardSFX = GetComponent<CardSFX>();

        initialScale = transform.localScale;
        targetScale = transform.localScale;
    }

    public SortingData GetSortingPriority()
    {
        return new SortingData(Canvas_.sortingOrder, Canvas_.sortingLayerID);
    }

    public bool CanHover() => true;

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


        hitbox.isTrigger = false;
        
        
        rb.angularVelocity = 0f;
        rb.linearVelocity = Vector2.zero;

        cardSFX.PickUpSound();

        transform.localRotation = Quaternion.identity;
    }

    public void Interact()
    {
        if (!isActive) return;

        averageLastMovementList.Clear();
        isBeingDragged = true;

        OnDragBegin?.Invoke(this);

        canvas.sortingOrder = -1;
        _saved_layer = canvas.sortingLayerName;
        Canvas_.sortingLayerName = "Cursor";

        DoPickupEffect();

        if (squeeze != null)
            squeeze.Trigger();
    }

    public void EndInteract()
    {
        hitbox.enabled = true;
        isBeingDragged = false;
        OnDragEnd?.Invoke(this);

        cardSFX.DropSound();

        Canvas_.sortingLayerName = _saved_layer;

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
        SetToScale(initialScale);
    }

    public void SetToScale(float scale)
    {
        targetScale = new Vector3(scale, scale, 1);
    }
    public void SetToScale(Vector3 scale)
    {
        targetScale = scale;
    }

    private void DefaultDropBehaviour()
    {
        Debug.LogWarning("No interactable under " + name);

        SlipOnTable();
        canvas.sortingOrder = 1000;

        canvas.sortingLayerID = SortingLayer.GetLayerValueFromName("Table");
    }

    public void Hover()
    {

    }


    private void TryToInteract()
    {
        int layerMask = ~(1 << LayerMask.NameToLayer("Card"));

        Collider2D[] results;

        results = Physics2D.OverlapBoxAll(this.transform.position, hitbox.size, 0, layerMask);

        int bestOrder = int.MinValue;
        IDragInteractable top = null;

        for (int i = 0; i < results.Length; i++)
        {
            var interact = results[i].GetComponentInParent<IDragInteractable>();
            if (interact == null) continue;

            if (!interact.CanUse(this)) continue;

            int sortingOrder = interact.GetSortingOrder().sortingOrder;
            if (sortingOrder > bestOrder)
            {
                bestOrder = sortingOrder;
                top = interact;
            }
        }

        if (top != null)
            top.UseDraggable(this);
        else
            this.DefaultDropBehaviour();
        // Remplit 'results' et renvoie le nombre
    }
}
