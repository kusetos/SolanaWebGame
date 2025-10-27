using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    public static PageManager Instance;

    [SerializeField] private List<Page> pageList = new List<Page>();
    [SerializeField] private Page defaultActivePage;
    [SerializeField] private float pageInactiveTime = 15f;

    private Stack<Page> pageStack = new Stack<Page>();
    private List<Page> activePageList = new List<Page>();

    public float PageInactiveTime { get { return pageInactiveTime; } }

    private void Awake()
    {
        Instance = this;
        defaultActivePage.Invoke("Open", 0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pageStack.Count > 0)
        {
            pageStack.Peek().OnBackPressed();
        }
    }

    public T CreatePageInstance<T>() where T : Page
    {
        foreach (var page in pageList)
        {
            if (page is T)
            {
                Page pageInstance = Instantiate(page, this.transform);
                activePageList.Add(pageInstance);
                return pageInstance as T;
            }
        }

        throw new MissingReferenceException("Prefab not found for type " + typeof(T));
    }

    public T GetPage<T>(bool forceCreatePageInstance = false) where T : Page
    {
        foreach (var page in activePageList)
        {
            if (page is T)
            {
                return page as T;
            }
        }

        if (forceCreatePageInstance)
        {
            return CreatePageInstance<T>() as T;
        }

        Debug.LogWarning("Page not found for type " + typeof(T));
        return null;
    }

    public bool TryGetPage<T>(out T page) where T : Page
    {
        foreach (var activePage in activePageList)
        {
            if (activePage is T)
            {
                page = activePage as T;
                return true;
            }
        }
        page = null;
        return false;
    }

    public void OpenPage(Page pageToOpen)
    {
        if (pageToOpen == null)
        {
            Debug.LogErrorFormat(pageToOpen, "{0} cannot be Open because the page reference is null", pageToOpen.name);
            return;
        }

        //Deactive Current active Page
        if (pageStack.Count > 0)
        {
            if (!pageToOpen.IsOverlay)
            {
                foreach (var page in pageStack)
                {
                    page.AnimatePage(false);
                    if (!page.IsOverlay)
                        break;
                }
            }
        }

        pageToOpen.AnimatePage(true);
        pageStack.Push(pageToOpen);
    }

    public void ClosePage(Page pageToClose)
    {
        if (pageStack.Count == 0)
        {
            Debug.LogErrorFormat(pageToClose, "{0} cannot be closed because page stack is empty", pageToClose.GetType());
            return;
        }

        if (pageStack.Peek() != pageToClose)
        {
            Debug.LogErrorFormat(pageToClose, "{0} cannot be closed because it is not on top of stack", pageToClose.GetType());
            return;
        }

        CloseTopPage();
    }

    public void CloseAllPage()
    {
        foreach (var page in pageStack)
        {
            page.AnimatePage(false);
        }
        pageStack.Clear();
    }

    public void RemovePageFromActiveList(Page pageToRemove)
    {
        activePageList.Remove(pageToRemove);
    }

    private void CloseTopPage()
    {
        Page pageToClose = pageStack.Pop();
        if (pageToClose.IsDestroyWhenClosed)
        {
            pageToClose.SetPageActivity(false);
        }

        pageToClose.AnimatePage(false);
        if (pageToClose.IsOverlay)
            return;
        // Re-activate top menu
        // If a re-activated menu is an overlay we need to activate the menu under it
        foreach (Page page in pageStack)
        {
            page.AnimatePage(true);
            if (!page.IsOverlay)
                break;
        }
    }

    [ContextMenu("Populate Page List")]
    private void PopulatePageList()
    {
        pageList.Clear();
        Page[] pages = Resources.LoadAll<Page>("Pages/");
        foreach (var page in pages)
        {
            pageList.Add(page);
        }
    }
}
