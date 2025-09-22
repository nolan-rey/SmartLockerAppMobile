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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadSessionData();
    }

    private async Task LoadSessionData()
    {
        if (!string.IsNullOrEmpty(SessionId))
        {
            var session = await _appState.GetSessionAsync(SessionId);
            if (session != null)
            {
                // Afficher l'ID du casier réel
                var locker = _appState.GetLockerDetails(session.LockerId);
                if (locker != null)
                {
                    // Mapper l'ID du service vers l'ID d'affichage
                    var displayId = MapServiceIdToDisplayId(session.LockerId);
                    LockerIdLabel.Text = displayId;
                }

                // Afficher la durée sélectionnée
                DurationLabel.Text = $"{session.DurationHours}h";
            }
        }
    }

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2",
            _ => serviceId
        };
    }

    private async void ContinueButton_Clicked(object sender, EventArgs e)
    {
        // Naviguer vers les instructions de verrouillage
        await Shell.Current.GoToAsync($"//LockInstructionsPage?sessionId={SessionId}");
    }
}
