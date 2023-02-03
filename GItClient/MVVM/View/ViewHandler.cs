namespace GItClient.MVVM.View
{
    public class ViewHandler
    {
        public delegate void Event();
        public event Event? OnViewChange;
        public object? ViewModel;

        public void RaiseOnViewChange()
        {
            OnViewChange?.Invoke();
        }
    }
}
