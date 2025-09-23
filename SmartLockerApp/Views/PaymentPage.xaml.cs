using SmartLockerApp.Services;

namespace SmartLockerApp.Views;

[QueryProperty(nameof(SessionId), "sessionId")]
[QueryProperty(nameof(Action), "action")]
public partial class PaymentPage : ContentPage
{
    private readonly AppStateService _appState = AppStateService.Instance;
    private string selectedPaymentMethod = "CreditCard";
    public string SessionId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // "receipt" pour afficher le reçu

    public PaymentPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        // Vérifier si nous avons les paramètres nécessaires
        if (string.IsNullOrEmpty(SessionId))
        {
            await DisplayAlert("Erreur", "Session introuvable", "OK");
            await Shell.Current.GoToAsync("//HomePage");
            return;
        }
        
        if (Action == "receipt")
        {
            // Afficher comme reçu de paiement
            Title = "Reçu de paiement";
            // Masquer les options de paiement et afficher le résumé
            await UpdateUIForReceipt();
        }
        else
        {
            // Mode paiement normal - s'assurer que les boutons sont visibles
            Title = "Paiement";
            PayButton.IsVisible = true;
            CancelButton.IsVisible = true;
            
            // Charger les données de session pour le paiement normal
            var session = await _appState.GetSessionAsync(SessionId);
            if (session != null)
            {
                UpdateSessionDisplay(session);
            }
            else
            {
                await DisplayAlert("Erreur", "Session introuvable", "OK");
                await Shell.Current.GoToAsync("//HomePage");
            }
        }
    }

    private async Task UpdateUIForReceipt()
    {
        // Masquer les options de paiement et le bouton de paiement
        PayButton.IsVisible = false;
        CancelButton.IsVisible = false;
        
        // Modifier le titre de la page
        Title = "Reçu de paiement";
        
        // Charger les détails de la session terminée
        if (!string.IsNullOrEmpty(SessionId))
        {
            var session = await _appState.GetSessionAsync(SessionId);
            if (session != null)
            {
                // Mettre à jour les informations de session
                UpdateSessionDisplay(session);
                
                // Afficher un message de confirmation
                await DisplayAlert("Session terminée", 
                    $"Votre session a été clôturée avec succès.\nCoût total: {session.TotalCost:C}", 
                    "OK");
            }
        }
        
        // Changer le bouton de retour pour aller à la page d'accueil
        BackButton.Text = "Retour à l'accueil";
    }

    private void UpdateSessionDisplay(Models.LockerSession session)
    {
        // Mapper l'ID du service vers l'ID d'affichage
        var displayId = MapServiceIdToDisplayId(session.LockerId);
        SessionLockerLabel.Text = displayId;
        
        // Afficher la durée
        SessionDurationLabel.Text = $"{session.DurationHours}h";
        
        // Afficher les heures de début et fin
        SessionStartTimeLabel.Text = session.StartTime.ToString("HH:mm");
        SessionEndTimeLabel.Text = session.EndTime.ToString("HH:mm");
        
        // Afficher le coût total
        SessionTotalLabel.Text = session.TotalCost.ToString("C");
        
        // Mettre à jour le bouton de paiement avec le montant réel
        PayButton.Text = $"Payer {session.TotalCost:C}";
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

    private async void BackButton_Clicked(object sender, EventArgs e)
    {
        if (Action == "receipt")
        {
            // Retourner à la page d'accueil après avoir vu le reçu
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await Shell.Current.GoToAsync("..");
        }
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
