using System;
using System.Windows.Media;

namespace Torshify.Client.Modules.EchoNest.Views.Similar
{
    public class HslColor
    {
        #region Fields

        private const double Scale = 240.0;

        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double _hue = 1.0;
        private double _luminosity = 1.0;
        private double _saturation = 1.0;

        #endregion Fields

        #region Constructors

        public HslColor()
        {
        }

        public HslColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }

        public HslColor(byte red, byte green, byte blue)
        {
            SetRGB(red, green, blue);
        }

        public HslColor(double hue, double saturation, double luminosity)
        {
            this.Hue = hue;
            this.Saturation = saturation;
            this.Luminosity = luminosity;
        }

        #endregion Constructors

        #region Properties

        public double Hue
        {
            get { return _hue * Scale; }
            set { _hue = CheckRange(value / Scale); }
        }

        public double Luminosity
        {
            get { return _luminosity * Scale; }
            set { _luminosity = CheckRange(value / Scale); }
        }

        public double Saturation
        {
            get { return _saturation * Scale; }
            set { _saturation = CheckRange(value / Scale); }
        }

        #endregion Properties

        #region Methods

        public static implicit operator Color(HslColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor._luminosity != 0)
            {
                if (hslColor._saturation == 0)
                    r = g = b = hslColor._luminosity;
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0 * hslColor._luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor._hue);
                    b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                }
            }
            return Color.FromRgb((byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
        }

        public static implicit operator HslColor(Color color)
        {
            HslColor hslColor = new HslColor();
            hslColor._hue = color.GetHue() / 360.0; // we store hue as 0-1 as opposed to 0-360
            hslColor._luminosity = color.GetBrightness();
            hslColor._saturation = color.GetSaturation();
            return hslColor;
        }

        public void SetRGB(byte red, byte green, byte blue)
        {
            HslColor hslColor = (HslColor)Color.FromRgb(red, green, blue);
            this._hue = hslColor._hue;
            this._saturation = hslColor._saturation;
            this._luminosity = hslColor._luminosity;
        }

        public string ToRGBString()
        {
            Color color = (Color)this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}",   Hue, Saturation, Luminosity);
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

        private static double GetTemp2(HslColor hslColor)
        {
            double temp2;
            if (hslColor._luminosity < 0.5)  //<=??
                temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
            else
                temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
            return temp2;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        #endregion Methods
    }
}