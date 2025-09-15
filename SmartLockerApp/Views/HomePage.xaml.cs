namespace SmartLockerApp.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        
        // Simulate active session for demo
        ShowActiveSession();
    }

    private void ShowActiveSession()
    {
        // For demo purposes, show the active session card
        ActiveSessionCard.IsVisible = true;
    }

    private async void SettingsButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Navigate to settings page
        await DisplayAlert("Paramètres", "Page de paramètres à implémenter", "OK");
    }

    private async void ViewActiveSessionButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Navigate to session details
        await DisplayAlert("Session active", "Détails de la session à implémenter", "OK");
    }
}
