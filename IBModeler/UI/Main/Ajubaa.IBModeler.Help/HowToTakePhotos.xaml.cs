using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ajubaa.IBModeler.Help
{
    /// <summary>
    /// Interaction logic for HowToTakePhotos.xaml
    /// </summary>
    public partial class HowToTakePhotos : Window
    {
        public HowToTakePhotos()
        {
            InitializeComponent();
            //trick to find out the names of all the fonts added in the fonts folder
            var fonts = new List<FontFamily>();
            foreach (var fontFamily in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./Fonts/"))
            {
                fonts.Add(fontFamily);
                //fontFamily will give you the actual name of the font to embed
            }
        }
    }
}
