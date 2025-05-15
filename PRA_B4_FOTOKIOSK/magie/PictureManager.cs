using PRA_B4_FOTOKIOSK.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PRA_B4_FOTOKIOSK.magie
{
    public class PictureManager
    {

        public static Home Instance { get; set; }

        public static void UpdatePictures(List<KioskPhoto> PicturesToDisplay)
        {
            int GridRow = 0;
            int GridCulumn;
            
            Instance.spPictures.Children.Clear();
            foreach (KioskPhoto picture in PicturesToDisplay)
            {
                if(GridRow <= 4)
                {
                    Image image = new Image();
                    var bitmap = pathToImage(picture.Source);
                    image.Source = bitmap;
                    image.Width = 1920 / 3.5;
                    image.Height = 1080 / 3.5;
                    //set images to grid 3x3
                    Grid.SetRow(image, GridRow);
                    GridRow++;
                    Instance.spPictures.Children.Add(image);
                    Console.WriteLine(picture);
                }
            }
        }

        public static BitmapImage pathToImage(string path)
        {
            var stream = new MemoryStream(File.ReadAllBytes(path));
            var img = new System.Windows.Media.Imaging.BitmapImage();

            img.BeginInit();
            img.StreamSource = stream;
            img.EndInit();

            return img;
        }

    }
}
