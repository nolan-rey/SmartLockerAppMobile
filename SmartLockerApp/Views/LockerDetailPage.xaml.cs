using SmartLockerApp.Services;
using SmartLockerApp.Models;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
public partial class LockerDetailPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private Locker? _selectedLocker;
    
    public string LockerId { get; set; } = string.Empty;

    public LockerDetailPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await AnimationService.FadeInAsync(this);
        await AnimationService.SlideInFromBottomAsync(this.Content, 400);
        
        LoadLockerData();
    }

    private void LoadLockerData()
    {
        // Utiliser le LockerId passé en paramètre ou le premier casier disponible
        if (!string.IsNullOrEmpty(LockerId))
        {
            // Créer un casier de démonstration basé sur l'ID
            _selectedLocker = CreateDemoLocker(LockerId);
        }
        else
        {
            // Fallback vers le premier casier disponible
            _selectedLocker = _appState.Lockers.FirstOrDefault() ?? CreateDemoLocker("A1");
        }
        
        if (_selectedLocker != null)
        {
            Title = $"Casier {_selectedLocker.Id}";
            UpdateStatusDisplay();
        }
    }

    private Locker CreateDemoLocker(string lockerId)
    {
        var locations = new Dictionary<string, string>
        {
            { "A1", "Entrée principale" },
            { "B2", "Hall principal" },
            { "C3", "Zone sécurisée" }
        };

        var status = lockerId == "C3" ? LockerStatus.Occupied : LockerStatus.Available;

        return new Locker
        {
            Id = lockerId,
            Location = locations.GetValueOrDefault(lockerId, "Emplacement inconnu"),
            Size = LockerSize.Medium,
            Status = status,
            PricePerHour = 2.50m,
            Features = new List<string> { "Sécurisé", "Climatisé", "Accès 24/7" }
        };
    }

    private void UpdateStatusDisplay()
    {
        // Simplified since UI elements don't exist in current XAML
        if (_selectedLocker == null) return;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("..");
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        OnBackClicked(sender, e);
    }

    private async void OnUseLockerClicked(object sender, EventArgs e)
    {
        if (_selectedLocker?.Status != LockerStatus.Available) return;

        await AnimationService.ButtonPressAsync((VisualElement)sender);
        
        // Pass locker ID to next page
        await Shell.Current.GoToAsync($"//DepositSetupPage?lockerId={_selectedLocker.Id}");
    }

    private async void OnRemoteOpenClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        
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
    }

    private void UseLockerButton_Clicked(object sender, EventArgs e)
    {
        OnUseLockerClicked(sender, e);
    }

    private void OpenRemotelyButton_Clicked(object sender, EventArgs e)
    {
        OnRemoteOpenClicked(sender, e);
    }
}
