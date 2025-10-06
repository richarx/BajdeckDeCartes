using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

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
        
        public SortingData GetSortingPriority();
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
        {
            if (Mouse.current.leftButton.isPressed)
                Binder.Instance.Close();
            return;
        }


        var top = hitboxs.Select(collider => new { collider, interactable = collider.GetComponent<IInteractable>() }).Where(x => x.interactable != null)
            .Select(x => new { x.collider, x.interactable, sortingData = x.interactable.GetSortingPriority() })
            .Select(x => new { x.collider, x.interactable, x.sortingData.sortingOrder, layerValue = SortingLayer.GetLayerValueFromID(x.sortingData.sortingLayerId) })
            .OrderByDescending(x => x.layerValue)
            .ThenByDescending(x => x.sortingOrder)
            .ThenByDescending(x => x.collider.transform, new ComparerHierarchy())
            .FirstOrDefault();


        //int bestOrder = int.MinValue;
        //IInteractable hitBoxHit = null;
        //int index = -1;

        //for (int i = 0; i < hitboxs.Length; i++)
        //{
        //    var interact = hitboxs[i].GetComponentInParent<IInteractable>();
        //    if (interact == null) continue;

        //    // HERE
        //    int sortingOrder = interact.GetSortingPriority().sortingOrder;
        //    if (sortingOrder > bestOrder)
        //    {
        //        bestOrder = sortingOrder;
        //        hitBoxHit = interact;
        //        index = i;
        //    }
        //}
        
        if (top != null)
        {
            IInteractable hitBoxHit = top.interactable;

            //if (index >= 0)
            //Debug.Log("hitboxs " + hitboxs[index].name);

            if (hitBoxHit != null)
            {
                if (hitBoxHit != null && Mouse.current.leftButton.isPressed && isGrabbing == null)
                {
                    isGrabbing = hitBoxHit;
                    if (top != null)
                        _draggable = top.collider.GetComponent<Draggable>();
                    //if (index >= 0)
                    //    _draggable = hitboxs[index].GetComponent<Draggable>();
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
            else 
            {
                if (Mouse.current.leftButton.isPressed)
                    Binder.Instance.Close();
            }
        }
        else
        {
            if (Mouse.current.leftButton.isPressed)
                Binder.Instance.Close();
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
        Vector3 cursorScreenPosition = new Vector3(cursorPixelPosition.x, cursorPixelPosition.y, 10);

        transform.position = mainCamera.ScreenToWorldPoint(cursorScreenPosition);
    }
}