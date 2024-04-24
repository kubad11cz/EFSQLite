using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using EFSQLite.Data;
using EFSQLite.Models;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QRCoder;

namespace EFSQLite
{
    public partial class OFaktury : ContentPage
    {
        
        Ofaktury _context;

        public OFaktury()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _context = new();
            InitializeComponent();
            lst.ItemsSource = _context.Student.ToList(); // p�ipojen� zdroje dat k ListView
            forName.ItemsSource = _context.Student.ToList();
        }

        private void SaveStudent(object sender, EventArgs e)
        {
            Ofaktury2 newStudent = new()
            {
                
                SurName = forSurname.Text,
                Address = forAddress.Text,
                PSC = forPSC.Text,
                Email = forEmail.Text,
                Number = forNumber.Text,
                Atrribute = forAtrribute.Text,
                Price = forPrice.Text,
                Zpusob = forZpusob.Text,
                AccountNumber = forAccountNumber.Text,
                PocetKusu = forPocetKusu.Text,
            };

            _context.Add(newStudent); // p�id� z�znam do Data Setu
            _context.SaveChanges(); // ulo�� zm�ny do datab�ze !!!!!!
            refresh();
        }

        private void Smazat(object sender, EventArgs e)
        {
            Ofaktury2 keSmazani = lst.SelectedItem as Ofaktury2;
            if (keSmazani != null)
            {
                _context.Student.Remove(keSmazani); // odebr�n� studenta z data setu
                _context.SaveChanges(); // ulo�� zm�ny do datab�ze
                refresh();
            }
        }


        private async void PDF(object sender, EventArgs e)
        {
            Ofaktury2 keSmazani = lst.SelectedItem as Ofaktury2;

            string accountNumber = keSmazani.AccountNumber;
            int celkcena = Int32.Parse(keSmazani.Price) * Int32.Parse(keSmazani.PocetKusu);

            string paymentString = $"SPD1.0ACC:{accountNumber}*AM:{celkcena}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(paymentString, QRCodeGenerator.ECCLevel.L);

            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20);

            string imageFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "qrcode.png");
            File.WriteAllBytes(imageFilePath, qrCodeBytes);
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "invoice.pdf");
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(QuestPDF.Helpers.Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    // Z�hlav� faktury
                    page.Header()
                        .AlignCenter()
                        .Text("Faktura")
                        .SemiBold().FontSize(24).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);


                    // Informace o z�kazn�kovi
                    page.Content()
                        .Background(QuestPDF.Helpers.Colors.Grey.Lighten2)
                        .Padding(10)

                        .Text(text =>
                        {
                            text.Span("Dodavatel: ").Bold();
                            text.Span($"{keSmazani.Name}\n");

                            text.Span("Jm�no a P��jmen�: ").Bold();
                            text.Span($"{keSmazani.SurName}\n");

                            text.Span("Adresa: ").Bold();
                            text.Span($"{keSmazani.Address}\n");

                            text.Span("PS�: ").Bold();
                            text.Span($"{keSmazani.PSC}\n");

                            text.Span("Email: ").Bold();
                            text.Span($"{keSmazani.Email}\n");

                            text.Span("Telefonn� ��slo: ").Bold();
                            text.Span($"{keSmazani.Number}\n");

                            text.Span("Atribut: ").Bold();
                            text.Span($"{keSmazani.Atrribute}\n");

                            text.Span("Cena: ").Bold();
                            text.Span($"{keSmazani.Price}\n");

                            text.Span("Zp�sob platby: ").Bold();
                            text.Span($"{keSmazani.Zpusob}\n");

                            text.Span("��slo ��tu: ").Bold();
                            text.Span($"{keSmazani.AccountNumber}\n");

                            text.Span("Po�et kus�: ").Bold();
                            text.Span($"{keSmazani.PocetKusu}\n");



                            // Celkov� cena
                            double cena;
                            if (double.TryParse(keSmazani.Price, out double parsedPrice) && double.TryParse(keSmazani.PocetKusu, out double parsedQuantity))
                            {
                                cena = parsedPrice * parsedQuantity;
                                text.Span($"Celkov� cena: ").Bold().FontColor(QuestPDF.Helpers.Colors.Red.Darken1);
                                text.Span($"{cena}\n");
                            }
                            else
                            {
                                text.Span("Neplatn� �daje pro v�po�et celkov� ceny.");
                            }





                        });
                    page.Content()

                    .Column(Column =>
                    {
                        Column
                        .Item()
                        .Image("")
                        .WithCompressionQuality(ImageCompressionQuality.High);
                    })
                        
                    ;
                     
                    

                   
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Strana ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(path);
        }



        void refresh()
        {
            lst.ItemsSource = null;
            lst.ItemsSource = _context.Student.ToList();
        }
    }
}
