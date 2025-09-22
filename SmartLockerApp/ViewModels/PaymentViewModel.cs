using SmartLockerApp.Interfaces;
using SmartLockerApp.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SmartLockerApp.ViewModels;

public class PaymentViewModel : BaseViewModel
{
    private readonly IDataService _dataService;
    private string _sessionId = string.Empty;
    private string _action = string.Empty;
    private LockerSession? _session;
    private PaymentMethod? _selectedPaymentMethod;
    private bool _isReceiptMode;

    public PaymentViewModel(IDataService dataService)
    {
        _dataService = dataService;
        Title = "Paiement";

        PaymentMethods = new ObservableCollection<PaymentMethod>
        {
            new("CreditCard", "Carte de crédit", "💳"),
            new("PayPal", "PayPal", "🅿️"),
            new("ApplePay", "Apple Pay", "📱")
        };

        SelectedPaymentMethod = PaymentMethods.First();

        // Commands
        BackCommand = new AsyncRelayCommand(GoBackAsync);
        PayCommand = new AsyncRelayCommand(ProcessPaymentAsync, CanPay);
        CancelCommand = new AsyncRelayCommand(CancelPaymentAsync);
        SelectPaymentMethodCommand = new RelayCommand<PaymentMethod>(SelectPaymentMethod);
    }

    // Properties
    public ObservableCollection<PaymentMethod> PaymentMethods { get; }

    public string SessionId
    {
        get => _sessionId;
        set => SetProperty(ref _sessionId, value);
    }

    public string Action
    {
        get => _action;
        set
        {
            if (SetProperty(ref _action, value))
            {
                IsReceiptMode = value == "receipt";
            }
        }
    }

    public LockerSession? Session
    {
        get => _session;
        set
        {
            if (SetProperty(ref _session, value))
            {
                UpdateSessionDisplay();
            }
        }
    }

    public PaymentMethod? SelectedPaymentMethod
    {
        get => _selectedPaymentMethod;
        set
        {
            if (SetProperty(ref _selectedPaymentMethod, value))
            {
                UpdatePaymentMethodSelection();
                ((AsyncRelayCommand)PayCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsReceiptMode
    {
        get => _isReceiptMode;
        set
        {
            if (SetProperty(ref _isReceiptMode, value))
            {
                Title = value ? "Reçu de paiement" : "Paiement";
                OnPropertyChanged(nameof(ShowPaymentOptions));
                OnPropertyChanged(nameof(PayButtonText));
            }
        }
    }

    public bool ShowPaymentOptions => !IsReceiptMode;

    // Session Display Properties
    public string LockerDisplayId => Session != null ? MapServiceIdToDisplayId(Session.LockerId) : string.Empty;
    public string SessionDuration => Session != null ? FormatDuration(Session.DurationHours) : string.Empty;
    public string SessionStartTime => Session?.StartTime.ToString("HH:mm") ?? string.Empty;
    public string SessionEndTime => Session?.EndTime.ToString("HH:mm") ?? string.Empty;
    public string SessionTotal => Session?.TotalCost.ToString("C") ?? string.Empty;
    public string PayButtonText => Session != null ? $"Payer {Session.TotalCost:C}" : "Payer";

    // Commands
    public ICommand BackCommand { get; }
    public ICommand PayCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand SelectPaymentMethodCommand { get; }

    // Methods
    public async Task InitializeAsync(string sessionId, string action = "")
    {
        SessionId = sessionId;
        Action = action;

        await ExecuteAsync(async () =>
        {
            if (!string.IsNullOrEmpty(SessionId))
            {
                Session = await _dataService.GetSessionAsync(SessionId);
            }

            if (IsReceiptMode && Session != null)
            {
                await ShowReceiptConfirmationAsync();
            }
        });
    }

    private void UpdateSessionDisplay()
    {
        OnPropertyChanged(nameof(LockerDisplayId));
        OnPropertyChanged(nameof(SessionDuration));
        OnPropertyChanged(nameof(SessionStartTime));
        OnPropertyChanged(nameof(SessionEndTime));
        OnPropertyChanged(nameof(SessionTotal));
        OnPropertyChanged(nameof(PayButtonText));
    }

    private void UpdatePaymentMethodSelection()
    {
        foreach (var method in PaymentMethods)
        {
            method.IsSelected = method == SelectedPaymentMethod;
        }
    }

    private void SelectPaymentMethod(PaymentMethod? method)
    {
        if (method != null)
        {
            SelectedPaymentMethod = method;
        }
    }

    private bool CanPay()
    {
        return SelectedPaymentMethod != null && Session != null && !IsBusy && !IsReceiptMode;
    }

    private async Task ProcessPaymentAsync()
    {
        if (Session == null || SelectedPaymentMethod == null)
            return;

        // Simulate payment processing
        if (Application.Current?.Windows?.Count > 0)
        {
            var mainPage = Application.Current.Windows[0].Page;
            if (mainPage != null)
                await mainPage.DisplayAlert(
                    "Paiement", 
                    $"Paiement de {Session.TotalCost:C} effectué avec {SelectedPaymentMethod.DisplayName}", 
                    "OK");
        }

        await Shell.Current.GoToAsync("//HomePage");
    }

    private async Task CancelPaymentAsync()
    {
        bool result = false;
        if (Application.Current?.Windows?.Count > 0)
        {
            var mainPage = Application.Current.Windows[0].Page;
            if (mainPage != null)
                result = await mainPage.DisplayAlert(
                    "Annuler", 
                    "Êtes-vous sûr de vouloir annuler le paiement?", 
                    "Oui", 
                    "Non");
        }

        if (result == true)
        {
            await GoBackAsync();
        }
    }

    private async Task GoBackAsync()
    {
        if (IsReceiptMode)
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
    }

    private async Task ShowReceiptConfirmationAsync()
    {
        if (Session != null)
        {
            if (Application.Current?.Windows?.Count > 0)
            {
                var mainPage = Application.Current.Windows[0].Page;
                if (mainPage != null)
                    await mainPage.DisplayAlert(
                        "Session terminée", 
                        $"Votre session a été clôturée avec succès.\nCoût total: {Session.TotalCost:C}", 
                        "OK");
            }
        }
    }

    private string MapServiceIdToDisplayId(string serviceId)
    {
        return serviceId switch
        {
            "L001" => "A1",
            "L002" => "B2",
            _ => serviceId
        };
    }

    private string FormatDuration(double hours)
    {
        if (hours < 1)
        {
            return $"{(int)(hours * 60)} minutes";
        }
        return $"{hours:F0} heure{(hours > 1 ? "s" : "")}";
    }
}

public class PaymentMethod : BaseViewModel
{
    private bool _isSelected;

    public PaymentMethod(string id, string displayName, string icon)
    {
        Id = id;
        DisplayName = displayName;
        Icon = icon;
    }

    public string Id { get; }
    public string DisplayName { get; }
    public string Icon { get; }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public string BorderColor => IsSelected ? "#2563EB" : "#E2E8F0";
    public string BackgroundColor => IsSelected ? "#F8FAFC" : "White";
}
