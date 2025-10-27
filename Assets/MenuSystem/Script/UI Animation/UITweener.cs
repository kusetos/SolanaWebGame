using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UITweener : MonoBehaviour
{
    [Serializable]
    public struct UITweenerData
    {
        public string TweenIdentifier;
        public TweenSettingData TweenSetting;
    }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Tweener tweener;

    [SerializeField] private bool canPlayOnEnable;
    [SerializeField] private List<UITweenerData> tweenList = new List<UITweenerData>();

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (!canPlayOnEnable) return;
        PlayTween("OnEnable");
    }

    public void AddTweenData(UITweenerData tweenData)
    {
        tweenList.Add(tweenData);
    }

    public void InsertTweenData(int index, UITweenerData tweenData)
    {
        tweenList.Insert(index, tweenData);
    }

    public bool HasTweenIdentifier(string identifier)
    {
        foreach (var tween in tweenList)
        {
            if (tween.TweenIdentifier == identifier)
                return true;
        }
        return false;
    }

    public void PlayTween(string identifier)
    {
        foreach (var tween in tweenList)
        {
            if (tween.TweenIdentifier != identifier) continue;

            PlayTween(tween.TweenSetting);
            return;
        }
        Debug.Log("Tween Identifier Not Found");
    }

    public void KillAllRunningTween()
    {
        DOTween.KillAll();
    }

    public void PlayTween(TweenSettingData tweenData)
    {
        switch (tweenData.TweenType)
        {
            case TweenTypes.None: Debug.Log("No Types used for this this tween"); break;
            case TweenTypes.Slide: Slide(tweenData); break;
            case TweenTypes.Scale:
            case TweenTypes.ScaleX:
            case TweenTypes.ScaleY: Scale(tweenData); break;
            case TweenTypes.Fade: Fade(tweenData); break;
        }
    }

    private void Scale(TweenSettingData tweenData)
    {
        tweenData.OnTweenStart?.Invoke();
        ResetCanvasGroupAlpha();
        Vector2 from = tweenData.UsingCustomValue ? tweenData.From : Vector3.one;
        Vector2 to = tweenData.UsingCustomValue ? tweenData.To : Vector3.zero;
        switch (tweenData.TweenType)
        {
            case TweenTypes.Scale:
                tweener = rectTransform.DOScale(to, tweenData.Duration).From(from); break;
            case TweenTypes.ScaleX:
                tweener = rectTransform.DOScaleX(to.x, tweenData.Duration).From(from); break;
            case TweenTypes.ScaleY:
                tweener = rectTransform.DOScaleY(to.y, tweenData.Duration).From(from); break;
        }

        if (tweenData.TweenMode == TweenModeType.IN && !tweenData.UsingCustomValue)
        {
            tweener.From();
        }

        tweener.SetDelay(tweenData.StartDelay)
            .SetEase(tweenData.TweenEase)
            .OnComplete(OnComplete);

        void OnComplete()
        {
            tweenData.OnTweenComplete?.Invoke();
            rectTransform.localScale = tweenData.UsingCustomValue ? rectTransform.localScale : Vector3.one;
        }
    }

    private void Slide(TweenSettingData tweenData)
    {
        tweenData.OnTweenStart?.Invoke();
        ResetCanvasGroupAlpha();
        Vector2 targetPos = Vector2.zero;
        switch (tweenData.TweenDirection)
        {
            case TweenDirectionType.UP:
                targetPos = tweenData.TweenMode == TweenModeType.IN ? new Vector2(0, -rectTransform.rect.height) : new Vector2(0, rectTransform.rect.height);
                break;
            case TweenDirectionType.RIGHT:
                targetPos = tweenData.TweenMode == TweenModeType.IN ? new Vector2(-rectTransform.rect.width, 0) : new Vector2(rectTransform.rect.width, 0);
                break;
            case TweenDirectionType.DOWN:
                targetPos = tweenData.TweenMode == TweenModeType.IN ? new Vector2(0, rectTransform.rect.height) : new Vector2(0, -rectTransform.rect.height);
                break;
            case TweenDirectionType.LEFT:
                targetPos = tweenData.TweenMode == TweenModeType.IN ? new Vector2(rectTransform.rect.width, 0) : new Vector2(-rectTransform.rect.width, 0);
                break;
        }
        Vector2 from = tweenData.UsingCustomValue ? tweenData.From : Vector2.zero;
        Vector2 to = tweenData.UsingCustomValue ? tweenData.To : targetPos;

        tweener = rectTransform.DOAnchorPos(to, tweenData.Duration)
            .From(from)
            .SetDelay(tweenData.StartDelay)
            .SetEase(tweenData.TweenEase)
            .OnComplete(OnComplete);

        if (tweenData.TweenMode == TweenModeType.IN && !tweenData.UsingCustomValue)
        {
            tweener.From();
        }

        void OnComplete()
        {
            tweenData.OnTweenComplete?.Invoke();
            rectTransform.anchoredPosition = tweenData.UsingCustomValue ? rectTransform.anchoredPosition : Vector3.zero;
        }
    }

    private void Fade(TweenSettingData tweenData)
    {
        CanvasGroupValidation();
        tweenData.OnTweenStart?.Invoke();
        float from = tweenData.UsingCustomValue ? tweenData.From.x : 1.0f;
        float to = tweenData.UsingCustomValue ? tweenData.To.x : 0.0f;
        tweener = canvasGroup.DOFade(to, tweenData.Duration)
            .From(from)
            .SetDelay(tweenData.StartDelay)
            .SetEase(tweenData.TweenEase)
            .OnComplete(OnComplete);

        if (tweenData.TweenMode == TweenModeType.IN && !tweenData.UsingCustomValue)
        {
            tweener.From();
        }
        tweener.Play();

        void OnComplete()
        {
            tweenData.OnTweenComplete?.Invoke();
            canvasGroup.alpha = tweenData.UsingCustomValue ? canvasGroup.alpha : 1.0f;
        }
    }

    private void CanvasGroupValidation()
    {
        if (canvasGroup != null) return;
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void ResetCanvasGroupAlpha()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = canvasGroup.alpha == 0 ? 1 : canvasGroup.alpha;
    }
}
