using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

public partial class DepositItemsPage : ContentPage, IQueryAttributable
{
    private readonly List<CheckBox> _checkboxes = new();
    private readonly AppStateService _appState;
    private string? _sessionId;

    public DepositItemsPage(AppStateService appState)
    {
        InitializeComponent();
        _appState = appState;
        SetupCheckboxes();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("sessionId"))
        {
            _sessionId = query["sessionId"].ToString();
        }
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
            var selectedItems = new List<string> { "Objets personnels" };

            // Mettre à jour la session avec les items sélectionnés
            if (!string.IsNullOrEmpty(_sessionId))
            {
                await _appState.UpdateSessionItemsAsync(_sessionId, selectedItems);
                
                ConfirmDepositButton.Text = "✓ Dépôt confirmé";
                await AnimationService.SuccessCheckmarkAsync(ConfirmDepositButton);
                
                // Naviguer vers UnlockInstructionsPage
                await Shell.Current.GoToAsync($"//UnlockInstructionsPage?sessionId={_sessionId}");
            }
            else
            {
                await DisplayAlert("Erreur", "Session non trouvée", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            ConfirmDepositButton.Text = "Confirmer le dépôt";
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
