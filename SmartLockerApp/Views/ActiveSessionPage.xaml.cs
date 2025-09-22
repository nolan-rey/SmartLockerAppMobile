using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class ActiveSessionPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;

    public ActiveSessionPage()
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
        if (_appState.ActiveSession != null)
        {
            var session = _appState.ActiveSession;
            
            // Mapper l'ID du casier pour l'affichage
            var displayLockerId = MapServiceIdToDisplayId(session.LockerId);
            LockerIdLabel.Text = $"Casier {displayLockerId}";
            
            // Afficher les heures de début et fin
            StartTimeLabel.Text = session.StartTime.ToString("HH:mm");
            EndTimeLabel.Text = session.EndTime.ToString("HH:mm");
            
            // Afficher la durée
            var duration = session.DurationHours;
            if (duration < 1)
            {
                DurationLabel.Text = $"{(int)(duration * 60)} minutes";
            }
            else
            {
                DurationLabel.Text = $"{duration:F0} heure{(duration > 1 ? "s" : "")}";
            }
            
            // Calculer et afficher le temps restant
            var remaining = session.EndTime - DateTime.Now;
            if (remaining.TotalMinutes > 0)
            {
                if (remaining.TotalHours >= 1)
                {
                    RemainingTimeLabel.Text = $"{(int)remaining.TotalHours}h {remaining.Minutes}min";
                }
                else
                {
                    RemainingTimeLabel.Text = $"{(int)remaining.TotalMinutes}min";
                }
                RemainingTimeLabel.TextColor = Color.FromArgb("#F59E0B");
            }
            else
            {
                RemainingTimeLabel.Text = "Expiré";
                RemainingTimeLabel.TextColor = Color.FromArgb("#EF4444");
            }
        }
    }

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2", 
            _ => "A1"
        };
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
