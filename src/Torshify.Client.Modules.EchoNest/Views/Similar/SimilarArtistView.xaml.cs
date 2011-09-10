using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    public partial class SimilarArtistView : UserControl
    {
        #region Constructors

        public SimilarArtistView(SimilarArtistViewModel viewModel)
        {
            InitializeComponent();
            Model = viewModel;
            Model.Graph = _graph;
        }

        #endregion Constructors

        #region Properties

        public SimilarArtistViewModel Model
        {
            get { return DataContext as SimilarArtistViewModel; }
            set { DataContext = value; }
        }

        #endregion Properties
    }

    public class HotttnesssToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double hotttnesss = System.Convert.ToDouble(value);

            HSLColor c = new HSLColor(Colors.Red);
            c.Hue = hotttnesss*255;

            Color color = (Color) c;
            return new SolidColorBrush(Lerp(Colors.Gray, color, (float)hotttnesss));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static float Lerp(float start, float end, float amount)
        {
            float difference = end - start;
            float adjusted = difference * amount;
            return start + adjusted;
        }

        public static Color Lerp(Color colour, Color to, float amount)
        {
            // start colours as lerp-able floats
            float sr = colour.R, sg = colour.G, sb = colour.B;

            // end colours as lerp-able floats
            float er = to.R, eg = to.G, eb = to.B;

            // lerp the colours to get the difference
            byte r = (byte)Lerp(sr, er, amount),
                 g = (byte)Lerp(sg, eg, amount),
                 b = (byte)Lerp(sb, eb, amount);

            // return the new colour
            return Color.FromRgb(r, g, b);
        }
    }

    public class MultiplyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value)*System.Convert.ToDouble(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HSLColor
     {
         // Private data members below are on scale 0-1
         // They are scaled for use externally based on scale
         private double hue = 1.0;
         private double saturation = 1.0;
         private double luminosity = 1.0;
  
         private const double scale = 240.0;
  
         public double Hue
         {
             get { return hue * scale; }
             set { hue = CheckRange(value / scale); }
         }
         public double Saturation
         {
             get { return saturation * scale; }
             set { saturation = CheckRange(value / scale); }
         }
         public double Luminosity
         {
             get { return luminosity * scale; }
             set { luminosity = CheckRange(value / scale); }
         }
  
         private double CheckRange(double value)
         {
             if (value < 0.0)
                 value = 0.0;
             else if (value > 1.0)
                 value = 1.0;
             return value;
         }
  
         public override string ToString()
         {
             return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}",   Hue, Saturation, Luminosity);
         }
  
         public string ToRGBString()
         {
             Color color = (Color)this;
             return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
         }
  
         #region Casts to/from System.Drawing.Color
         public static implicit operator Color(HSLColor hslColor)
         {
             double r = 0, g = 0, b = 0;
             if (hslColor.luminosity != 0)
             {
                 if (hslColor.saturation == 0)
                     r = g = b = hslColor.luminosity;
                 else
                 {
                     double temp2 = GetTemp2(hslColor);
                     double temp1 = 2.0 * hslColor.luminosity - temp2;
  
                     r = GetColorComponent(temp1, temp2, hslColor.hue + 1.0 / 3.0);
                     g = GetColorComponent(temp1, temp2, hslColor.hue);
                     b = GetColorComponent(temp1, temp2, hslColor.hue - 1.0 / 3.0);
                 }
             }
             return Color.FromRgb((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
         }
  
         private static double GetColorComponent(double temp1, double temp2, double temp3)
         {
             temp3 = MoveIntoRange(temp3);
             if (temp3 < 1.0 / 6.0)
                 return temp1 + (temp2 - temp1) * 6.0 * temp3;
             else if (temp3 < 0.5)
                 return temp2;
             else if (temp3 < 2.0 / 3.0)
                 return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
             else
                 return temp1;
         }
         private static double MoveIntoRange(double temp3)
         {
             if (temp3 < 0.0)
                 temp3 += 1.0;
             else if (temp3 > 1.0)
                 temp3 -= 1.0;
             return temp3;
         }
         private static double GetTemp2(HSLColor hslColor)
         {
             double temp2;
             if (hslColor.luminosity < 0.5)  //<=??
                 temp2 = hslColor.luminosity * (1.0 + hslColor.saturation);
             else
                 temp2 = hslColor.luminosity + hslColor.saturation - (hslColor.luminosity * hslColor.saturation);
             return temp2;
         }
  
         public static implicit operator HSLColor(Color color)
         {
             HSLColor hslColor = new HSLColor();
             hslColor.hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360 
             hslColor.luminosity = color.GetBrightness();
             hslColor.saturation = color.GetSaturation();
             return hslColor;
         }
         #endregion
  
         public void SetRGB(byte red, byte green, byte blue)
         {
             HSLColor hslColor = (HSLColor)Color.FromRgb(red, green, blue);
             this.hue = hslColor.hue;
             this.saturation = hslColor.saturation;
             this.luminosity = hslColor.luminosity;
         }
  
         public HSLColor() { }
         public HSLColor(Color color)
         {
             SetRGB(color.R, color.G, color.B);
         }
         public HSLColor(byte red, byte green, byte blue)
         {
             SetRGB(red, green, blue);
         }
         public HSLColor(double hue, double saturation, double luminosity)
         {
             this.Hue = hue;
             this.Saturation = saturation;
             this.Luminosity = luminosity;
         }


     }

    public static class ColorExtensions
    {
        public static float GetBrightness(this Color color)
        {
            float num = ((float)color.R) / 255f;
            float num2 = ((float)color.G) / 255f;
            float num3 = ((float)color.B) / 255f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            return ((num4 + num5) / 2f);
        }

        public static float GetHue(this Color color)
        {
            if ((color.R == color.G) && (color.G == color.B))
            {
                return 0f;
            }
            float num = ((float)color.R) / 255f;
            float num2 = ((float)color.G) / 255f;
            float num3 = ((float)color.B) / 255f;
            float num7 = 0f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            float num6 = num4 - num5;
            if (num == num4)
            {
                num7 = (num2 - num3) / num6;
            }
            else if (num2 == num4)
            {
                num7 = 2f + ((num3 - num) / num6);
            }
            else if (num3 == num4)
            {
                num7 = 4f + ((num - num2) / num6);
            }
            num7 *= 60f;
            if (num7 < 0f)
            {
                num7 += 360f;
            }
            return num7;
        }

        public static float GetSaturation(this Color color)
        {
            float num = ((float)color.R) / 255f;
            float num2 = ((float)color.G) / 255f;
            float num3 = ((float)color.B) / 255f;
            float num7 = 0f;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            if (num4 == num5)
            {
                return num7;
            }
            float num6 = (num4 + num5) / 2f;
            if (num6 <= 0.5)
            {
                return ((num4 - num5) / (num4 + num5));
            }
            return ((num4 - num5) / ((2f - num4) - num5));
        }

 


 

    }
}