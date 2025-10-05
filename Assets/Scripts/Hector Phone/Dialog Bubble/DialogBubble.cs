using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class DialogBubble : MonoBehaviour
{
    [SerializeField] float appearAndDisappearDuration = 0.35f;
    [SerializeField] float timePerLetter = 0.1f;
    [SerializeField] float timePerWhiteSpace = 0.1f;
    [SerializeField] float pauseAtTheEnd = 1f;
    [Space]
    [SerializeField] Vector2 extraSizeInBubble = new(20f, 20f);
    [Space]
    [SerializeField] TextMeshProUGUI textGUI;
    [SerializeField] Transform pivotTransform;
    [SerializeField] RectTransform bubbleRectTransform;

    void Awake()
    {
        pivotTransform.localScale = Vector3.zero;
    }

    public void Display(string text, Action onFinished)
    {
        StopAllCoroutines();

        pivotTransform.localScale = Vector3.zero;
        textGUI.text = string.Empty;

        bubbleRectTransform.sizeDelta = GetBestSizeForAspectRatio(text) + extraSizeInBubble;

        StartCoroutine(DisplayRoutine(text, onFinished));
    }

    public void Hide()
    {
        StopAllCoroutines();
        pivotTransform.DOScale(0, appearAndDisappearDuration);
    }

    Vector2 GetBestSizeForAspectRatio(string text)
    {
        return textGUI.GetPreferredValues(text);
    }

    IEnumerator DisplayRoutine(string text, Action onFinished)
    {
        pivotTransform.DOScale(1, appearAndDisappearDuration);
        yield return new WaitForSeconds(appearAndDisappearDuration);

        foreach (char c in text)
        {
            textGUI.text += c;
            yield return new WaitForSeconds(char.IsWhiteSpace(c) ? timePerWhiteSpace : timePerLetter);
        }

        yield return new WaitForSeconds(pauseAtTheEnd);

        pivotTransform.DOScale(0, appearAndDisappearDuration);
        yield return new WaitForSeconds(appearAndDisappearDuration);

        onFinished?.Invoke();
    }
}
