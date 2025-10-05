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

    private Draggable _draggable;
    public Draggable Draggable => _draggable;

    private bool isHovering;

    public interface IInteractable
    {
        void Interact();
        void EndInteract();
        public bool CanHover();

        public void Hover();
        
        public int GetSortingPriority();
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
            _draggable = null;
            isGrabbing = null;
            UpdateGraphicsState();
        }
    }

    private void FixedUpdate()
    {
        
        Collider2D[] hitboxs = Physics2D.OverlapPointAll(fingerPosition.position, ~0);

        if (hitboxs.Length == 0)
            return;


        int bestOrder = int.MinValue;
        IInteractable hitBoxHit = null;
        int index = -1;

        for (int i = 0; i < hitboxs.Length; i++)
        {
            var interact = hitboxs[i].GetComponentInParent<IInteractable>();
            if (interact == null) continue;
            

            int sortingOrder = interact.GetSortingPriority();
            if (sortingOrder > bestOrder)
            {
                bestOrder = sortingOrder;
                hitBoxHit = interact;
                index = i;
            }
        }



        //if (index >= 0)
        //Debug.Log("hitboxs " + hitboxs[index].name);

        if (hitBoxHit != null)
        {
            if (hitBoxHit != null && Mouse.current.leftButton.isPressed && isGrabbing == null)
            {
                isGrabbing = hitBoxHit;
                if (index >= 0)
                _draggable = hitboxs[index].GetComponent<Draggable>();
                isGrabbing.Interact();
                UpdateGraphicsState();
                return;
            }
            else if (hitBoxHit != null)
            {
                if (isHovering == false && hitBoxHit.CanHover())
                {
                    hitBoxHit.Hover();
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