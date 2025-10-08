using TMPro;
using UnityEngine;
using DG.Tweening;

public class ToastDisplay : MonoBehaviour
{
    static ToastDisplay instance;

    public static void Show(string text) => instance.InternalShow(text);

    [SerializeField] float tweenDuration = 0.35f;
    [SerializeField] float displayDuration = 3f;
    [Space]
    [SerializeField] RectTransform boxRect;
    [SerializeField] TextMeshProUGUI textComp;

    float shownPosY;
    float hiddenPosY;

    Sequence displaySequence;

    void Awake()
    {
        instance = this;

        shownPosY = boxRect.anchoredPosition.y;
        hiddenPosY = -boxRect.sizeDelta.y;

        boxRect.anchoredPosition = new(boxRect.anchoredPosition.x, hiddenPosY);
    }

    void InternalShow(string text)
    {
        if (displaySequence != null)
            displaySequence.Kill(false);

        float width = textComp.GetPreferredValues(text).x + 20f; // bouh magic number boooooh

        boxRect.sizeDelta = new(width, boxRect.sizeDelta.y);
        boxRect.anchoredPosition = new(boxRect.anchoredPosition.x, hiddenPosY);
        textComp.text = text;

        displaySequence = DOTween.Sequence()
                                 .Append(boxRect.DOAnchorPosY(shownPosY, tweenDuration).SetEase(Ease.OutCubic))
                                 .AppendInterval(displayDuration)
                                 .Append(boxRect.DOAnchorPosY(hiddenPosY, tweenDuration).SetEase(Ease.InCubic))
                                 .Play();
    }
}
