using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
public partial class LockConfirmationPage : ContentPage
{
    private readonly AppStateService _appState;
    public string SessionId { get; set; } = string.Empty;

    public LockConfirmationPage(AppStateService appState)
    {
        InitializeComponent();
        _appState = appState;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Marquer la session comme verrouillée
        if (!string.IsNullOrEmpty(SessionId))
        {
            await _appState.LockLockerAsync(SessionId);
        }
    }

    private async void HomeButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }
}
