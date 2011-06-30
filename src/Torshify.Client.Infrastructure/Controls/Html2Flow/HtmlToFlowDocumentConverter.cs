using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Torshify.Client.Infrastructure.Controls.Html2Flow
{
    public class HtmlToFlowDocumentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                FlowDocument flowDocument = new FlowDocument();
                flowDocument.FontSize = 12;
                flowDocument.FontFamily = new FontFamily("Segoe UI");

                string xaml = HtmlToXamlConverter.ConvertHtmlToXaml(value.ToString(), false);

                using (MemoryStream stream = new MemoryStream((new ASCIIEncoding()).GetBytes(xaml)))
                {
                    TextRange text = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
                    text.Load(stream, DataFormats.Xaml);
                }

                return flowDocument;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}