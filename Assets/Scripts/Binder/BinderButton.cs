using UnityEngine;
using MoreMountains.Feedbacks;

public class BinderButton : MonoBehaviour, GrabCursor.IInteractable, IDragInteractable
{
    [SerializeField] private Binder _binder;
    [SerializeField] private MMF_Player _shakeSequencer;
    [SerializeField] private MMF_Player _cleanSequencer;

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
        if (_shakeSequencer.IsPlaying)
        {
            if (!GrabCursor.instance.IsHovering && GrabCursor.instance.Draggable == null)
            {
                Clean();
            }
        }
        else
        {
            if (GrabCursor.instance.Draggable != null)
            {
                CardInstance cardInstance = GrabCursor.instance.Draggable.GetComponent<CardInstance>();

                if (cardInstance != null && _binder.DoesCardFitInBinder(cardInstance))
                {
                    _shakeSequencer.PlayFeedbacks();
                }
            }
        }
    }

    private void Clean()
    {

        _shakeSequencer.StopFeedbacks();
        _cleanSequencer.PlayFeedbacks();
    }


    private void OnEndDrag(Draggable draggable)
    {
        //Clean();
    }

    public void Hover()
    {
        if (!_shakeSequencer.IsPlaying && GrabCursor.instance.Draggable == null)
        {
            _shakeSequencer.PlayFeedbacks();
        }
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


    public bool CanHover()
    {
        return (true);
    }

}
