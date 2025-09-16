using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class DepositSetupPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private string selectedDuration = "1 heure";
    private string selectedPrice = "4,00€";
    private double selectedHours = 1.0;
    private string? lockerId;

    public DepositSetupPage()
    {
        InitializeComponent();
        
        // Set default selection (1 hour)
        UpdateSelection("1 heure", "4,00€", 1.0);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimationService.FadeInAsync(this);
        await AnimationService.SlideInFromBottomAsync(this.Content, 400);
        
        // Get locker ID from query parameters if available
        var uri = Shell.Current.CurrentState.Location.ToString();
        if (uri.Contains("lockerId="))
        {
            lockerId = uri.Split("lockerId=")[1].Split('&')[0];
        }
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnOption30MinTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        UpdateSelection("30 minutes", "2,50€", 0.5);
        UpdateRadioButtons("30min");
    }

    private async void OnOption1HourTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        UpdateSelection("1 heure", "4,00€", 1.0);
        UpdateRadioButtons("1hour");
    }

    private async void OnOption2HoursTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        UpdateSelection("2 heures", "7,00€", 2.0);
        UpdateRadioButtons("2hours");
    }

    private async void OnOption4HoursTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        UpdateSelection("4 heures", "12,00€", 4.0);
        UpdateRadioButtons("4hours");
    }

    private void UpdateSelection(string duration, string price, double hours)
    {
        selectedDuration = duration;
        selectedPrice = price;
        selectedHours = hours;
        SelectedDurationText.Text = duration;
        SelectedPriceText.Text = price;
    }

    private void UpdateRadioButtons(string selected)
    {
        // Reset all borders and radios
        Option30Min.Stroke = Colors.Gray;
        Option1Hour.Stroke = Colors.Gray;
        Option2Hours.Stroke = Colors.Gray;
        Option4Hours.Stroke = Colors.Gray;

        Radio30Min.BackgroundColor = Colors.Transparent;
        Radio1Hour.BackgroundColor = Colors.Transparent;
        Radio2Hours.BackgroundColor = Colors.Transparent;
        Radio4Hours.BackgroundColor = Colors.Transparent;

        // Set selected option
        switch (selected)
        {
            case "30min":
                Option30Min.Stroke = Color.FromArgb("#3B82F6");
                Radio30Min.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "1hour":
                Option1Hour.Stroke = Color.FromArgb("#3B82F6");
                Radio1Hour.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "2hours":
                Option2Hours.Stroke = Color.FromArgb("#3B82F6");
                Radio2Hours.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
            case "4hours":
                Option4Hours.Stroke = Color.FromArgb("#3B82F6");
                Radio4Hours.BackgroundColor = Color.FromArgb("#3B82F6");
                break;
        }
    }

    private string? GetSelectedDuration()
    {
        return selectedDuration;
    }

    private async Task OnConfirmClicked(object sender, EventArgs e)
    {
        var selectedDuration = GetSelectedDuration();
        if (selectedDuration == null) return;

        await AnimationService.ButtonPressAsync(ConfirmButton);
        
        ConfirmButton.Text = "Création en cours...";
        ConfirmButton.IsEnabled = false;
        
        try
        {
            // Mapper l'ID du casier de la page d'accueil vers l'ID du service
            var selectedLockerId = MapLockerIdToServiceId(lockerId);
            
            // Créer une liste d'items vide pour l'instant, sera remplie dans DepositItemsPage
            var result = await _appState.StartSessionWithItemsAsync(selectedLockerId, (int)selectedHours, new List<string>());
            
            if (result.Success && result.Session != null)
            {
                ConfirmButton.Text = "✓ Session créée";
                await AnimationService.SuccessCheckmarkAsync(ConfirmButton);
                
                // Naviguer vers la page d'ouverture du casier avec l'ID de session
                await Shell.Current.GoToAsync($"//OpenLockerPage?sessionId={result.Session.Id}");
            }
            else
            {
                await DisplayAlert("Erreur", result.Message ?? "Impossible de créer la session", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            ConfirmButton.Text = "Confirmer";
            ConfirmButton.IsEnabled = true;
        }
    }

    /// <summary>
    /// Mappe l'ID du casier de la page d'accueil vers l'ID utilisé par le service
    /// </summary>
    private string MapLockerIdToServiceId(string? homePageLockerId)
    {
        return homePageLockerId switch
        {
            "A1" => "L001",
            "B2" => "L002", 
            "C3" => "L003",
            _ => "L001" // Casier par défaut
        };
    }

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        await OnConfirmClicked(sender, e);
    }
}
