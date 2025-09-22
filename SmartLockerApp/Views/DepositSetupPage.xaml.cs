using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
public partial class DepositSetupPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private string selectedDuration = "1 heure";
    private string selectedPrice = "4,00€";
    private double selectedHours = 1.0;
    private string? lockerId;

    public string LockerId
    {
        get => lockerId ?? string.Empty;
        set => lockerId = value;
    }

    public DepositSetupPage()
    {
        InitializeComponent();
        
        // Set default selection (1 hour)
        selectedHours = 1.0;
        UpdateSelection("1 heure", CalculatePrice(1.0).ToString("C"), 1.0);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // Charger les détails du casier si l'ID est fourni
        if (!string.IsNullOrEmpty(lockerId))
        {
            LoadLockerDetails();
        }
        
        // Calculer et afficher les prix réels
        UpdatePricing();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void OnOption30MinTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        ResetAllSelections();
        SetSelectedStyle(Option30Min, Radio30Min);
        var price = CalculatePrice(0.5);
        UpdateSelection("30 minutes", price.ToString("C"), 0.5);
    }

    private void ResetAllSelections()
    {
        // Reset all border styles
        Option30Min.Stroke = Color.FromArgb("#E5E7EB");
        Option30Min.StrokeThickness = 1;
        Option1Hour.Stroke = Color.FromArgb("#E5E7EB");
        Option1Hour.StrokeThickness = 1;
        Option2Hours.Stroke = Color.FromArgb("#E5E7EB");
        Option2Hours.StrokeThickness = 1;
        Option4Hours.Stroke = Color.FromArgb("#E5E7EB");
        Option4Hours.StrokeThickness = 1;

        // Reset all radio buttons
        Radio30Min.BackgroundColor = Colors.Transparent;
        Radio1Hour.BackgroundColor = Colors.Transparent;
        Radio2Hours.BackgroundColor = Colors.Transparent;
        Radio4Hours.BackgroundColor = Colors.Transparent;
    }

    private void SetSelectedStyle(Border option, Border radio)
    {
        // Set selected border style
        option.Stroke = Color.FromArgb("#3B82F6");
        option.StrokeThickness = 2;
        
        // Set selected radio button
        radio.BackgroundColor = Color.FromArgb("#3B82F6");
    }

    private void LoadLockerDetails()
    {
        // Charger les détails du casier basé sur lockerId
        // Pour l'instant, on utilise les prix calculés dynamiquement
    }

    private async void OnOption1HourTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        ResetAllSelections();
        SetSelectedStyle(Option1Hour, Radio1Hour);
        var price = CalculatePrice(1.0);
        UpdateSelection("1 heure", price.ToString("C"), 1.0);
    }

    private async void OnOption2HoursTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        ResetAllSelections();
        SetSelectedStyle(Option2Hours, Radio2Hours);
        var price = CalculatePrice(2.0);
        UpdateSelection("2 heures", price.ToString("C"), 2.0);
    }

    private async void OnOption4HoursTapped(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        ResetAllSelections();
        SetSelectedStyle(Option4Hours, Radio4Hours);
        var price = CalculatePrice(4.0);
        UpdateSelection("4 heures", price.ToString("C"), 4.0);
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
                
                // Naviguer directement vers LockerOpenedPage après création de session
                await Shell.Current.GoToAsync($"//LockerOpenedPage?sessionId={result.Session.Id}");
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
            _ => "L001" // Casier par défaut
        };
    }

    private decimal CalculatePrice(double hours)
    {
        // Tarification basée sur les heures
        return hours switch
        {
            0.5 => 2.50m,  // 30 minutes
            1.0 => 4.00m,  // 1 heure
            2.0 => 7.00m,  // 2 heures
            4.0 => 12.00m, // 4 heures
            _ => 4.00m     // Par défaut 1 heure
        };
    }

    private void UpdatePricing()
    {
        // Mettre à jour tous les prix affichés
        Price30MinLabel.Text = CalculatePrice(0.5).ToString("C");
        Price1HourLabel.Text = CalculatePrice(1.0).ToString("C");
        Price2HoursLabel.Text = CalculatePrice(2.0).ToString("C");
        Price4HoursLabel.Text = CalculatePrice(4.0).ToString("C");
        
        // Mettre à jour le prix sélectionné
        var currentPrice = CalculatePrice(selectedHours);
        selectedPrice = currentPrice.ToString("C");
        SelectedPriceText.Text = selectedPrice;
    }

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        await OnConfirmClicked(sender, e);
    }
}
