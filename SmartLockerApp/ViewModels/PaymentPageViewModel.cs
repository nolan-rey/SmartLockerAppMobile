using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Models;
using SmartLockerApp.Services;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page de paiement
/// </summary>
[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class PaymentPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    #region Observable Properties

    [ObservableProperty]
    private string sessionId = string.Empty;

    [ObservableProperty]
    private string action = string.Empty;

    [ObservableProperty]
    private string sessionLocker = "Casier";

    [ObservableProperty]
    private string sessionDuration = "N/A";

    [ObservableProperty]
    private string sessionStartTime = "N/A";

    [ObservableProperty]
    private string sessionEndTime = "N/A";

    [ObservableProperty]
    private string sessionTotal = "0,00 €";

    [ObservableProperty]
    private string payButtonText = "Payer";

    [ObservableProperty]
    private bool isPayButtonVisible = true;

    [ObservableProperty]
    private bool isCancelButtonVisible = true;

    [ObservableProperty]
    private bool isCreditCardSelected = true;

    [ObservableProperty]
    private bool isPayPalSelected;

    [ObservableProperty]
    private bool isApplePaySelected;

    [ObservableProperty]
    private string backButtonText = "Retour";

    private string selectedPaymentMethod = "CreditCard";

    #endregion

    public PaymentPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "Paiement";
    }

    #region Commands

    [RelayCommand]
    private async Task LoadData()
    {
        if (string.IsNullOrEmpty(SessionId))
        {
            await Shell.Current.DisplayAlert("Erreur", "Session introuvable", "OK");
            await Shell.Current.GoToAsync("//HomePage");
            return;
        }

        if (Action == "receipt")
        {
            await UpdateUIForReceipt();
        }
        else
        {
            Title = "Paiement";
            IsPayButtonVisible = true;
            IsCancelButtonVisible = true;

            var session = await _appState.GetSessionAsync(SessionId);
            if (session != null)
            {
                UpdateSessionDisplay(session);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
    }

    [RelayCommand]
    private void SelectCreditCard()
    {
        ResetPaymentSelections();
        IsCreditCardSelected = true;
        selectedPaymentMethod = "CreditCard";
    }

    [RelayCommand]
    private void SelectPayPal()
    {
        ResetPaymentSelections();
        IsPayPalSelected = true;
        selectedPaymentMethod = "PayPal";
    }

    [RelayCommand]
    private void SelectApplePay()
    {
        ResetPaymentSelections();
        IsApplePaySelected = true;
        selectedPaymentMethod = "ApplePay";
    }

    [RelayCommand]
    private async Task Pay()
    {
        if (IsBusy) return;

        IsBusy = true;

        try
        {
            await Shell.Current.DisplayAlert("Paiement", $"Paiement effectué avec {selectedPaymentMethod}", "OK");
            await Shell.Current.GoToAsync("//HomePage");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        bool result = await Shell.Current.DisplayAlert("Annuler", "Êtes-vous sûr de vouloir annuler le paiement?", "Oui", "Non");
        if (result)
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        if (Action == "receipt")
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    #endregion

    #region Private Methods

    private void ResetPaymentSelections()
    {
        IsCreditCardSelected = false;
        IsPayPalSelected = false;
        IsApplePaySelected = false;
    }

    private async Task UpdateUIForReceipt()
    {
        IsPayButtonVisible = false;
        IsCancelButtonVisible = false;
        Title = "Reçu de paiement";
        BackButtonText = "Retour à l'accueil";

        if (!string.IsNullOrEmpty(SessionId))
        {
            var session = await _appState.GetSessionAsync(SessionId);
            if (session != null)
            {
                UpdateSessionDisplay(session);
                await Shell.Current.DisplayAlert("Session terminée",
                    $"Votre session a été clôturée avec succès.\nCoût total: {session.TotalCost:C}",
                    "OK");
            }
        }
    }

    private void UpdateSessionDisplay(LockerSession session)
    {
        try
        {
            var displayId = CompatibilityService.IntToStringId(session.LockerId);
            SessionLocker = displayId;
            SessionDuration = $"{session.DurationHours}h";
            SessionStartTime = session.StartTime.ToString("HH:mm");
            SessionEndTime = session.EndTime.ToString("HH:mm");
            SessionTotal = $"{session.TotalCost:F2} €";
            PayButtonText = $"Payer {session.TotalCost:F2} €";
        }
        catch (Exception ex)
        {
            SessionLocker = "Casier";
            SessionDuration = "N/A";
            SessionStartTime = "N/A";
            SessionEndTime = "N/A";
            SessionTotal = "0,00 €";
            PayButtonText = "Payer";
            System.Diagnostics.Debug.WriteLine($"Erreur UpdateSessionDisplay: {ex.Message}");
        }
    }

    #endregion
}
