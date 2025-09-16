using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class LockerOpenedPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    public string SessionId { get; set; } = string.Empty;

    public LockerOpenedPage()
    {
        InitializeComponent();
    }

    private async void ContinueButton_Clicked(object sender, EventArgs e)
    {
        // Naviguer vers les instructions de verrouillage
        await Shell.Current.GoToAsync($"//LockInstructionsPage?sessionId={SessionId}");
    }
}
