using EFSQLite.Data;
using EFSQLite.Models;
using Microsoft.Maui.Controls;

namespace EFSQLite;

public partial class NewPage1 : ContentPage
{
    MyContext _context;

    public NewPage1()
    {
        _context = new();
        InitializeComponent();
        lst.ItemsSource = _context.Students.ToList(); // p�ipojen� zdroje dat k ListView
    }

    private void SaveStudent(object sender, EventArgs e)
    {
        Student newStudent = new()
        {
            Name = forName.Text,
            Email = forEmail.Text,
            IC = forIC.Text,
            DIC = forDIC.Text,
            Number = forNumber.Text,
            Address = forAddress.Text,
            //Surname = forSurname.Text 
        };

        _context.Add(newStudent); // p�id� z�znam do Data Setu
        _context.SaveChanges(); // ulo�� zm�ny do datab�ze !!!!!!
        refresh();
    }

    private void Smazat(object sender, EventArgs e)
    {
        Student keSmazani = lst.SelectedItem as Student;
        if (keSmazani != null)
        {
            _context.Students.Remove(keSmazani); // odebr�n� studenta z data setu
            _context.SaveChanges(); // ulo�� zm�ny do datab�ze
            refresh();
        }
    }

    private async void Detajly(object sender, EventArgs e)
    {

    }

    void refresh()
    {
        lst.ItemsSource = null;
        lst.ItemsSource = _context.Students.ToList();
    }


}