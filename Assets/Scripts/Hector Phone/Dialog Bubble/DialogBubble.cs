using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
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
    [SerializeField] Image mouseIcon;
    [SerializeField] RectTransform bubbleRectTransform;

    void Awake()
    {
        pivotTransform.localScale = Vector3.zero;
    }

    public void Display(IReadOnlyCollection<string> text, Action onFinished)
    {
        StopAllCoroutines();

        pivotTransform.localScale = Vector3.zero;

        //bubbleRectTransform.sizeDelta = GetBestSizeForAspectRatio(text) + extraSizeInBubble;

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

    IEnumerator DisplayRoutine(IReadOnlyCollection<string> text, Action onFinished)
    {
        textGUI.text = "";
        mouseIcon.gameObject.SetActive(false);
        
        pivotTransform.DOScale(1, appearAndDisappearDuration);
        yield return new WaitForSeconds(appearAndDisappearDuration);

        foreach (string dialog in text)
        {
            textGUI.text = dialog;
            mouseIcon.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            mouseIcon.gameObject.SetActive(true);
            yield return WaitForInput();
        }

        pivotTransform.DOScale(0, appearAndDisappearDuration);
        yield return new WaitForSeconds(appearAndDisappearDuration);

        onFinished?.Invoke();
    }

    IEnumerator WaitForInput()
    {
        bool waitInput = true;
        while (waitInput)
        {
            if (Input.GetMouseButtonDown(0))
                waitInput = false;
            yield return null;
        }
    }
}
