using System.Collections;
using UnityEngine;

public abstract class Page : MonoBehaviour
{
    [Tooltip("Destroy the Game Object when page is closed (reduces memory usage)")]
    public bool IsDestroyWhenClosed = true;

    [Tooltip("Disable page that are under this one in the stack")]
    public bool IsOverlay = false;

    public bool IsUsingAnimation;
    protected static bool isAnimationRunning;

    [SerializeField] protected bool IsWaitingPreviousPageAnimationComplete;
    [SerializeField] protected BasePageAnimation pageAnimation;
    [SerializeField] protected GameObject pageContent;

    protected abstract void Open();
    protected abstract void Close();
    public abstract void OnBackPressed();
    public abstract void SetPageActivity(bool isActive);
    public abstract void AnimatePage(bool isActive);
}

public abstract class Page<T> : Page where T : Page<T>
{
    protected override void Open()
    {
        if (PageManager.Instance.TryGetPage<T>(out T page))
        {
            page.SetPageActivity(true);
        }
        else
        {
            page = PageManager.Instance.CreatePageInstance<T>();
        }

        page.pageContent.SetActive(false);
        PageManager.Instance.OpenPage(page);
    }

    protected override void Close()
    {
        PageManager.Instance.ClosePage(this);
    }

    public override void OnBackPressed()
    {
        if (isAnimationRunning) return;
        Close();
    }

    public override void SetPageActivity(bool isActive)
    {
        if (isActive)
            StopCoroutine(nameof(DestroyOnInactive));
        else
            StartCoroutine(nameof(DestroyOnInactive));
    }

    public override void AnimatePage(bool isShowPage)
    {
        if (!IsUsingAnimation || pageAnimation == null)
        {
            pageContent.SetActive(isShowPage);
            return;
        }
        StartCoroutine(PlayPageAnimation(isShowPage));
    }

    private IEnumerator PlayPageAnimation(bool isShowPage)
    {
        while (isAnimationRunning && IsWaitingPreviousPageAnimationComplete)
        {
            yield return null;
        }

        if (isShowPage)
        {
            pageAnimation.PlayEntryAnimation(OnAnimationStart, OnEntryAnimationEnd);
            yield break;
        }
        pageAnimation.PlayExitAnimation(OnAnimationStart, OnExitAnimationEnd);

        void OnAnimationStart()
        {
            pageContent.SetActive(true);
            isAnimationRunning = true;
        }

        void OnEntryAnimationEnd()
        {
            isAnimationRunning = false;
        }

        void OnExitAnimationEnd()
        {
            isAnimationRunning = false;
            pageContent.SetActive(false);
        }
    }

    private IEnumerator DestroyOnInactive()
    {
        yield return new WaitForSeconds(PageManager.Instance.PageInactiveTime);
        PageManager.Instance.RemovePageFromActiveList(this);
        Destroy(gameObject);
    }
}
