using SmartLockerApp.Interfaces;
using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Services;

/// <summary>
/// Service locator pour les ViewModels - facilite l'injection de dépendances
/// Version locale sans API
/// </summary>
public class ViewModelLocator
{
    private readonly IDataService _dataService;
    private readonly LockerManagementService _lockerService;

    public ViewModelLocator(IDataService dataService, LockerManagementService lockerService)
    {
        _dataService = dataService;
        _lockerService = lockerService;
    }

    // ViewModels
    public HomeViewModel HomeViewModel => new(_dataService, _lockerService);
    public LoginViewModel LoginViewModel => new(_dataService);
    public DepositSetupViewModel DepositSetupViewModel => new(_dataService);
    public ActiveSessionViewModel ActiveSessionViewModel => new(_dataService);
    public PaymentViewModel PaymentViewModel => new(_dataService);
}
