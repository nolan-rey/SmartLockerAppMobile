using SmartLockerApp.ViewModels;

namespace SmartLockerApp.Views;

public partial class PaymentPage : ContentPage
{
    public PaymentPage(PaymentPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is PaymentPageViewModel viewModel)
        {
            await viewModel.LoadDataCommand.ExecuteAsync(null);
        }
    }
}
