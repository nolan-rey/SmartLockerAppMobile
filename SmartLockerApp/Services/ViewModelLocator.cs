using SmartLockerApp.Interfaces;
using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Services;

/// <summary>
/// Service locator pour les ViewModels - facilite l'injection de dépendances
/// </summary>
public class ViewModelLocator
{
    private static ViewModelLocator? _instance;
    private readonly IDataService _dataService;

    public static ViewModelLocator Instance => _instance ??= new ViewModelLocator();

    private ViewModelLocator()
    {
        // Pour l'instant, utilise le service local
        // Plus tard, on pourra basculer vers ApiDataService
        _dataService = new LocalDataService();
    }

    // ViewModels
    public HomeViewModel HomeViewModel => new(_dataService);
    public LoginViewModel LoginViewModel => new(_dataService);
    public DepositSetupViewModel DepositSetupViewModel => new(_dataService);
    public ActiveSessionViewModel ActiveSessionViewModel => new(_dataService);
    public PaymentViewModel PaymentViewModel => new(_dataService);

    // Méthode pour basculer vers l'API quand elle sera prête
    public static void ConfigureForApi(IApiService apiService)
    {
        // _instance = new ViewModelLocator(new ApiDataService(apiService, new LocalDataService()));
    }
}
