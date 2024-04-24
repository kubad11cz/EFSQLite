using EFSQLite.Data;
using EFSQLite.Models;

namespace EFSQLite;

public partial class FirtsPage : ContentPage
{
    public FirtsPage()
    {
        InitializeComponent();
    }

    private void Vytvor(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new NewPage1());
    }

    private void OFaktury(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new OFaktury());
    }

    private void PFaktury(object sender, EventArgs e)
    {
        //  await Navigation.PushAsync(new Page1());
    }
}

