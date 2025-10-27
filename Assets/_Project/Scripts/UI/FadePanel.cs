
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
[RequireComponent(typeof(Image))]
public class FadePanel : MonoBehaviour
{

    public static FadePanel Instance { get; private set; }

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    public Ease fadeEase = Ease.Linear;

    private Image image;
    private Tween fadeTween;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        image = GetComponent<Image>();
        image.enabled = true;

    }

    public void FadeIn(System.Action onComplete = null)
    {
        // Kill any running tween
        fadeTween?.Kill();

        // Ensure image is visible
        Color color = image.color;
        color.a = 0f;
        image.color = color;

        // Animate alpha to 1
        fadeTween = image.DOFade(1f, fadeDuration)
            .SetEase(fadeEase)
            .OnComplete(() => {
                StartCoroutine(WaitTimeAfter(onComplete));
    });

    }

    public void FadeOut(System.Action onComplete = null)
    {
        fadeTween?.Kill();
        Color currentColor = image.color;
        currentColor.a = 1f; // Alpha value between 0 (transparent) and 1 (opaque)
        image.color = currentColor;
        // Animate alpha to 0
        fadeTween = image.DOFade(0f, fadeDuration)
            .SetEase(fadeEase)
            .OnComplete(() =>
            {
                StartCoroutine(WaitTimeAfter(onComplete));
            });
    }
    IEnumerator WaitTimeAfter(System.Action onComplete = null){
        yield return new WaitForSeconds(0.3f);
        onComplete?.Invoke();

    }
    public void SetInstantAlpha(float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}