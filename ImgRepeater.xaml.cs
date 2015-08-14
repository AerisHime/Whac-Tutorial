using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Maulwurf_MVA
{
    public sealed partial class ImgRepeater : UserControl
    {
        public int Count { get { return count; }
            set
            {
                if (count == value)
                {
                    return;
                }
                count = value;
                SetImage();
            } }

        public ImageSource Img { get
            { return img; }
            set
            {
                if (img == value)
                {
                    return;
                }
                img = value;
                SetImage();
            } }

        private int count;
        private ImageSource img;

        private void SetImage()
        {
            Images.Children.Clear();
            for (int i = 0; i < Count; i++)
            {
                Images.Children.Add(new Image { Source = Img, Stretch = Stretch.None });
            }
        }

        public ImgRepeater()
        {
            this.InitializeComponent();
        }
    }
}
