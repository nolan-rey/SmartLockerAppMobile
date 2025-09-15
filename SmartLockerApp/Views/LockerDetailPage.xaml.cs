namespace SmartLockerApp.Views;

public partial class LockerDetailPage : ContentPage
{
    public LockerDetailPage()
    {
        InitializeComponent();
        
        // Initialize with demo data
        LoadLockerDetails();
    }

    private void LoadLockerDetails()
    {
        // Demo data - in real app, this would come from parameters or service
        LockerNameText.Text = "Casier A1";
        LockerStatusText.Text = "Disponible";
        LockerLocationText.Text = "Entrée principale";
        
        // Show appropriate button based on status
        UseLockerButton.IsVisible = true;
        OpenRemotelyButton.IsVisible = false;
        MaintenanceButton.IsVisible = false;
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void UseLockerButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//DepositSetupPage");
    }

    private async void OpenRemotelyButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement remote opening logic
        await DisplayAlert("Ouverture", "Casier ouvert à distance", "OK");
    }
}
