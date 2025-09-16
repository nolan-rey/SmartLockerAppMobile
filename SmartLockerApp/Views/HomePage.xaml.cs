using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class HomePage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;

    public HomePage()
    {
        InitializeComponent();
        _appState.PropertyChanged += OnAppStateChanged;
        
        // Add mobile gestures
        SetupGestures();
    }
    
    private void SetupGestures()
    {
        // Add swipe gestures for navigation
        GestureService.AddSwipeGestures(WelcomeSection,
            onSwipeLeft: async () => await Shell.Current.GoToAsync("//HistoryPage"),
            onSwipeRight: async () => await Shell.Current.GoToAsync("//SettingsPage"));
    }
    
    private async Task RefreshData()
    {
        // Simulate data refresh
        await Task.Delay(1000);
        UpdateUI();
        
        // Show success feedback
        await DisplayAlert("Actualisé", "Les données ont été mises à jour", "OK");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _appState.PropertyChanged += OnAppStateChanged;
        
        await AnimationService.FadeInAsync(this);
        
        // Animate cards in sequence
        await Task.Delay(100);
        await AnimationService.SlideInFromBottomAsync(WelcomeSection, 300);
        await Task.Delay(100);
        
        // Mettre à jour l'interface utilisateur
        UpdateUI();
        
        if (_appState.ActiveSession != null)
        {
            ActiveSessionCard.IsVisible = true;
        }
        else
        {
            ActiveSessionCard.IsVisible = false;
        }
    }

    private void OnAppStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppStateService.ActiveSession))
        {
            MainThread.BeginInvokeOnMainThread(() => {
                UpdateUI();
                // Mettre à jour la visibilité de la carte de session active
                if (_appState.ActiveSession != null)
                {
                    ActiveSessionCard.IsVisible = true;
                }
                else
                {
                    ActiveSessionCard.IsVisible = false;
                }
            });
        }
    }

    private void UpdateUI()
    {
        if (_appState.CurrentUser == null) return;

        // Update welcome message
        WelcomeLabel.Text = $"Bonjour, {_appState.CurrentUser.FirstName} !";

        // Update active session info
        if (_appState.ActiveSession != null)
        {
            ViewActiveSessionButton.IsVisible = true;
            SessionLockerLabel.Text = $"Casier {_appState.ActiveSession.LockerId}";
            SessionLocationLabel.Text = _appState.GetLockerDetails(_appState.ActiveSession.LockerId)?.Location ?? "Inconnu";
            
            // Afficher le temps restant en temps réel
            var remainingTime = _appState.GetRemainingTime();
            if (remainingTime > TimeSpan.Zero)
            {
                if (remainingTime.TotalHours >= 1)
                    SessionTimeLabel.Text = $"Expire dans {remainingTime.Hours}h {remainingTime.Minutes}min";
                else
                    SessionTimeLabel.Text = $"Expire dans {remainingTime.Minutes}min";
            }
            else
            {
                SessionTimeLabel.Text = "Session expirée";
            }
        }
        else
        {
            ViewActiveSessionButton.IsVisible = false;
        }

        // Update statistics avec vraies données
        var (totalSessions, totalSpent, totalTime) = _appState.GetUserStats();
        // Update session count if label exists
        try 
        { 
            var sessionCountLabel = this.FindByName<Label>("SessionCountLabel");
            if (sessionCountLabel != null)
                sessionCountLabel.Text = totalSessions.ToString();
        } 
        catch { }
        TotalSpentLabel.Text = $"€{totalSpent:F2}";
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("//SettingsPage");
    }

    private async void OnActiveSessionClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(ViewActiveSessionButton);
        await Shell.Current.GoToAsync("//ActiveSessionPage");
    }

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("//HistoryPage");
    }

    private async void OnLockerA1Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(LockerA1);
        await Shell.Current.GoToAsync($"//LockerDetailPage?lockerId=A1");
    }

    private async void OnLockerB2Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync(LockerB2);
        await Shell.Current.GoToAsync($"//LockerDetailPage?lockerId=B2");
    }

    private async void OnStartSessionClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("//DepositSetupPage");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _appState.PropertyChanged -= OnAppStateChanged;
    }
}
