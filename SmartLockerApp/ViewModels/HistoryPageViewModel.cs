using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartLockerApp.Models;
using SmartLockerApp.Services;
using System.Collections.ObjectModel;

namespace SmartLockerApp.ViewModels;

/// <summary>
/// ViewModel pour la page d'historique des sessions
/// </summary>
public partial class HistoryPageViewModel : BaseViewModel
{
    private readonly AppStateService _appState;

    #region Observable Properties

    [ObservableProperty]
    private ObservableCollection<LockerSession> sessions = new();

    [ObservableProperty]
    private ObservableCollection<LockerSession> filteredSessions = new();

    [ObservableProperty]
    private bool isAllFilterActive = true;

    [ObservableProperty]
    private bool isCompletedFilterActive;

    [ObservableProperty]
    private bool isActiveFilterActive;

    [ObservableProperty]
    private string currentFilter = "All";

    #endregion

    public HistoryPageViewModel(AppStateService appState)
    {
        _appState = appState;
        Title = "Historique";
    }

    #region Commands

    [RelayCommand]
    private async Task LoadData()
    {
        await LoadHistoryData();
    }

    [RelayCommand]
    private void FilterAll()
    {
        ResetFilters();
        IsAllFilterActive = true;
        CurrentFilter = "All";
        FilterHistory("Tout");
    }

    [RelayCommand]
    private void FilterCompleted()
    {
        ResetFilters();
        IsCompletedFilterActive = true;
        CurrentFilter = "Completed";
        FilterHistory("Terminées");
    }

    [RelayCommand]
    private void FilterActive()
    {
        ResetFilters();
        IsActiveFilterActive = true;
        CurrentFilter = "Active";
        FilterHistory("Actives");
    }

    [RelayCommand]
    private async Task LoadMore()
    {
        await Shell.Current.DisplayAlert("Chargement", "Chargement de plus de sessions...", "OK");
    }

    [RelayCommand]
    private async Task NavigateBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    #endregion

    #region Private Methods

    private void ResetFilters()
    {
        IsAllFilterActive = false;
        IsCompletedFilterActive = false;
        IsActiveFilterActive = false;
    }

    private async Task LoadHistoryData()
    {
        // TODO: Charger les sessions réelles depuis le service
        // Pour l'instant, données statiques
        await Task.CompletedTask;
    }

    private void FilterHistory(string filter)
    {
        // TODO: Implémenter la logique de filtrage réelle
        switch (filter)
        {
            case "Tout":
                FilteredSessions = new ObservableCollection<LockerSession>(Sessions);
                break;
            case "Terminées":
                FilteredSessions = new ObservableCollection<LockerSession>(
                    Sessions.Where(s => s.Status == "Completed"));
                break;
            case "Actives":
                FilteredSessions = new ObservableCollection<LockerSession>(
                    Sessions.Where(s => s.Status == "Active"));
                break;
        }
    }

    #endregion
}
