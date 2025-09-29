using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Models;
using SmartLockerApp.Services;
using System.Collections.ObjectModel;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel de démonstration pour l'utilisation de l'API SmartLocker
/// Exemple simple d'intégration MVVM
/// </summary>
public partial class ApiDemoViewModel : ObservableObject
{
    private readonly SmartLockerIntegratedService _smartLockerService;

    [ObservableProperty]
    private string connectionStatus = "Non connecté";

    [ObservableProperty]
    private bool isConnected = false;

    [ObservableProperty]
    private bool isBusy = false;

    [ObservableProperty]
    private string testResults = string.Empty;

    public ObservableCollection<Locker> AvailableLockers { get; } = new();
    public ObservableCollection<LockerSession> ActiveSessions { get; } = new();

    public ApiDemoViewModel(SmartLockerIntegratedService smartLockerService)
    {
        _smartLockerService = smartLockerService;
    }

    /// <summary>
    /// Se connecte à l'API avec les credentials de test
    /// </summary>
    [RelayCommand]
    private async Task ConnectAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ConnectionStatus = "Connexion en cours...";

        try
        {
            var success = await _smartLockerService.LoginAsync();
            
            if (success)
            {
                IsConnected = true;
                ConnectionStatus = _smartLockerService.IsApiAvailable 
                    ? "✅ Connecté à l'API" 
                    : "✅ Connecté (mode local)";
                
                // Charge les données initiales
                await LoadDataAsync();
            }
            else
            {
                IsConnected = false;
                ConnectionStatus = "❌ Échec de connexion";
            }
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"❌ Erreur: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Se déconnecte
    /// </summary>
    [RelayCommand]
    private void Disconnect()
    {
        _smartLockerService.Logout();
        IsConnected = false;
        ConnectionStatus = "Déconnecté";
        
        AvailableLockers.Clear();
        ActiveSessions.Clear();
    }

    /// <summary>
    /// Charge les casiers disponibles
    /// </summary>
    [RelayCommand]
    private async Task LoadLockersAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var lockers = await _smartLockerService.GetAvailableLockersAsync();
            
            AvailableLockers.Clear();
            foreach (var locker in lockers)
            {
                AvailableLockers.Add(locker);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur LoadLockers: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Charge les sessions actives
    /// </summary>
    [RelayCommand]
    private async Task LoadSessionsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var sessions = await _smartLockerService.GetActiveSessionsAsync();
            
            ActiveSessions.Clear();
            foreach (var session in sessions)
            {
                ActiveSessions.Add(session);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur LoadSessions: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Démarre une session de test
    /// </summary>
    [RelayCommand]
    private async Task StartTestSessionAsync()
    {
        if (IsBusy || AvailableLockers.Count == 0) return;

        IsBusy = true;
        try
        {
            var firstLocker = AvailableLockers.First();
            var plannedEnd = DateTime.Now.AddHours(2);
            
            var session = await _smartLockerService.StartSessionAsync(1, firstLocker.Id, plannedEnd);
            
            if (session != null)
            {
                ActiveSessions.Add(session);
                await LoadLockersAsync(); // Recharge les casiers pour voir le changement de statut
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erreur StartTestSession: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Lance un test complet de l'API
    /// </summary>
    [RelayCommand]
    private async Task RunFullTestAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        TestResults = "Test en cours...";

        try
        {
            var results = await _smartLockerService.TestApiConnectionAsync();
            TestResults = results;
        }
        catch (Exception ex)
        {
            TestResults = $"Erreur pendant le test: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Charge les données initiales après connexion
    /// </summary>
    private async Task LoadDataAsync()
    {
        await LoadLockersAsync();
        await LoadSessionsAsync();
    }
}
