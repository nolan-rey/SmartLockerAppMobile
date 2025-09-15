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
        SetupMobileGestures();
    }
    
    private void SetupMobileGestures()
    {
        // Add pull-to-refresh for updating data
        GestureService.AddPullToRefresh(this.FindByName<ScrollView>("MainScrollView") ?? new ScrollView(), 
            async () => await RefreshData());
        
        // Add swipe gestures for navigation
        GestureService.AddSwipeGestures(this,
            onSwipeLeft: async () => await Shell.Current.GoToAsync("//HistoryPage"),
            onSwipeRight: async () => await Shell.Current.GoToAsync("//SettingsPage"));
        
        // Add tap with feedback to locker cards
        foreach (var child in LockersContainer.Children)
        {
            if (child is Border border)
            {
                GestureService.AddTapWithFeedback(border, async () => await OnLockerClicked(border, EventArgs.Empty));
            }
        }
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
        await AnimationService.FadeInAsync(this);
        
        // Animate cards in sequence
        await Task.Delay(100);
        await AnimationService.SlideInFromBottomAsync(WelcomeSection, 300);
        await Task.Delay(100);
        
        if (_appState.ActiveSession != null)
        {
            ActiveSessionCard.IsVisible = true;
        UpdateUI();
    }

    private void OnAppStateChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AppStateService.ActiveSession))
        {
            MainThread.BeginInvokeOnMainThread(UpdateUI);
        }
    }

    private void UpdateUI()
    {
        var currentUser = _appState.CurrentUser;
        if (currentUser != null)
        {
            WelcomeLabel.Text = $"Bonjour, {currentUser.FirstName}";
        }

        var activeSession = _appState.ActiveSession;
        if (activeSession != null)
        {
            ActiveSessionCard.IsVisible = true;
            SessionLockerLabel.Text = activeSession.LockerId;
            SessionLocationLabel.Text = "Entrée principale"; // TODO: Get from locker data
            
            var timeRemaining = activeSession.EndTime - DateTime.Now;
            if (timeRemaining.TotalMinutes > 0)
            {
                SessionTimeLabel.Text = $"Temps restant: {timeRemaining.Hours}h {timeRemaining.Minutes}min";
            }
            else
            {
                SessionTimeLabel.Text = "Session expirée";
            }
        }
        else
        {
            ActiveSessionCard.IsVisible = false;
        }

        // Update statistics
        var availableCount = _appState.AvailableLockers.Count(l => l.Status == LockerStatus.Available);
        AvailableLockersLabel.Text = availableCount.ToString();
        
        var sessionsCount = _appState.SessionHistory.Count;
        SessionsCountLabel.Text = sessionsCount.ToString();
        
        var totalSpent = _appState.SessionHistory.Sum(s => s.TotalCost);
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

    private async void OnLockerClicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("//LockerDetailPage");
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
