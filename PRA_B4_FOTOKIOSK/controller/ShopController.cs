using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class ShopController
    {
        public static Home Window { get; set; }

        public void Start()
        {
            // Voeg producten toe aan de lijst
            ShopManager.Products.Add(new KioskProduct() { Name = "Foto 10x15", Prijs = 2.99 });
            ShopManager.Products.Add(new KioskProduct() { Name = "Mug met foto 10x15", Prijs = 5.99 });
            ShopManager.Products.Add(new KioskProduct() { Name = "T-shirt met foto 10x15", Prijs = 7.99 });

            foreach (KioskProduct product in ShopManager.Products)
            {
                ShopManager.AddShopPriceList($"\n{product.Name} : €{product.Prijs}");
            }

            // Update dropdown met producten
            ShopManager.UpdateDropDownProducts();
        }

        public void AddButtonClick()
        {
            KioskProduct product = ShopManager.GetSelectedProduct();
            int? fotoID = ShopManager.GetFotoId();
            int? amount = ShopManager.GetAmount();

            if (fotoID == null || fotoID <= 0)
            {
                MessageBox.Show("Voer een geldig foto-ID in.", "Ongeldige invoer", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ Zoek exact overeenkomend fotopad op basis van ID
            string matchedFotoPath = null;
            string matchedFotoId = null;

            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // e.g. 10_05_30_id1234
                    if (fileName.Contains($"id{fotoID}.") || fileName.EndsWith($"id{fotoID}")) // stricter match
                    {
                        matchedFotoPath = file;
                        int idStart = fileName.IndexOf("id") + 2;
                        string realId = fileName.Substring(idStart); // extract "1234"
                        matchedFotoId = realId;
                        break;
                    }
                }
                if (matchedFotoPath != null) break;
            }

            if (matchedFotoPath == null)
            {
                MessageBox.Show($"Geen foto gevonden met exact ID: {fotoID}", "ID niet gevonden", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (amount == null) { amount = 1; }

            if (product != null)
            {
                double totaalPrijs = product.Prijs * amount.Value;

                OrderedProduct order = new OrderedProduct()
                {
                    FotoNummer = int.Parse(matchedFotoId),
                    ProductNaam = product.Name,
                    Aantal = amount.Value,
                    TotaalPrijs = totaalPrijs
                };

                ShopManager.OrderedProducts.Add(order);

                StringBuilder sb = new StringBuilder();
                double totaal = 0;

                sb.AppendLine("Eindbedrag");

                foreach (OrderedProduct item in ShopManager.OrderedProducts)
                {
                    totaal += item.TotaalPrijs;
                }

                sb.AppendLine($"€{totaal:0.00}\n");

                foreach (OrderedProduct item in ShopManager.OrderedProducts)
                {
                    sb.AppendLine($"{item.ProductNaam} ({item.FotoNummer}) : €{item.TotaalPrijs:0.00} ({item.Aantal}x)");
                }

                ShopManager.SetShopReceipt(sb.ToString());
            }
        }


        public void ResetButtonClick()
        {
            ShopManager.OrderedProducts.Clear();
            ShopManager.SetShopReceipt("");
        }

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
            }

            string fileName = $"Kassabon_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = Path.Combine(bonMap, fileName);

            StringBuilder sb = new StringBuilder();
            double totaal = 0;

            sb.AppendLine("KASSABON");
            sb.AppendLine("---------");

            foreach (OrderedProduct item in ShopManager.OrderedProducts)
            {
                totaal += item.TotaalPrijs;
                sb.AppendLine($"{item.ProductNaam} ({item.FotoNummer}) : €{item.TotaalPrijs:0.00} ({item.Aantal}x)");
            }

            sb.AppendLine("---------");
            sb.AppendLine($"Totaal: €{totaal:0.00}");

            File.WriteAllText(filePath, sb.ToString());

            MessageBox.Show($"Bon opgeslagen in:\n{Path.GetFullPath(filePath)}", "Bon opgeslagen", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
