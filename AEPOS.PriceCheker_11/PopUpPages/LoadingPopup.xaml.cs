namespace AEPOS.PriceChecker_11.PopUpPages;

public partial class LoadingPopup : ContentPage
{
    public LoadingPopup()
    {
        InitializeComponent();
    }
    public void Show()
    {
        popup.IsOpen = true;
    }

    public void Hide()
    {
        popup.IsOpen = false;
    }
}