namespace SmartLockerApp.Views;

public partial class LockerDetailPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private Locker? _selectedLocker;

    public LockerDetailPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimationService.FadeInAsync(this);
        await AnimationService.SlideInFromBottomAsync(LockerCard, 400);
        
        LoadLockerData();
    }

    private void LoadLockerData()
    {
        // Get first available locker for demo
        _selectedLocker = _appState.AvailableLockers.FirstOrDefault(l => l.Status == LockerStatus.Available);
        
        if (_selectedLocker != null)
        {
            LockerIdLabel.Text = _selectedLocker.Id;
            LockerSizeLabel.Text = _selectedLocker.Size;
            LockerLocationLabel.Text = "Entrée principale";
            LockerPriceLabel.Text = $"{_selectedLocker.PricePerHour:F2}€/h";
            
            UpdateStatusDisplay();
        }
    }

    private void UpdateStatusDisplay()
    {
        if (_selectedLocker == null) return;

        switch (_selectedLocker.Status)
        {
            case LockerStatus.Available:
                LockerStatusLabel.Text = "Disponible";
                StatusBadge.BackgroundColor = Color.FromArgb("#10B981");
                UseLockerButton.IsEnabled = true;
                UseLockerButton.Text = "Utiliser ce casier";
                break;
            case LockerStatus.Occupied:
                LockerStatusLabel.Text = "Occupé";
                StatusBadge.BackgroundColor = Color.FromArgb("#EF4444");
                UseLockerButton.IsEnabled = false;
                UseLockerButton.Text = "Non disponible";
                break;
            case LockerStatus.Maintenance:
                LockerStatusLabel.Text = "Maintenance";
                StatusBadge.BackgroundColor = Color.FromArgb("#F59E0B");
                UseLockerButton.IsEnabled = false;
                UseLockerButton.Text = "En maintenance";
                break;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnUseLockerClicked(object sender, EventArgs e)
    {
        if (_selectedLocker?.Status != LockerStatus.Available) return;

        await AnimationService.ButtonPressAsync(UseLockerButton);
        
        // Pass locker ID to next page
        await Shell.Current.GoToAsync($"//DepositSetupPage?lockerId={_selectedLocker.Id}");
    }

    private async void OnRemoteOpenClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(RemoteOpenButton);
        
        // Show loading state
        RemoteOpenButton.Text = "Ouverture...";
        RemoteOpenButton.IsEnabled = false;
        
        try
        {
            await Task.Delay(2000); // Simulate API call
            await DisplayAlert("Succès", $"Le casier {_selectedLocker?.Id} a été ouvert à distance", "OK");
            await Shell.Current.GoToAsync("//OpenLockerPage");
        }
        catch
        {
            await DisplayAlert("Erreur", "Impossible d'ouvrir le casier à distance", "OK");
        }
        finally
        {
            RemoteOpenButton.Text = "Ouvrir à distance";
            RemoteOpenButton.IsEnabled = true;
        }
    }
}
