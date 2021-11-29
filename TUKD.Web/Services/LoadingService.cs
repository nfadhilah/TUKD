namespace TUKD.Web.Services;

public class LoadingService : ILoadingService
{
    public Action? OnLoading { get; set; }
    public bool IsLoading { get; set; }

    public void SetLoading()
    {
        IsLoading = true;
        OnLoading?.Invoke();
    }

    public void StopLoading()
    {
        IsLoading = false;
        OnLoading?.Invoke();
    }
}