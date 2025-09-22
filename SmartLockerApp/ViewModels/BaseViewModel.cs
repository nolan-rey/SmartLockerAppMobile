using CommunityToolkit.Mvvm.ComponentModel;

namespace SmartLockerApp.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title = string.Empty;

    protected virtual async Task ExecuteAsync(Func<Task> operation)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            await operation();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    protected virtual async Task HandleErrorAsync(Exception ex)
    {
        // Log error and show user-friendly message
        if (Application.Current?.Windows?.Count > 0)
        {
            var mainPage = Application.Current.Windows[0].Page;
            if (mainPage != null)
                await mainPage.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }
}
