using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(LockerId), "lockerId")]
[QueryProperty(nameof(DurationHours), "durationHours")]
[QueryProperty(nameof(Price), "price")]
public partial class DepositItemsPage : ContentPage
{
    private readonly AppStateService _appState;
    
    public string LockerId { get; set; } = string.Empty;
    public string DurationHours { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;

    public DepositItemsPage(AppStateService appState)
    {
        InitializeComponent();
        _appState = appState;
        SetupCheckboxes();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine($"📦 Page Dépôt d'Objets:");
        System.Diagnostics.Debug.WriteLine($"   - Casier ID: {LockerId}");
        System.Diagnostics.Debug.WriteLine($"   - Durée: {DurationHours}h");
        System.Diagnostics.Debug.WriteLine($"   - Prix: {Price}€");
    }

    private void SetupCheckboxes()
    {
        // Add checkboxes to list for easier management
        var checkboxes = new[] { ItemsPlacedCheck, DoorClosedCheck, ConfirmDepositCheck };

        // Désélectionner toutes les checkboxes
        ItemsPlacedCheck.IsChecked = false;
        DoorClosedCheck.IsChecked = false;
        ConfirmDepositCheck.IsChecked = false;

        // Subscribe to checkbox events
        ItemsPlacedCheck.CheckedChanged += OnCheckboxChanged;
        DoorClosedCheck.CheckedChanged += OnCheckboxChanged;
        ConfirmDepositCheck.CheckedChanged += OnCheckboxChanged;
    }

    private void OnCheckboxChanged(object? sender, CheckedChangedEventArgs e)
    {
        UpdateConfirmButton();
    }

    private void UpdateConfirmButton()
    {
        var allChecked = ItemsPlacedCheck.IsChecked && DoorClosedCheck.IsChecked && ConfirmDepositCheck.IsChecked;
        ConfirmDepositButton.IsEnabled = allChecked;
        ConfirmDepositButton.BackgroundColor = allChecked ? Color.FromArgb("#10B981") : Color.FromArgb("#94A3B8");
    }

    private async void ConfirmDepositButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);

        ConfirmDepositButton.Text = "Traitement...";
        ConfirmDepositButton.IsEnabled = false;

        try
        {
            System.Diagnostics.Debug.WriteLine("✅ Dépôt confirmé, navigation vers LockInstructionsPage");
            
            ConfirmDepositButton.Text = "✓ Dépôt confirmé";
            await AnimationService.SuccessCheckmarkAsync(ConfirmDepositButton);
            
            // ✅ CORRECTION: Naviguer vers LockInstructionsPage avec tous les paramètres
            await Shell.Current.GoToAsync($"//LockInstructionsPage?lockerId={LockerId}&durationHours={DurationHours}&price={Price}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            ConfirmDepositButton.Text = "Continuer";
            ConfirmDepositButton.IsEnabled = true;
        }
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("..");
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await AnimationService.ButtonPressAsync((VisualElement)sender);
        await Shell.Current.GoToAsync("..");
    }
}
