using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UITweener)), DisallowMultipleComponent]
public class PageTweener : BasePageAnimation
{
    [Tooltip("Disable interactable when animation played or running")]
    [SerializeField] private bool isDisableInteractable;
    [SerializeField] private TweenSettingData entryTweenData;
    [SerializeField] private TweenSettingData exitTweenData;

    private UITweener tweenUI;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        tweenUI = GetComponent<UITweener>();
    }

    private void OnValidate()
    {
        if (entryTweenData != null)
            entryTweenData.TweenMode = TweenModeType.IN;
        if (exitTweenData != null)
            exitTweenData.TweenMode = TweenModeType.OUT;
    }

    public override void PlayEntryAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        entryTweenData.OnTweenStart.AddListener(() =>
        {
            DisableInteractable();
            onAnimationStart?.Invoke();
        });
        entryTweenData.OnTweenComplete.AddListener(() =>
        {
            EnableInteractable();
            onAnimationEnd?.Invoke();
        });

        tweenUI.PlayTween(entryTweenData);
    }

    public override void PlayExitAnimation(UnityAction onAnimationStart, UnityAction onAnimationEnd)
    {
        exitTweenData.OnTweenStart.AddListener(() =>
        {
            DisableInteractable();
            onAnimationStart?.Invoke();
        });
        exitTweenData.OnTweenComplete.AddListener(() =>
        {
            EnableInteractable();
            onAnimationEnd?.Invoke();
        });

        tweenUI.PlayTween(exitTweenData);
    }

    private void DisableInteractable()
    {
        if (!isDisableInteractable) return;
        if (!TryGetComponent<CanvasGroup>(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void EnableInteractable()
    {
        if (!isDisableInteractable) return;
        if (!TryGetComponent<CanvasGroup>(out canvasGroup))
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
