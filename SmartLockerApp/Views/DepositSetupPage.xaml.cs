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

    private async void ConfirmButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(ConfirmButton);
        
        // Show loading state
        ConfirmButton.Text = "Création de la session...";
        ConfirmButton.IsEnabled = false;
        
        try
        {
            // Create new session
            var lockerId = this.lockerId ?? "A1"; // Default if no locker specified
            var success = await _appState.StartSessionAsync(lockerId, (int)selectedHours);
            
            if (success)
            {
                ConfirmButton.Text = "✓ Session créée";
                await AnimationService.SuccessCheckmarkAsync(ConfirmButton);
                await Shell.Current.GoToAsync("//UnlockInstructionsPage");
            }
            else
            {
                try
                {
                    // Handle error
                    await DisplayAlert("Erreur", "Impossible de créer la session", "OK");
                }
                catch (Exception)
                {
                    await DisplayAlert("Erreur", "Impossible de créer la session. Veuillez réessayer.", "OK");
                }
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Erreur", "Une erreur s'est produite lors de la création de la session.", "OK");
        }
        finally
        {
            ConfirmButton.Text = "Confirmer";
            ConfirmButton.IsEnabled = true;
        }
    }
}
