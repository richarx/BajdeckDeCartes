using System;
using System.Collections;
using System.Linq;
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

    private Draggable _draggable;
    public Draggable Draggable => _draggable;

    private IInteractable _hovering = null;
    public IInteractable Hovering => _hovering;

    public interface IInteractable
    {
        void Interact();
        void EndInteract();
        public bool CanInteract();

        public void Hover();
        public void EndHover();

        public SortingData GetSortingPriority();
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        squeeze = GetComponent<SqueezeAndStretch>();
        hitbox = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.visible = true;

        isGrabbing?.EndInteract();
        _draggable = null;
        isGrabbing = null;
        UpdateGraphicsState();
    }

    private void Update()
    {
        FollowCursor();

        if (isGrabbing != null)
        {
            if (Pointer.current.press.isPressed == false)
            {
                isGrabbing.EndInteract();
                _draggable = null;
                isGrabbing = null;
                UpdateGraphicsState();
            }
        }
        else
        {
            Collider2D[] hitboxs = Physics2D.OverlapPointAll(fingerPosition.position, ~0);

            var top = hitboxs.Select(collider => new { collider, interactable = collider.GetComponent<IInteractable>() }).Where(x => x.interactable != null)
                .Select(x => new { x.collider, x.interactable, sortingData = x.interactable.GetSortingPriority() })
                .Select(x => new { x.collider, x.interactable, x.sortingData.SortingOrder, layerValue = SortingLayer.GetLayerValueFromID(x.sortingData.SortingLayerId) })
                .OrderByDescending(x => x.layerValue)
                .ThenByDescending(x => x.SortingOrder)
                .ThenByDescending(x => x.collider.transform, new ComparerHierarchy())
                .FirstOrDefault();

            IInteractable topInteractable = top?.interactable;

            if (topInteractable != null)
            {
                if (Pointer.current.press.isPressed)
                {
                    isGrabbing = topInteractable;
                    _draggable = top.collider.GetComponent<Draggable>();
                    isGrabbing.Interact();
                    UpdateGraphicsState();
                }
                else if (Mouse.current.rightButton.isPressed)
                {
                    Draggable draggable = top.collider.GetComponent<Draggable>();
                    if (draggable != null)
                        draggable.Spin();
                }
                else
                {
                    if (_hovering != topInteractable && topInteractable.CanInteract())
                    {
                        _hovering?.EndHover();
                        topInteractable.Hover();
                        _hovering = topInteractable;
                        UpdateGraphicsState();
                    }
                }
            }
            else if (_hovering != null)
            {
                _hovering.EndHover();
                _hovering = null;
                UpdateGraphicsState();
            }
        }
    }

    private void UpdateGraphicsState()
    {
        if (isGrabbing != null)
            spriteRenderer.sprite = isGrabbingHand;
        else if (_hovering != null)
            spriteRenderer.sprite = openHand;
        else
            spriteRenderer.sprite = pointingFinger;

        if (squeeze != null)
            squeeze.Trigger();
    }

    public void HideCursor()
    {
        spriteRenderer.enabled = false;
    }

    public void ShowCursor()
    {
        spriteRenderer.enabled = true;
    }

    private void FollowCursor()
    {
        Vector3 cursorPixelPosition = Input.mousePosition;

        cursorPixelPosition.x = Mathf.Clamp(cursorPixelPosition.x, 0, Screen.width);
        cursorPixelPosition.y = Mathf.Clamp(cursorPixelPosition.y, 0, Screen.height);

        Vector3 cursorScreenPosition = new Vector3(cursorPixelPosition.x, cursorPixelPosition.y, 10);

        Vector3 position = mainCamera.ScreenToWorldPoint(cursorScreenPosition);
        transform.position = position;
    }
}