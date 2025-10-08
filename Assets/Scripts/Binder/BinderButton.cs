using MoreMountains.Feedbacks;
using UnityEngine;

public class BinderButton : MonoBehaviour, GrabCursor.IInteractable, IDragInteractable
{
    [SerializeField] private Binder _binder;
    [SerializeField] private MMF_Player _shakeSequencer;
    [SerializeField] private MMF_Player _cleanSequencer;

    private bool _hovering = false;

    private void Awake()
    {
        Draggable.OnDragEnd += OnEndDrag;
    }

    public void EndInteract()
    {
    }

    public void Interact()
    {
        if (_binder.IsOpened)
            _binder.Close();
        else
            _binder.Open();
    }

    private void Update()
    {
        if (GrabCursor.instance.Draggable != null)
        {
            CardInstance cardInstance = GrabCursor.instance.Draggable.GetComponent<CardInstance>();

            if (cardInstance != null && _binder.GetSlotCardFitInBinder(cardInstance) != null)
            {
                _shakeSequencer.PlayFeedbacks();
                return;
            }
        }
        if (_hovering && GrabCursor.instance.Draggable == null)
        {
            if (!_shakeSequencer.IsPlaying)
                _shakeSequencer.PlayFeedbacks();
        }
        else
            Clean();
    }

    private void Clean()
    {
        if (_shakeSequencer.IsPlaying)
        {
            _shakeSequencer.StopFeedbacks();
            _cleanSequencer.PlayFeedbacks();
        }
    }


    private void OnEndDrag(Draggable draggable)
    {
        //Clean();
    }

    public void Hover()
    {
        _hovering = true;
    }

    public void EndHover()
    {
        _hovering = false;
    }

    public void UseDraggable(Draggable drag)
    {
        _shakeSequencer.StopFeedbacks();
        _cleanSequencer.PlayFeedbacks();
        _binder.UseDraggable(drag);
    }

    public bool CanUse(Draggable drag)
    {
        return (drag.GetComponent<CardInstance>() != null);
    }

    public SortingData GetSortingOrder()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }

    public SortingData GetSortingPriority()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        return new SortingData(spriteRenderer.sortingOrder, spriteRenderer.sortingLayerID);
    }


    public bool CanInteract()
    {
        return (true);
    }

    public void DragHover(Draggable drag)
    {
    }

}
