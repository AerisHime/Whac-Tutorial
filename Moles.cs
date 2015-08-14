using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Maulwurf_MVA
{
    class Moles
    {
        public Image Img { get; set; }
        public int Timer { get; set; }
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public void Generate(int position)
        {
            Timer = rnd.Next(1, 4);
            int row = position / 3;
            int column = position % 3;            
            Img = new Image();
            Img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Target.png"));
            Img.SetValue(Grid.RowProperty, row);
            Img.SetValue(Grid.ColumnProperty, column);
        }
    }
}
