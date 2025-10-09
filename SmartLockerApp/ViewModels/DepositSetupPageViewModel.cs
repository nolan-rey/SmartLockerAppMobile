using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Services;
using SmartLockerApp.Interfaces;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page de configuration de dépôt
/// </summary>
[QueryProperty(nameof(LockerId), "lockerId")]
public partial class DepositSetupPageViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    #region Observable Properties

    [ObservableProperty]
    private string lockerId = string.Empty;

    [ObservableProperty]
    private string selectedDuration = "1 heure";

    [ObservableProperty]
    private string selectedPrice = "4,00 €";

    [ObservableProperty]
    private double selectedHours = 1.0;

    [ObservableProperty]
    private bool isOption30MinSelected;

    [ObservableProperty]
    private bool isOption1HourSelected = true;

    [ObservableProperty]
    private bool isOption2HoursSelected;

    [ObservableProperty]
    private bool isOption4HoursSelected;

    [ObservableProperty]
    private string price30Min = "2,50 €";

    [ObservableProperty]
    private string price1Hour = "4,00 €";

    [ObservableProperty]
    private string price2Hours = "7,00 €";

    [ObservableProperty]
    private string price4Hours = "12,00 €";

    [ObservableProperty]
    private string confirmButtonText = "Confirmer";

    [ObservableProperty]
    private bool isConfirmButtonEnabled = true;

    #endregion

    public DepositSetupPageViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Configuration";
        
        // Initialiser avec 1 heure par défaut
        UpdatePricing();
    }

    #region Commands

    [RelayCommand]
    private async Task LoadData()
    {
        System.Diagnostics.Debug.WriteLine("🔄 Chargement DepositSetupPage...");
        
        // Vérifier que l'utilisateur est connecté
        var currentUser = await _dataService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            System.Diagnostics.Debug.WriteLine("❌ Utilisateur non connecté sur DepositSetupPage");
            await Shell.Current.DisplayAlert("Erreur", "Vous devez être connecté pour continuer", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
            return;
        }
        
        System.Diagnostics.Debug.WriteLine($"✅ Utilisateur connecté: {currentUser.name} (ID: {currentUser.id})");
        
        if (!string.IsNullOrEmpty(LockerId))
        {
            System.Diagnostics.Debug.WriteLine($"✅ Casier sélectionné: {LockerId}");
            LoadLockerDetails();
        }
        UpdatePricing();
    }

    [RelayCommand]
    private void Select30Min()
    {
        ResetSelections();
        IsOption30MinSelected = true;
        UpdateSelection("30 minutes", CalculatePrice(0.5), 0.5);
    }

    [RelayCommand]
    private void Select1Hour()
    {
        ResetSelections();
        IsOption1HourSelected = true;
        UpdateSelection("1 heure", CalculatePrice(1.0), 1.0);
    }

    [RelayCommand]
    private void Select2Hours()
    {
        ResetSelections();
        IsOption2HoursSelected = true;
        UpdateSelection("2 heures", CalculatePrice(2.0), 2.0);
    }

    [RelayCommand]
    private void Select4Hours()
    {
        ResetSelections();
        IsOption4HoursSelected = true;
        UpdateSelection("4 heures", CalculatePrice(4.0), 4.0);
    }

    [RelayCommand]
    private async Task Confirm()
    {
        if (IsBusy) return;

        IsBusy = true;
        ConfirmButtonText = "Vérification...";
        IsConfirmButtonEnabled = false;

        try
        {
            System.Diagnostics.Debug.WriteLine("✅ Confirmation de la durée de dépôt");
            
            // Vérifier que l'utilisateur est toujours connecté
            var currentUser = await _dataService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Utilisateur non connecté");
                await Shell.Current.DisplayAlert("Erreur", "Vous devez être connecté pour continuer", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return;
            }
            
            System.Diagnostics.Debug.WriteLine($"✅ Utilisateur: {currentUser.name} (ID: {currentUser.id})");
            System.Diagnostics.Debug.WriteLine($"✅ Casier: {LockerId}");
            System.Diagnostics.Debug.WriteLine($"✅ Durée: {SelectedHours} heure(s)");
            System.Diagnostics.Debug.WriteLine($"✅ Prix: {SelectedPrice}");
            
            ConfirmButtonText = "✓ Confirmé";
            
            // Naviguer vers OpenLockerPage avec les paramètres (PAS de création de session ici)
            await Shell.Current.GoToAsync($"//OpenLockerPage?lockerId={LockerId}&durationHours={SelectedHours}&price={SelectedPrice.Replace(" €", "")}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"❌ Erreur: {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", $"Une erreur s'est produite : {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            ConfirmButtonText = "Confirmer";
            IsConfirmButtonEnabled = true;
        }
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    #endregion

    #region Private Methods

    private void ResetSelections()
    {
        IsOption30MinSelected = false;
        IsOption1HourSelected = false;
        IsOption2HoursSelected = false;
        IsOption4HoursSelected = false;
    }

    private void UpdateSelection(string duration, decimal price, double hours)
    {
        SelectedDuration = duration;
        SelectedPrice = $"{price:F2} €";
        SelectedHours = hours;
    }

    private void LoadLockerDetails()
    {
        // Charger les détails du casier si nécessaire
        // Pour l'instant, on utilise les prix calculés dynamiquement
    }

    private decimal CalculatePrice(double hours)
    {
        return hours switch
        {
            0.5 => 2.50m,
            1.0 => 4.00m,
            2.0 => 7.00m,
            4.0 => 12.00m,
            _ => 4.00m
        };
    }

    private void UpdatePricing()
    {
        Price30Min = $"{CalculatePrice(0.5):F2} €";
        Price1Hour = $"{CalculatePrice(1.0):F2} €";
        Price2Hours = $"{CalculatePrice(2.0):F2} €";
        Price4Hours = $"{CalculatePrice(4.0):F2} €";
    }

    private string MapLockerIdToServiceId(string? homePageLockerId)
    {
        return homePageLockerId switch
        {
            "A1" => "L001",
            "B2" => "L002",
            _ => "L001"
        };
    }

    #endregion
}
