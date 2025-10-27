
public class BasePage<T> : Page<T> where T : BasePage<T>
{
    public void Show()
    {
        Open();
    }

    public void Hide()
    {
        Close();
    }
}
