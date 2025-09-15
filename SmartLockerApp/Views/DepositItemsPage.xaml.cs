namespace SmartLockerApp.Views;

public partial class DepositItemsPage : ContentPage
{
    public DepositItemsPage()
    {
        InitializeComponent();
        
        // Subscribe to checkbox events
        ItemsPlacedCheck.CheckedChanged += OnCheckboxChanged;
        DoorClosedCheck.CheckedChanged += OnCheckboxChanged;
        ConfirmDepositCheck.CheckedChanged += OnCheckboxChanged;
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnCheckboxChanged(object sender, CheckedChangedEventArgs e)
    {
        // Enable confirm button only if all checkboxes are checked
        ConfirmDepositButton.IsEnabled = ItemsPlacedCheck.IsChecked && 
                                        DoorClosedCheck.IsChecked && 
                                        ConfirmDepositCheck.IsChecked;
        
        // Update button color based on state
        if (ConfirmDepositButton.IsEnabled)
        {
            ConfirmDepositButton.BackgroundColor = Color.FromArgb("#10B981");
        }
        else
        {
            ConfirmDepositButton.BackgroundColor = Color.FromArgb("#94A3B8");
        }
    }

    private async void ConfirmDepositButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement deposit confirmation logic
        await DisplayAlert("Dépôt confirmé", "Vos objets ont été déposés avec succès!", "OK");
        await Shell.Current.GoToAsync("//ActiveSessionPage");
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Annuler", "Êtes-vous sûr de vouloir annuler le dépôt?", "Oui", "Non");
        if (result)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
