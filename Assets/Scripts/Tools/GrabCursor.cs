using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrabCursor : MonoBehaviour
{
    public static GrabCursor instance;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite isGrabbingHand;
    [SerializeField] private Sprite emptyHand;

    private Camera mainCamera;
    private SqueezeAndStretch squeeze;

    private bool isGrabbing;
    public bool IsGrabbing => isGrabbing;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;

        Cursor.visible = false;

        squeeze = GetComponent<SqueezeAndStretch>();
    }

    private void Update()
    {
        FollowCursor();

        if (isGrabbing != Mouse.current.leftButton.isPressed)
        {
            isGrabbing = Mouse.current.leftButton.isPressed;
            UpdateGraphicsState();
        }
    }

    private void UpdateGraphicsState()
    {
        if (isGrabbing)
            spriteRenderer.sprite = isGrabbingHand;
        else
            spriteRenderer.sprite = emptyHand;

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