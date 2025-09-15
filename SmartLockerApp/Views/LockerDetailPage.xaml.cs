using SmartLockerApp.Services;
using SmartLockerApp.Models;

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
        await AnimationService.SlideInFromBottomAsync(this.Content, 400);
        
        LoadLockerData();
    }

    private void LoadLockerData()
    {
        // Get first available locker for demo
        var locker = _appState.Lockers.FirstOrDefault();
        
        if (locker != null)
        {
            _selectedLocker = locker;
            // Demo data since UI elements don't exist in XAML
            Title = $"Casier {_selectedLocker.Id}";
            
            UpdateStatusDisplay();
        }
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
