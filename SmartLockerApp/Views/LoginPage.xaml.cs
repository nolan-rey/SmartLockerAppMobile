using SmartLockerApp.Services;
using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class LoginPage : ContentPage
{
    private LoginViewModel ViewModel => (LoginViewModel)BindingContext;

    public LoginPage(ViewModelLocator viewModelLocator)
    {
        InitializeComponent();
        BindingContext = viewModelLocator.LoginViewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}
