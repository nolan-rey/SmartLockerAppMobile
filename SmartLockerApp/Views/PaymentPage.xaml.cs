namespace SmartLockerApp.Views;

public partial class PaymentPage : ContentPage
{
    private string selectedPaymentMethod = "CreditCard";

    public PaymentPage()
    {
        InitializeComponent();
    }

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void PaymentMethod_Tapped(object sender, EventArgs e)
    {
        var tappedBorder = sender as Border;
        
        // Reset all payment method styles
        ResetPaymentMethodStyles();
        
        // Set selected style
        if (tappedBorder == CreditCardOption)
        {
            selectedPaymentMethod = "CreditCard";
            SetSelectedStyle(CreditCardOption);
        }
        else if (tappedBorder == PayPalOption)
        {
            selectedPaymentMethod = "PayPal";
            SetSelectedStyle(PayPalOption);
        }
        else if (tappedBorder == ApplePayOption)
        {
            selectedPaymentMethod = "ApplePay";
            SetSelectedStyle(ApplePayOption);
        }
    }

    private void ResetPaymentMethodStyles()
    {
        var options = new[] { CreditCardOption, PayPalOption, ApplePayOption };
        
        foreach (var option in options)
        {
            option.BackgroundColor = Colors.White;
            option.Stroke = Color.FromArgb("#E2E8F0");
            option.StrokeThickness = 1;
            
            // Reset radio button
            var grid = option.Content as Grid;
            var radioButton = grid.Children[0] as Border;
            radioButton.BackgroundColor = Colors.Transparent;
        }
    }

    private void SetSelectedStyle(Border selectedOption)
    {
        selectedOption.BackgroundColor = Color.FromArgb("#F8FAFC");
        selectedOption.Stroke = Color.FromArgb("#2563EB");
        selectedOption.StrokeThickness = 2;
        
        // Set radio button
        var grid = selectedOption.Content as Grid;
        var radioButton = grid.Children[0] as Border;
        radioButton.BackgroundColor = Color.FromArgb("#2563EB");
    }

    private async void PayButton_Clicked(object sender, EventArgs e)
    {
        // TODO: Implement actual payment processing
        await DisplayAlert("Paiement", $"Paiement de €5.75 effectué avec {selectedPaymentMethod}", "OK");
        await Shell.Current.GoToAsync("//HomePage");
    }

    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        bool result = await DisplayAlert("Annuler", "Êtes-vous sûr de vouloir annuler le paiement?", "Oui", "Non");
        if (result)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
