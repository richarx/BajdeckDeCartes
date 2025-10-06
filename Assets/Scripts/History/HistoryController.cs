using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HistoryController : MonoBehaviour
{
    [SerializeField] float tweenDuration = 0.35f;
    [SerializeField] float backgroundOpacity = 0.7f;
    [Space]
    [SerializeField] Button openButton;
    [SerializeField] Button closeButton;
    [SerializeField] Image background;
    [SerializeField] RectTransform panel;
    [SerializeField] ScrollRect scroll;

    float panelPositionX;

    void Awake()
    {
        panelPositionX = -panel.rect.width;

        panel.anchoredPosition = new(panel.anchoredPosition.x, 0f);

        background.color = new(background.color.r, background.color.g, background.color.b, 0f);
        background.raycastTarget = false;

        openButton.gameObject.SetActive(false);
        History.OnNewLogWithCatchUp += RevealOpenButton;
    }

    void OnEnable()
    {
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);
    }

    void OnDisable()
    {
        openButton.onClick.RemoveListener(OpenPanel);
        closeButton.onClick.RemoveListener(ClosePanel);
    }

    void RevealOpenButton(LogType _source, string _text)
    {
        if (openButton.isActiveAndEnabled)
            return;

        History.OnNewLogWithCatchUp -= RevealOpenButton;

        openButton.gameObject.SetActive(true);
        openButton.image.DOFade(1f, tweenDuration);
    }

    void OpenPanel()
    {
        scroll.verticalNormalizedPosition = 0f;

        background.DOFade(backgroundOpacity, tweenDuration);
        panel.DOAnchorPosX(panelPositionX, tweenDuration);

        background.raycastTarget = true;
    }

    void ClosePanel()
    {
        background.DOFade(0f, tweenDuration);
        panel.DOAnchorPosX(0f, tweenDuration);

        background.raycastTarget = false;
    }
}
