using SmartLockerApp.Services;
using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class HomePage : ContentPage
{
    private HomeViewModel ViewModel => (HomeViewModel)BindingContext;

    public HomePage()
    {
        InitializeComponent();
        BindingContext = ViewModelLocator.Instance.HomeViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
