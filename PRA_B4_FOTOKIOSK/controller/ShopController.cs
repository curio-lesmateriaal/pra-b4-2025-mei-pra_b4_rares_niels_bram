using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class ShopController
    {

        public static Home Window { get; set; }

        public void Start()
        {


            //// Stel de prijslijst in aan de rechter kant.
            //ShopManager.SetShopPriceList("Prijzen:\nFoto 10x15: €2.55");

            //// Stel de bon in onderaan het scherm
            //ShopManager.SetShopReceipt("Eindbedrag\n€");

            // Vul de productlijst met producten
            ShopManager.Products.Add(new KioskProduct() { Name = "Foto 10x15", Prijs = 2.55 });

            foreach (KioskProduct product in ShopManager.Products)
            {
                ShopManager.AddShopPriceList($"\n{product.Name} : €{product.Prijs}");
            }

            // Update dropdown met producten
            ShopManager.UpdateDropDownProducts();
        }

        // Wordt uitgevoerd wanneer er op de Toevoegen knop is geklikt
        public void AddButtonClick()
        {
            KioskProduct product = ShopManager.GetSelectedProduct();
            int? fotoID = ShopManager.GetFotoId();
            int? amount = ShopManager.GetAmount();
            if (fotoID == null) { fotoID = 0; }
            if (amount == null) { amount = 1; }
            if (product != null) {
                double totaalPrijs = product.Prijs * amount.Value;

                OrderedProduct order = new OrderedProduct()
                {
                    FotoNummer = fotoID.Value,
                    ProductNaam = product.Name,
                    Aantal = amount.Value,
                    TotaalPrijs = totaalPrijs
                };

                ShopManager.OrderedProducts.Add(order);
                
                StringBuilder sb = new StringBuilder();
                double totaal = 0;

                sb.AppendLine("Eindbedrag");

                foreach (OrderedProduct item in ShopManager.OrderedProducts) {
                    totaal += item.TotaalPrijs;
                }

                sb.AppendLine($"€{totaal:0.00}\n");

                foreach (OrderedProduct item in ShopManager.OrderedProducts)
                {
                    sb.AppendLine($"{item.ProductNaam} ({item.FotoNummer}) : €{item.TotaalPrijs:0.00} ({item.Aantal}x)");
                }

                ShopManager.SetShopReceipt( sb.ToString() );
                //ShopManager.AddShopReceipt($"{product.Name} ({fotoID}) : €{product.Prijs} x{amount}\n");
            }

        }

        // Wordt uitgevoerd wanneer er op de Resetten knop is geklikt
        public void ResetButtonClick()
        {
            ShopManager.OrderedProducts.Clear();
            ShopManager.SetShopReceipt("");
        }

        // Wordt uitgevoerd wanneer er op de Save knop is geklikt
        public void SaveButtonClick()
        {
            if (ShopManager.OrderedProducts.Count == 0)
            {
                MessageBox.Show("Er staan geen producten op de bon om op te slaan.", "Lege bon", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string bonMap = Path.Combine("..", "..", "..", "BonOpslag");


            if (!Directory.Exists(bonMap))
            {
                Directory.CreateDirectory(bonMap);  
                return;
            }

            string fileName = $"Kassabon_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(bonMap, fileName);

            StringBuilder sb = new StringBuilder();
            double totaal = 0;

            sb.AppendLine("KASSABON");
            sb.AppendLine("---------");

            foreach(OrderedProduct item in ShopManager.OrderedProducts)
            {
                totaal = item.TotaalPrijs;
                sb.AppendLine($"{item.ProductNaam} ({item.FotoNummer}) : €{item.TotaalPrijs:0.00} ({item.Aantal}x)");
            }

            sb.AppendLine("---------");
            sb.AppendLine($"Totaal: €{totaal:0.00}");

            File.WriteAllText(filePath, sb.ToString());

            MessageBox.Show($"Bon opgeslagen in:\n{Path.GetFullPath(filePath)}", "Bon opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
