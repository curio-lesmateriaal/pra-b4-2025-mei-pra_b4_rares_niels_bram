using PRA_B4_FOTOKIOSK.magie;
using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRA_B4_FOTOKIOSK.controller
{
    public class SearchController
    {
        // De window die we laten zien op het scherm
        public static Home Window { get; set; }

        // Start methode die wordt aangeroepen wanneer de zoek pagina opent.
        public void Start()
        {
            SearchManager.Instance = Window;

            // Optioneel: Reset eerdere zoekresultaten
            SearchManager.SetSearchImageInfo(" ");
        }

        // Wordt uitgevoerd wanneer er op de Zoeken knop is geklikt
        public void SearchButtonClick()
        {
            string inputId = SearchManager.GetSearchInput();
            bool found = false;

            foreach (string dir in Directory.GetDirectories(@"../../../fotos"))
            {
                foreach (string file in Directory.GetFiles(dir))
                {
                    string fileName = Path.GetFileNameWithoutExtension(file); // bv: "10_05_30_id8824"

                    if (fileName.Contains("id" + inputId))
                    {
                        // Toon de afbeelding
                        SearchManager.SetPicture(file);

                        // ✅ INFO VOOR C3
                        string[] parts = fileName.Split('_'); // ["10", "05", "30", "id8824"]
                        if (parts.Length >= 4)
                        {
                            string time = $"{parts[0]}:{parts[1]}:{parts[2]}";
                            string id = parts[3].Replace("id", "");

                            // Datum bepalen op basis van mapnaam (bijv. \fotos\2_Dinsdag)
                            string folderName = new DirectoryInfo(dir).Name;
                            string dayText = folderName.Contains("_") ? folderName.Split('_')[1] : folderName;

                            string info = $"ID: {id} | Tijd: {time} | Dag: {dayText}";
                            SearchManager.SetSearchImageInfo(info);
                        }
                        else
                        {
                            SearchManager.SetSearchImageInfo("Foto gevonden, maar info is onvolledig.");
                        }

                        found = true;
                        break;
                    }
                }

                if (found) break;
            }

            if (!found)
            {
                SearchManager.SetPicture(null);
                SearchManager.SetSearchImageInfo("Geen foto gevonden met ID: " + inputId);
            }
        }

    }
}
