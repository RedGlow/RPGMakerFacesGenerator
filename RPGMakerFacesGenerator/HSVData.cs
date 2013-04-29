using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;

namespace RPGMakerFacesGenerator
{
    public class HSVData : INotifyPropertyChanged
    {
        private float hShift = 0.0f;

        public float HShift
        {
            get
            {
                return hShift;
            }
            set
            {
                value = Math.Max(0.0f, Math.Min(1.0f, value));
                if (value != hShift)
                {
                    hShift = value;
                    propChanged("HShift");
                    propChanged("ResultingBaseColor");
                }
            }
        }

        private float sFactor = 1.0f;

        public float SFactor
        {
            get
            {
                return sFactor;
            }
            set
            {
                value = Math.Max(0.0f, Math.Min(2.0f, value));
                if (value != sFactor)
                {
                    sFactor = value;
                    propChanged("SFactor");
                    propChanged("ResultingBaseColor");
                }
            }
        }

        private float vFactor = 1.0f;

        public float VFactor
        {
            get
            {
                return vFactor;
            }
            set
            {
                value = Math.Max(0.0f, Math.Min(2.0f, value));
                if (value != vFactor)
                {
                    vFactor = value;
                    propChanged("VFactor");
                    propChanged("ResultingBaseColor");
                }
            }
        }

        public Color ResultingBaseColor
        {
            get
            {
                var h = (188 + (int)(HShift * 360)) % 360;
                var s = 0.85 * SFactor;
                var v = 0.60 * VFactor;
                var color = colorFromHSV(h, s, v);
                return color;
            }
        }

        // taken from http://stackoverflow.com/a/1626175/909280
        private static Color colorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int vi = Convert.ToInt32(value);
            int pi = Convert.ToInt32(value * (1 - saturation));
            int qi = Convert.ToInt32(value * (1 - f * saturation));
            int ti = Convert.ToInt32(value * (1 - (1 - f) * saturation));
            byte v = (byte)vi;
            byte p = (byte)pi;
            byte q = (byte)qi;
            byte t = (byte)ti;

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        private void propChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
