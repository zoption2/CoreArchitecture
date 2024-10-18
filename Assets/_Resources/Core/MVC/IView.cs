using Action = System.Action;


namespace MVC
{
    public interface IView
    {
        void Show(Action onShow = null);
        void Hide(Action onHide = null);
        void Release();
    }
}

