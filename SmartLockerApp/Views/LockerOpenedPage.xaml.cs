using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class LockerOpenedPage : ContentPage
{
    public LockerOpenedPage(LockerOpenedPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is LockerOpenedPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
