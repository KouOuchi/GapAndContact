using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace v2scheduler.ViewModel.Utilities
{
    internal class MSColorConverter
    {
        public Brush FromHtmlColor(string htmlColor)
        {
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(htmlColor);
            return new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
        }
        public SolidColorBrush FromHtmlColorToSolidColorBrush(string htmlColor)
        {
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(htmlColor);
            return new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
        }
        public Color FromHtmlColorToColor(string htmlColor)
        {
            System.Drawing.Color c = System.Drawing.ColorTranslator.FromHtml(htmlColor);
            return Color.FromArgb(c.A, c.R, c.G, c.B);
        }
        public Brush FromColor(Color c)
        {
            return new SolidColorBrush(c);
        }
        public string FromColorToHtmlColor(System.Drawing.Color c)
        {
            return System.Drawing.ColorTranslator.ToHtml(c);
        }
        public string FromColorToHtmlColor(Color c)
        {
            return System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B));
        }
        public string ToHtmlColor(Brush brush)
        {
            SolidColorBrush s = brush as SolidColorBrush;

            System.Drawing.Color c = System.Drawing.Color.FromArgb(s.Color.A, s.Color.R, s.Color.G, s.Color.B);
            return System.Drawing.ColorTranslator.ToHtml(c);
        }
        public string ToHtmlColor(SolidColorBrush s)
        {
            System.Drawing.Color c = System.Drawing.Color.FromArgb(s.Color.A, s.Color.R, s.Color.G, s.Color.B);
            return System.Drawing.ColorTranslator.ToHtml(c);
        }
    }
}
