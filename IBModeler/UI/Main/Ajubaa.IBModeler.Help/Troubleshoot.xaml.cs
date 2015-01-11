using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Ajubaa.IBModeler.Help
{
    /// <summary>
    /// Interaction logic for Troubleshoot.xaml
    /// </summary>
    public partial class Troubleshoot
    {
        public Troubleshoot()
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
