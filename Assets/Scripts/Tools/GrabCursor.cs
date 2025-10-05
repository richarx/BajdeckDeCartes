using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabCursor : MonoBehaviour
{
    public static GrabCursor instance;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite isGrabbingHand;
    [SerializeField] private Sprite openHand;
    [SerializeField] private Sprite pointingFinger;

    [SerializeField] private Transform fingerPosition;

    private Camera mainCamera;
    private SqueezeAndStretch squeeze;
    private Collider2D hitbox;

    private IInteractable isGrabbing;

    private bool isHovering;

    public interface IInteractable
    {
        void Interact();
        void EndInteract();
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        Cursor.visible = false;

        squeeze = GetComponent<SqueezeAndStretch>();
        hitbox = GetComponent<Collider2D>();
    }

    private void Update()
    {
        FollowCursor();

        if (isGrabbing != null && Mouse.current.leftButton.isPressed == false)
        {
            isGrabbing.EndInteract();
            isGrabbing = null;
            UpdateGraphicsState();
        }
    }

    private void FixedUpdate()
    {
        if (isGrabbing != null)
            return;

        Collider2D hitbox = Physics2D.OverlapPoint(fingerPosition.position, 1<< LayerMask.NameToLayer("Card"));

        if (hitbox != null)
        {
            IInteractable interactable = hitbox.GetComponent<IInteractable>();

            if (interactable != null && Mouse.current.leftButton.isPressed)
            {
                isGrabbing = interactable;
                isGrabbing.Interact();
                UpdateGraphicsState();
                return;
            }
            else if (interactable != null)
            {
                if (isHovering == false)
                {
                    isHovering = true;
                    UpdateGraphicsState();
                }
                return;
            }
        }

        if (isHovering == true)
        {
            isHovering = false;
            UpdateGraphicsState();
        }
    }

    private void UpdateGraphicsState()
    {
        if (isGrabbing != null)
            spriteRenderer.sprite = isGrabbingHand;
        else if (isHovering)
            spriteRenderer.sprite = openHand;
        else
            spriteRenderer.sprite = pointingFinger;

        if (squeeze != null)
            squeeze.Trigger();
    }

    private void FollowCursor()
    {
        Vector3 cursorPixelPosition = Input.mousePosition;
        Vector3 cursorScreenPosition = new Vector3(cursorPixelPosition.x, cursorPixelPosition.y, 10);

        transform.position = mainCamera.ScreenToWorldPoint(cursorScreenPosition);
    }
}