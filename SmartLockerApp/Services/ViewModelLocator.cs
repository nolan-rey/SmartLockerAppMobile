using SmartLockerApp.Interfaces;
using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Services;

/// <summary>
/// Service locator pour les ViewModels - facilite l'injection de dépendances
/// Version locale sans API
/// </summary>
public class ViewModelLocator
{
    private static ViewModelLocator? _instance;
    private readonly IDataService _dataService;

    public static ViewModelLocator Instance => _instance ??= new ViewModelLocator();

    private ViewModelLocator()
    {
        // Utilise le service local
        _dataService = new LocalDataService();
    }

    // ViewModels
    public HomeViewModel HomeViewModel => new(_dataService);
    public LoginViewModel LoginViewModel => new(_dataService);
    public DepositSetupViewModel DepositSetupViewModel => new(_dataService);
    public ActiveSessionViewModel ActiveSessionViewModel => new(new LocalDataService());
    public PaymentViewModel PaymentViewModel => new(_dataService);
}
