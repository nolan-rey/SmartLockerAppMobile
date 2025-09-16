using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class ActiveSessionPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;

    public ActiveSessionPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OpenLockerButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//UnlockInstructionsPage");
    }

    private async void EndSessionButton_Clicked(object sender, EventArgs e)
    {
        if (_appState.ActiveSession != null)
        {
            // Naviguer vers les instructions de déverrouillage pour clôturer la session
            await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={_appState.ActiveSession.Id}&action=close");
        }
    }
}
