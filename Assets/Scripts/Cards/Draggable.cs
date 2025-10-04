using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using static Unity.Collections.AllocatorManager;

public class Draggable : MonoBehaviour
{
    [HideInInspector] public static UnityEvent OnDragCard = new UnityEvent();
    [HideInInspector] public static UnityEvent OnDropCard = new UnityEvent();

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

    private Collider2D hitbox;
    private Rigidbody2D rb;

    public bool IsBeingDragged => isBeingDragged;
    private bool isBeingDragged;

    private SqueezeAndStretch squeeze;

    private List<Vector3> averageLastMovementList = new ();

    private void Start()
    {
        squeeze = GetComponent<SqueezeAndStretch>();
        hitbox = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        initialScale = transform.localScale;
        targetScale = transform.localScale;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref smoothZoom, smoothTimeZoomIn);

        if (isBeingDragged)
            FollowCursor();

        if (isBeingDragged && !GrabCursor.instance.IsGrabbing)
            Drop();

        if (rb.linearVelocity.magnitude > 0)
            rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, Vector2.zero, ref smoothrbVelocity, velocityDeceleration * Time.fixedDeltaTime);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (GrabCursor.instance.IsGrabbing &&  collider.CompareTag("Cursor") && !isBeingDragged)
            Drag();
    }

    private void Drag()
    {
        if (!CanGrap())
            return;

        OnDragCard.Invoke();

        averageLastMovementList.Clear();
        isBeingDragged = true;

        DoPickupEffect();

        if (squeeze != null)
            squeeze.Trigger();
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

    private void Drop()
    {
        hitbox.enabled = true;
        isBeingDragged = false;
        targetScale = initialScale;

        rb.linearVelocity = distancetoPosition;

        OnDropCard.Invoke();

        Vector2 total = Vector2.zero;
        foreach (Vector2 value in averageLastMovementList)
        {
            total += value;
        }

        rb.AddForceAtPosition(total / averageLastMovementList.Count, transform.position * rotationAcceleration);
    }

    private bool CanGrap()
    {
        return GrabCursor.instance.IsGrabbing == true;
    }
}
